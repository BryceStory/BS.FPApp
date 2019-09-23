using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FiiiPay.Framework.Exceptions;
using log4net;
using Newtonsoft.Json.Linq;

namespace FiiiPay.Foundation.Business
{
    public class PriceInfoComponent
    {
        #region Constructor

        private readonly ILog Log = LogManager.GetLogger(typeof(PriceInfoComponent));

        private static PriceInfoDAC _priceInfoDAC;

        public static readonly string CoinMarketCapV2_URL = ConfigurationManager.AppSettings.Get("CoinMarketCapV2_URL");

        public static readonly string CMBDexPriceInfoURL = ConfigurationManager.AppSettings.Get("CMBPriceAPI");

        public static readonly string CurrencyCoverterAPIURL =
            ConfigurationManager.AppSettings.Get("CurrencyCoverterAPIURL");

        public static readonly decimal CMBToUSDDefault =
            Convert.ToDecimal((ConfigurationManager.AppSettings.Get("CMBToUSDDefault")));

        public static readonly string FiiiCoinPriceInfoURL =
            ConfigurationManager.AppSettings.Get("FiiiCoinPriceInfoURL");

        public static readonly string WinCoinPriceInfoURL = ConfigurationManager.AppSettings.Get("WinCoinPriceInfoURL");
        public static readonly string Bittrex_URL = ConfigurationManager.AppSettings.Get("Bittrex_URL");

        public static readonly string FiatCurrenciesExchangeRateURL1 =
            ConfigurationManager.AppSettings.Get("FiatCurrenciesExchangeRateURL1");

        private static readonly Dictionary<string, decimal> _fiatExchaneRate = new Dictionary<string, decimal>();

        private static PriceInfoDAC PriceInfoDac
        {
            get
            {
                if (_priceInfoDAC == null)
                {
                    _priceInfoDAC = new PriceInfoDAC();
                }

                return _priceInfoDAC;
            }
        }

        private readonly CurrenciesDAC currenciesDAC = new CurrenciesDAC();
        private readonly CryptocurrencyDAC cryptoCurrDAC = new CryptocurrencyDAC();

        private decimal BTCToUSD = decimal.Zero;

        private readonly List<string> _exceptive = new List<string> { "CMB", "WC", "FIII" };

        #endregion

        public void GetPriceInfo(List<CurrencyConverterModel> currencyConverterModels)
        {
            Task.Run(() =>
            {
                try
                {
                    //CryptoAddressBindingComponent cryptoaddressbindingcomponent = new CryptoAddressBindingComponent();
                    //CryptocurrenciesComponent cryptocurrenciescomponent = new CryptocurrenciesComponent();
                    Stack<Currencies> currencieslist = new Stack<Currencies>();
                    Stack<Currencies> currenciestoinsert = new Stack<Currencies>();

                    //List<Cryptocurrencies> cryptocurrencieslist = cryptocurrenciescomponent.ListAllCurrencies();
                    currenciesDAC.GetAll().ForEach(x => currencieslist.Push(x));
                    //int NoofAddrTreshold = Configuration.NoofAddrTreshold;

                    while (currencieslist.Count > 0)
                    {
                        // Get 5 currencies to update, if < 5, get the remaining
                        if (currencieslist.Count >= 25)
                        {
                            for (int i = 0; i < 25; i++)
                            {
                                currenciestoinsert.Push(currencieslist.Pop());
                            }
                        }
                        else
                        {
                            var currencieslistcount = currencieslist.Count;
                            for (int i = 0; i < currencieslistcount; i++)
                            {
                                currenciestoinsert.Push(currencieslist.Pop());
                            }
                        }

                        while (currenciestoinsert.Count > 0)
                        {
                            Currencies currenciesitem = null;
                            try
                            {
                                currenciesitem = currenciestoinsert.Pop();

                                //UpdateMarketPriceFrmCoinMarketCapToDB(currenciesitem.ID);
                                UpdateMarketPriceFrmBittrexToDB(currenciesitem.ID, currencyConverterModels);
                            }
                            catch (Exception ex)
                            {
                                //string functionpath = "UpdateMarketPriceFrmCoinMarketCapToDB";
                                //EventLogger.WriteToLog(EventLogEntryType.Error, ex.Message + ", unable to update market price for CurrenciesID: " + currenciesitem.ID, ex.StackTrace, functionpath);
                                Log.Error(MethodBase.GetCurrentMethod().Name + "(): " +
                                          "Fail when updating market price for CurrencyName : " + currenciesitem?.Name +
                                          ", Message : " + ex.Message);
                            }
                        }

                        // Invoke the next 5 API call after 5 mins.
                        if (currencieslist.Count > 0)
                        {
                            Thread.Sleep(120000);
                        }
                    }

                    if (BTCToUSD != decimal.Zero)
                    {
                        GetFiiiCoinInfo(BTCToUSD, currencyConverterModels);
                        GetWinCoinInfo(BTCToUSD, currencyConverterModels);
                    }
                    else
                    {
                        Log.Error(MethodBase.GetCurrentMethod().Name + "(): Fail to get price for BTC_to_USD.");
                    }
                }
                catch (Exception ex)
                {
                    //string functionpath = "GetPriceInfo";
                    //EventLogger.WriteToLog(EventLogEntryType.Error, ex.Message, ex.StackTrace, functionpath);

                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): " + "Fail with Message : " + ex.Message +
                              ", StackTrace : " + ex.StackTrace);
                }
            });
        }

        public void GetCMBPriceInfo(List<CurrencyConverterModel> currencyConverterModels)
        {
            Task.Run(() =>
            {
                try
                {
                    //CryptocurrenciesComponent cryptocurrenciescomponent = new CryptocurrenciesComponent();
                    //UtilityComponent utilitycomponent = new UtilityComponent();
                    List<Currencies> currencieslist = new List<Currencies>();
                    //Cryptocurrencies cryptocurrencieslist = new Cryptocurrencies();

                    currencieslist = currenciesDAC.GetAll();
                    var cryptocurrencieslist =
                        cryptoCurrDAC.GetByCode("CMB"); //cryptocurrenciescomponent.GetByName("CMB");

                    if (cryptocurrencieslist == null)
                    {
                        Log.Error(MethodBase.GetCurrentMethod().Name + "(): " + "CMB is not found in database.");
                        throw new CommonException(0, "Error: CMB is not found in database.");
                    }

                    //Get per CMB price in USD currency
                    decimal CMBtoUSDPrice = GetCMBToUSDPriceInfo();

                    //Check whether list contains USD
                    var USDcurrenciesitem =
                        currencieslist.FirstOrDefault(x => string.Compare(x.Code, "USD", true) == 0);

                    if (USDcurrenciesitem != null)
                    {
                        //Save CMB/USD rate to DB
                        if (!UpdateERCPriceInfo(USDcurrenciesitem.ID, cryptocurrencieslist.Id, CMBtoUSDPrice))
                        {
                            Log.Error(MethodBase.GetCurrentMethod().Name + "(): " +
                                      "Fail when updating market price for CurrencyName : " + USDcurrenciesitem.Name +
                                      ", Message : Unable to insert/update PriceInfo for CMB/" +
                                      USDcurrenciesitem.Code + ".");
                        }

                        //Remove USD from currencieslist
                        currencieslist.RemoveAll(x => string.Compare(x.Code, USDcurrenciesitem.Code, true) == 0);
                    }

                    foreach (var item in currencieslist)
                    {
                        try
                        {
                            //var currencyitemprice = ConvertCurrency(USDcurrenciesitem.Code, item.Code, CMBtoUSDPrice);
                            var currencyitemprice = currencyConverterModels.SingleOrDefault(x =>
                                String.Compare(x.CurrencyPairTo, item.Code, true) == 0);

                            //Save CMB/Currency rate to DB
                            if (!UpdateERCPriceInfo(item.ID, cryptocurrencieslist.Id,
                                currencyitemprice.Value * CMBtoUSDPrice))
                            {
                                throw new CommonException(0,
                                    "Error: Unable to insert/update PriceInfo for CMB/" + item.Code + ".");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(MethodBase.GetCurrentMethod().Name + "(): " +
                                      "Fail when updating market price for CurrencyName : " + item.Name +
                                      ", Message : " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): " + "Fail with Message : " + ex.Message +
                              ", StackTrace : " + ex.StackTrace);
                }
            });
        }

        public bool UpdateERCPriceInfo(short currencyID, int cryptoID, decimal amount)
        {
            var allPriceInfo = PriceInfoDac.GetAll();
            var selectExistPriceInfo =
                allPriceInfo.SingleOrDefault(x => x.CurrencyID == currencyID && x.CryptoID == cryptoID);

            if (selectExistPriceInfo != null)
            {
                selectExistPriceInfo.Price = amount;
                selectExistPriceInfo.LastUpdateDate = DateTime.UtcNow;
                if (!PriceInfoDac.UpdatePriceInfo(selectExistPriceInfo))
                {
                    return false;
                }

                Log.Info(MethodBase.GetCurrentMethod().Name + "(): " + "Succcess updating PriceInfo for CrypoID : " +
                         cryptoID + " to CurrencyID : " + currencyID);
                return true;
            }

            selectExistPriceInfo = new PriceInfo
            {
                CryptoID = cryptoID,
                CurrencyID = currencyID,
                Price = amount,
                LastUpdateDate = DateTime.UtcNow
            };
            var result = PriceInfoDac.CreatePriceInfo(selectExistPriceInfo);
            if (result.ID > 0)
            {
                Log.Info(MethodBase.GetCurrentMethod().Name + "(): " + "Succcess creating PriceInfo for CrypoID : " +
                         cryptoID + " to CurrencyID : " + currencyID);
                return true;
            }

            return false;
        }

        public decimal ConvertCurrency(string fromCurrency, string toCurrency, decimal amount)
        {
            try
            {
                string jsonCurrencyParam = fromCurrency + "_" + toCurrency;
                string apiURL = "api/v6/convert?q=" + jsonCurrencyParam;

                HttpClient client = new HttpClient { BaseAddress = new Uri(CurrencyCoverterAPIURL) };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(apiURL).Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    var jsonResult = (decimal)JObject.Parse(jsonData)["results"][jsonCurrencyParam]["val"];
                    return jsonResult * amount;
                }
                else
                {
                    throw new CommonException(0,
                        "Convert currency failed, FromCurrency: " + fromCurrency + "ToCurrency: " + toCurrency);
                }
            }
            catch (Exception ex)
            {
                Log.Info(MethodBase.GetCurrentMethod().Name + "Message: " + ex.Message + "StackTrace: " +
                         ex.StackTrace);
                throw new CommonException(0, "Message: " + ex.Message + "StackTrace: " + ex.StackTrace);

            }
        }

        public decimal GetCMBToUSDPriceInfo()
        {
            decimal result = 0;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(CMBDexPriceInfoURL).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    //data = data.Replace(colName, "price_curr");

                    CMBCurrencyPrice cmbApi = JsonConvert.DeserializeObject<CMBCurrencyPrice>(data);

                    if (cmbApi.StatusCode == 200 && string.Compare(cmbApi.Error, "false") == 0)
                    {
                        //var result = priceList.Markets.Where(x => x.ContainsKey("usd_price")).FirstOrDefault();

                        //if (result != null)
                        //{
                        //    //return Convert.ToDecimal(result["usd_price"]);
                        //    resultToreturn = Convert.ToDecimal(result["usd_price"]);
                        //}
                        //else
                        //{
                        //    Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + "(): " + "Response does not contain USD price info for CMB.");
                        //    //throw new BaseException(CodeConst.COMMON_ERROR, "Error: Response does not contain USD price info for CMB.");
                        //}
                        decimal? apiReturnRate = null;

                        apiReturnRate = cmbApi.Data.Rates.USD;
                        if (apiReturnRate.HasValue && apiReturnRate > 0)
                        {
                            result = apiReturnRate.Value;
                        }
                        else
                        {
                            Log.Error(MethodBase.GetCurrentMethod().Name + "(): " +
                                      "CMB API response does not contain USD price info or USD rate <= 0.");
                            throw new Exception(
                                "Error: CMB API response does not contain USD price info or USD rate <= 0.");
                        }
                    }
                    else
                    {
                        Log.Error(MethodBase.GetCurrentMethod().Name + "(): " +
                                  "Response failed when requesting price info for CMB.");
                        //throw new Exception(CodeConst.COMMON_ERROR, "Error: Response failed when requesting price info for CMB.");
                    }
                }
                else
                {
                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): " + "Unable to connect to CMB Dex");
                    //throw new Exception(CodeConst.COMMON_ERROR, "Error: Cannot connect to CMB Dex.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().Name + "(): " +
                          "Something unexpected happen when getting CMB Price.");
                //throw new Exception(CodeConst.COMMON_ERROR, "Error: Something unexpected happen when getting CMB Price.");
            }

            return result;
        }

        public List<CurrencyConverterModel> CurrencyConvertList()
        {
            var returnList = new List<CurrencyConverterModel>();

            foreach (var item in currenciesDAC.GetAll())
            {
                try
                {
                    var currPriceList = ConvertMultipleCurrency("USD", item.Code);
                    
                    if (currPriceList != null)
                    {
                        if (_fiatExchaneRate.ContainsKey(item.Code))
                        {
                            _fiatExchaneRate[item.Code] = currPriceList.Value;
                        }
                        else
                        {
                            _fiatExchaneRate.Add(item.Code, currPriceList.Value);
                        }

                        returnList.Add(new CurrencyConverterModel
                        {
                            CurrencyPairFrom = currPriceList.CurrencyPairFrom,
                            CurrencyPairTo = item.Code,
                            Value = currPriceList.Value
                        });
                    }
                    else
                    {
                        if (_fiatExchaneRate.ContainsKey(item.Code))
                        {
                            returnList.Add(new CurrencyConverterModel
                            {
                                CurrencyPairFrom = "USD",
                                CurrencyPairTo = item.Code,
                                Value = _fiatExchaneRate[item.Code]
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): Fail to get converted currency, Message : " +
                              ex.Message);

                    if (_fiatExchaneRate.ContainsKey(item.Code))
                    {
                        returnList.Add(new CurrencyConverterModel
                        {
                            CurrencyPairFrom = "USD",
                            CurrencyPairTo = item.Code,
                            Value = _fiatExchaneRate[item.Code]
                        });
                    }
                }
            }

            return returnList;
        }

        public CurrencyConverterModel ConvertMultipleCurrency(string fromCurrency, string toCurrency1)
        {
            try
            {
                //CurrencyConverterModel dataList = new CurrencyConverterModel();
                //string currencyPairTwo = "";
                string jsonCurrencyParam = fromCurrency + "_" + toCurrency1;
                string apiURL = "api/v6/convert?q=" + jsonCurrencyParam;

                //if (!string.IsNullOrWhiteSpace(toCurrency2))
                //{
                //    currencyPairTwo = fromCurrency + "_" + toCurrency2;
                //    apiURL = apiURL + "," + currencyPairTwo;
                //}              

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(CurrencyCoverterAPIURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(apiURL).Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;

                    // First currency pair to convert
                    var jsonResult = (decimal)JObject.Parse(jsonData)["results"][jsonCurrencyParam]["val"];

                    CurrencyConverterModel data = new CurrencyConverterModel
                    {
                        CurrencyPairFrom = fromCurrency,
                        CurrencyPairTo = toCurrency1,
                        Value = jsonResult
                    };

                    //dataList.Add(data);

                    // Second currency pair to convert
                    //if (!string.IsNullOrWhiteSpace(toCurrency2))
                    //{
                    //    var jsonResult2 = (decimal)JObject.Parse(jsonData)["results"][currencyPairTwo]["val"];

                    //    CurrencyConverterModel data2 = new CurrencyConverterModel
                    //    {
                    //        CurrencyPairFrom = fromCurrency,
                    //        CurrencyPairTo = toCurrency2,
                    //        Value = jsonResult2
                    //    };

                    //    dataList.Add(data2);
                    //}

                    return data;
                }

                throw new Exception("Convert currency failed, FromCurrency: " + fromCurrency + ", ToCurrency: " + toCurrency1);
            }
            catch (Exception ex)
            {
                Log.Info(MethodBase.GetCurrentMethod().Name + "Message: " + ex.Message + "StackTrace: " + ex.StackTrace);
                throw new Exception("Message: " + ex.Message + "StackTrace: " + ex.StackTrace);

            }
        }

        public void GetFiiiCoinInfo(decimal btcToUSD, List<CurrencyConverterModel> currencyconvertermodel)
        {
            try
            {
                var cryptoCurr2 = cryptoCurrDAC.GetByCode("FIII");
                if (cryptoCurr2 == null)
                {
                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): FIII is not found in database.");
                    throw new Exception("Error: FIII is not found in database");
                }

                if ((from x in currenciesDAC.GetAll()
                     where string.Compare(x.Code, "USD", StringComparison.OrdinalIgnoreCase) == 0
                     select x).FirstOrDefault() != null)
                {
                    decimal fiiiBTCPrice = GetFiiiToBTCPriceInfo();
                    if (fiiiBTCPrice != decimal.Zero)
                    {
                        decimal fiiiUSD = decimal.Multiply(fiiiBTCPrice, btcToUSD);
                        List<Currencies> currencylist = currenciesDAC.GetAll();
                        if (currencylist.Count == 0)
                        {
                            throw new Exception("Error: Currency table does not contains any data.");
                        }

                        foreach (Currencies item in currencylist)
                        {
                            CurrencyConverterModel usdtocurrencyrate = (from x in currencyconvertermodel
                                                                        where string.Compare(x.CurrencyPairTo, item.Code, true) == 0
                                                                        select x).FirstOrDefault();
                            if (usdtocurrencyrate != null)
                            {
                                if (!UpdateERCPriceInfo(item.ID, cryptoCurr2.Id,
                                    Math.Round(usdtocurrencyrate.Value * fiiiUSD, 2)))
                                {
                                    Log.Error(MethodBase.GetCurrentMethod().Name +
                                              "(): Error: Unable to insert/update PriceInfo for FIII/" + item.Code +
                                              ".");
                                }
                            }
                            else
                            {
                                Log.Error(MethodBase.GetCurrentMethod().Name + "(): Unable to update.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().Name + "(): Fail with message: " + ex.Message +
                          ", StackTrace: " + ex.StackTrace);
            }
        }

        public decimal GetFiiiToBTCPriceInfo()
        {
            string apiURL = string.Format(FiiiCoinPriceInfoURL, Array.Empty<object>());
            decimal dataReturn = default(decimal);
            try
            {
                var response = new HttpClient
                {
                    DefaultRequestHeaders =
                    {
                        Accept =
                        {
                            new MediaTypeWithQualityHeaderValue("application/json")
                        }
                    }
                }.GetAsync(apiURL).Result;
                if (!response.IsSuccessStatusCode)
                {
                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): Unable to connect to CoinEgg.");
                    return dataReturn;
                }

                string data = response.Content.ReadAsStringAsync().Result;
                FiiiCoin_CoinEggPriceInfoModel priceList =
                    JsonConvert.DeserializeObject<FiiiCoin_CoinEggPriceInfoModel>(data);
                if (string.IsNullOrEmpty(priceList.Last))
                {
                    Log.Error(MethodBase.GetCurrentMethod().Name +
                              "(): Response does not contain Fiii/BTC price info. Response data: " + data);
                    return dataReturn;
                }

                dataReturn = Convert.ToDecimal(priceList.Last);
                return dataReturn;
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().Name +
                          "() Error: Something unexpected happen when getting Fiii/BTC price. Error message - " +
                          ex.Message + "StackTrace: " + ex.StackTrace);
                return dataReturn;
            }
        }

        public void GetWinCoinInfo(decimal btcToUSD, List<CurrencyConverterModel> currencyconvertermodel)
        {
            try
            {
                var cryptoCurr = cryptoCurrDAC.GetByCode("WC");
                if (cryptoCurr == null)
                {
                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): WC is not found in database.");
                    //throw new Exception("Error: WC is not found in database");
                    return;
                }

                // Check whether db contains USD
                //var currUSD = currenciesDAC.GetAll().Where(x => string.Compare(x.Code, "USD", true) == 0).FirstOrDefault();
                var currUSD = currenciesDAC.GetByCode("USD");

                if (currUSD != null)
                {
                    // Get WC/BTC
                    decimal WCBTCPrice = GetWCToBTCPriceInfo();

                    // Convert from BTC to USD, if 0, it means that Winbitex's API return null or error for WC/BTC
                    if (WCBTCPrice != 0)
                    {
                        decimal WCUSD = decimal.Multiply(WCBTCPrice, btcToUSD);

                        /// TODO - update Wincoin for other currencies in db
                        List<Currencies> currencylist = currenciesDAC.GetAll();

                        if (currencylist.Count != 0)
                        {
                            foreach (var currency in currencylist)
                            {
                                var usdtocurrencyrate = currencyconvertermodel
                                    .Where(x => string.Compare(x.CurrencyPairTo, currency.Code, true) == 0)
                                    .FirstOrDefault();

                                if (usdtocurrencyrate != null)
                                {
                                    if (!UpdateERCPriceInfo(currency.ID, cryptoCurr.Id,
                                        Math.Round(usdtocurrencyrate.Value * WCUSD, 2)))
                                    {
                                        Log.Error(MethodBase.GetCurrentMethod().Name +
                                                  "(): Error: Unable to insert/update PriceInfo for WC/" +
                                                  currency.Code + ".");
                                    }
                                }
                                else
                                {
                                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): Unable to update.");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Error: Currency table does not contains any data.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().Name + "(): Fail with message: " + ex.Message +
                          ", StackTrace: " + ex.StackTrace);
            }
        }

        public decimal GetWCToBTCPriceInfo()
        {
            string apiURL = string.Format(WinCoinPriceInfoURL);
            decimal dataReturn = 0;

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(apiURL).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    if (string.Compare((string)JObject.Parse(data)["success"], "true", true) == 0)
                    {
                        decimal Last = (decimal)JObject.Parse(data)["data"]["last"];
                        if (Last > 0)
                        {
                            dataReturn = Last;
                        }
                        else
                        {
                            Log.Error(MethodBase.GetCurrentMethod().Name + "(): " +
                                      "Response does not contain WC/BTC price info. Response data: " + data);
                        }
                    }
                    else
                    {
                        Log.Error(MethodBase.GetCurrentMethod().Name + "(): " +
                                  "Response does not contain WC/BTC price info. Response data: " + data);
                    }
                }
                else
                {
                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): Unable to connect to Winbitex.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().Name +
                          "() Error: Something unexpected happen when getting WC/BTC price. Error message - " +
                          ex.Message + "StackTrace: " + ex.StackTrace);
            }

            return dataReturn;
        }

        public void UpdateMarketPriceFrmCoinMarketCapToDB(short currencyId)
        {
            // Get all Cryptocurrencies
            List<Cryptocurrency> crypto = new List<Cryptocurrency>();
            //List<PriceInfo> priceinfolist = new List<PriceInfo>();

            //Janet change get cryptocurrencies list without transaction setting
            //crypto = cryptoCurrDAC.GetAllCurrencies();
            crypto = cryptoCurrDAC.List();

            Currencies fiatcurrencyitem = new Currencies();
            fiatcurrencyitem = currenciesDAC.GetByID(currencyId);

            //priceinfolist = priceInfoDAC.GetAll();

            string apiURL = string.Format(CoinMarketCapV2_URL, fiatcurrencyitem.Code.ToUpper());

            //string colName = "price_" + fiatcurrencyitem.Code.ToLower();  // "price_eur"
            //string colName = currency.ToUpper();  // "EUR"

            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //HttpResponseMessage response = client.GetAsync(apiURL).Result;

            var data = RestUtilities.GetJson(apiURL);
            //data = data.Replace(colName, "price_curr");

            CoinMarketCapPriceDetailV2 priceList = JsonConvert.DeserializeObject<CoinMarketCapPriceDetailV2>(data);
            //BittrexModels priceList = JsonConvert.DeserializeObject<BittrexModels>(data);
            //List<Welcome> priceList = JsonConvert.DeserializeObject<List<Welcome>>(data);

            var query = (from cryptoitem in crypto
                         join priceitem in priceList.Data on cryptoitem.Code equals priceitem.Value.Symbol
                         where priceitem.Value.Quotes != null
                         where priceitem.Value.Quotes.Any(x => string.Compare(x.Key, fiatcurrencyitem.Code, true) == 0)
                         select new PriceInfo
                         {
                             CryptoID = cryptoitem.Id,
                             CurrencyID = fiatcurrencyitem.ID,
                             //Price = priceitem.Value.Quotes.Select(x => Convert.ToDecimal(x.Value.Price.Value)).First()
                             Price = (decimal)priceitem.Value.Quotes
                                 .Where(x => string.Compare(x.Key, fiatcurrencyitem.Code, true) == 0).Select(x => x)
                                 .FirstOrDefault().Value.Price
                         }).ToList();

            if (query.Count > 0)
            {
                foreach (var item in query)
                {
                    var priceinfolist = PriceInfoDac.GetAll();
                    var existpriceinfo = priceinfolist.SingleOrDefault(x =>
                        x.CryptoID == item.CryptoID && x.CurrencyID == item.CurrencyID);

                    if (existpriceinfo != null)
                    {
                        existpriceinfo.Price = item.Price;
                        existpriceinfo.LastUpdateDate = DateTime.UtcNow;
                        PriceInfoDac.UpdatePriceInfo(existpriceinfo);

                        Log.Info(MethodBase.GetCurrentMethod().Name + "(): " +
                                 "Succcess updating PriceInfo for CrypoID : " + existpriceinfo.CryptoID +
                                 " to CurrencyID : " + existpriceinfo.CurrencyID);
                    }
                    else
                    {
                        item.LastUpdateDate = DateTime.UtcNow;
                        PriceInfoDac.CreatePriceInfo(item);

                        Log.Info(MethodBase.GetCurrentMethod().Name + "(): " +
                                 "Succcess creating PriceInfo for CrypoID : " + item.CryptoID + " to CurrencyID : " +
                                 item.CurrencyID);
                    }

                    var cryptoCurr2 = (from x in crypto
                                       where string.Compare(x.Code, "BTC", StringComparison.OrdinalIgnoreCase) == 0
                                       select x).SingleOrDefault();
                    var fiatCurr2 = currenciesDAC.GetByCode("USD");
                    if (cryptoCurr2 != null && fiatCurr2 != null && item.CryptoID == cryptoCurr2.Id &&
                        item.CurrencyID == fiatCurr2.ID)
                    {
                        BTCToUSD = item.Price;
                    }
                }
            }
        }

        public void UpdateMarketPriceFrmBittrexToDB(short currenciesid, List<CurrencyConverterModel> currencyconverterlist)
        {
            List<Cryptocurrency> crypto = new List<Cryptocurrency>();
            crypto = cryptoCurrDAC.GetAllActived();

            Currencies fiatcurrencyitem = new Currencies();
            fiatcurrencyitem = currenciesDAC.GetByID(currenciesid);

            foreach (var item in crypto)
            {
                if (_exceptive.Contains(item.Code)) continue;

                string apiURL = string.Format(Bittrex_URL + item.Code);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync(apiURL).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    if (data != null)
                    {
                        BittrexModels BittrexResult = JsonConvert.DeserializeObject<BittrexModels>(data);

                        if (BittrexResult.success == true)
                        {
                            PriceInfo NewPriceInfo = new PriceInfo();
                            NewPriceInfo.CryptoID = item.Id;
                            NewPriceInfo.LastUpdateDate = DateTime.UtcNow;

                            //getCurrencyCode
                            //string currencyCode = currenciesDAC.GetByID(currenciesid).Code;
                            var currencyCode = fiatcurrencyitem.Code;

                            if (string.Compare(currencyCode, "USD", true) == 0)
                            {
                                NewPriceInfo.CurrencyID = currenciesid;
                                NewPriceInfo.Price = Convert.ToDecimal(BittrexResult.result.Last);
                            }
                            else
                            {
                                //ApiQuotes fiatQuotesList = new ApiQuotes();
                                //string exchangeRateURL = string.Format(FiatCurrenciesExchangeRateURL1);

                                //HttpResponseMessage ExchangeResponse = client.GetAsync(exchangeRateURL).Result;

                                //if (ExchangeResponse.IsSuccessStatusCode)
                                //{
                                //    string ExchangeData = ExchangeResponse.Content.ReadAsStringAsync().Result;
                                //    fiatQuotesList = JsonConvert.DeserializeObject<ApiQuotes>(ExchangeData);
                                //}

                                //string code = "USD" + currencyCode.ToUpper();

                                //NewPriceInfo.CurrencyID = currenciesid;
                                //var keey = fiatQuotesList.Quotes.First(y => y.Key == code);

                                //var rate = keey.Value;
                                //NewPriceInfo.Price = Convert.ToDecimal(BittrexResult.result.Last) * Convert.ToDecimal(rate);

                                //string code = "USD" + (currencyCode).ToUpper();
                                //PriceInfoComponent PriceInfoComponent = new PriceInfoComponent();

                                try
                                {
                                    //var currencyitemprice = utilitycomponent.ConvertCurrency(USDcurrenciesitem.Code, item.Code, CMBtoUSDPrice);
                                    var currencyitemprice = currencyconverterlist.SingleOrDefault(x => string.Compare(x.CurrencyPairTo, currencyCode, StringComparison.OrdinalIgnoreCase) == 0);
                                    if (currencyitemprice != null)
                                    {
                                        NewPriceInfo.CurrencyID = currenciesid;
                                        NewPriceInfo.Price = currencyitemprice.Value * Convert.ToDecimal(BittrexResult.result.Last);
                                    }
                                    else
                                    {
                                        Log.Error(MethodBase.GetCurrentMethod().Name + "(): " + "Fail when getting market price for CurrencyName : " + currencyCode + ", Message : currencyitemprice is null");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): " + "Fail when getting market price for CurrencyName : " + currencyCode + ", Message : " + ex.Message);
                                }
                            }

                            //List<PriceInfo> priceinfolist = new List<PriceInfo>();
                            //PriceInfo existpriceinfo = new PriceInfo();

                            //priceinfolist = _priceInfoDAC.GetAll();
                            //existpriceinfo = priceinfolist.SingleOrDefault(x => x.CryptoID == item.Id && x.CurrencyID == currenciesid);
                            var existpriceinfo = _priceInfoDAC.GetPriceInfo(currenciesid, item.Id);
                            // ============================= Get BTC to USD - Added by yee may 31-Oct-2018 ===========================
                            //Cryptocurrency cryptoCurr = new Cryptocurrency();
                            //Currencies fiatCurr = new Currencies();

                            //cryptoCurr = crypto.SingleOrDefault(x => string.Compare(x.Name, "BTC", true) == 0);
                            //fiatCurr = currenciesDAC.GetByCode("USD");

                            //if (cryptoCurr != null && fiatCurr != null)
                            //{
                            //    //if (query.Where(x => x.CryptoID == cryptoCurr.ID && x.CurrencyID == fiatCurr.ID) != null)
                            //    if (item.Id == cryptoCurr.Id && currenciesid == fiatCurr.ID)
                            //    {
                            //        BTCToUSD = Convert.ToDecimal(BittrexResult.result.Last);
                            //    }
                            //}
                            // =======================================================================================================

                            if (item.Code == "BTC" && BTCToUSD == decimal.Zero)
                            {
                                BTCToUSD = Convert.ToDecimal(BittrexResult.result.Last);
                            }

                            if (existpriceinfo != null)
                            {
                                //existpriceinfo.Price = Convert.ToDecimal(BittrexResult.result.Last);
                                existpriceinfo.Price = NewPriceInfo.Price;
                                existpriceinfo.LastUpdateDate = DateTime.UtcNow;
                                _priceInfoDAC.UpdatePriceInfo(existpriceinfo);

                                Log.Info(MethodBase.GetCurrentMethod().Name + "(): " +
                                         "Succcess updating PriceInfo for CrypoID : " + existpriceinfo.CryptoID +
                                         " to CurrencyID : " + existpriceinfo.CurrencyID);
                            }
                            else
                            {
                                _priceInfoDAC.CreatePriceInfo(NewPriceInfo);

                                Log.Info(MethodBase.GetCurrentMethod().Name + "(): " +
                                         "Succcess creating PriceInfo for CrypoID : " + item.Id + " to CurrencyID : " +
                                         currenciesid);
                            }
                        }
                    }
                    else
                    {
                        Log.Error(MethodBase.GetCurrentMethod().Name + "(): " + "CurrencyID : " +
                                  currenciesid + " not available in CoinMarketCap ");
                        throw new Exception("Invalid Currency");
                    }
                }
                else
                {
                    Log.Error(MethodBase.GetCurrentMethod().Name + "(): " +
                              "Unable to connect to CoinMarketCap");
                    throw new Exception("Error: Cannot connect to CoinMarketCap.");
                }
            }
        }
        
        public decimal GetPrice(string currency, string crypto)
        {
            return new PriceInfoDAC().GetPriceByName(currency, crypto);
        }
    }

    public class CurrencyConverterModel
    {
        public string CurrencyPairFrom
        {
            get;
            set;
        }

        public string CurrencyPairTo
        {
            get;
            set;
        }

        public decimal Value
        {
            get;
            set;
        }
    }

    public class BittrexPrice
    {
        [JsonProperty("Bid")]
        public double Bid { get; set; }

        [JsonProperty("Ask")]
        public double Ask { get; set; }

        [JsonProperty("Last")]
        public double Last { get; set; }
    }

    public class BittrexModels
    {
        [JsonProperty("Success")]
        public bool success { get; set; }

        [JsonProperty("Message")]
        public string message { get; set; }

        [JsonProperty("Result")]
        public BittrexPrice result { get; set; }
    }

    public class ApiQuotes
    {
        [JsonProperty("quotes")]
        public Dictionary<string, decimal> Quotes { get; set; }
    }


    public partial class CoinMarketCapPriceDetailV2
    {
        [JsonProperty("data")]
        public Dictionary<string, Data> Data { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }

    public class Data
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("website_slug")]
        public string WebsiteSlug { get; set; }

        [JsonProperty("rank")]
        public int? Rank { get; set; }

        [JsonProperty("circulating_supply")]
        public decimal? CirculatingSupply { get; set; }

        [JsonProperty("total_supply")]
        public decimal? TotalSupply { get; set; }

        [JsonProperty("max_supply")]
        public decimal? MaxSupply { get; set; }

        [JsonProperty("quotes")]
        public Dictionary<string, Quotes> Quotes { get; set; }

        [JsonProperty("last_updated")]
        public string Last_updated { get; set; }
    }

    public partial class Quotes
    {
        [JsonProperty("price")]
        public double? Price { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("timestamp")]
        public long? Timestamp { get; set; }

        [JsonProperty("num_cryptocurrencies")]
        public long? NumCryptocurrencies { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }
    }

    public class CMBCurrencyPrice
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public CMBPriceData Data { get; set; }
    }

    public class CMBPriceData
    {
        [JsonProperty("base_currency")]
        public string BaseCurrency { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("rates")]
        public Rates Rates { get; set; }
    }

    public class Rates
    {
        [JsonProperty("USD")]
        public decimal USD { get; set; }
    }

    public class FiiiCoin_CoinEggPriceInfoModel
    {
        [JsonProperty("high")]
        public string High
        {
            get;
            set;
        }

        [JsonProperty("low")]
        public string Low
        {
            get;
            set;
        }

        [JsonProperty("buy")]
        public string Buy
        {
            get;
            set;
        }

        [JsonProperty("sell")]
        public string Sell
        {
            get;
            set;
        }

        [JsonProperty("last")]
        public string Last
        {
            get;
            set;
        }

        [JsonProperty("vol")]
        public string Vol
        {
            get;
            set;
        }
    }

}
