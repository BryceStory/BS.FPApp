using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using FiiiPay.Data;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Foundation.Entities.Enum;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Queue;
using FiiiPOS.Business.Properties;
using FiiiPOS.DTO;
using log4net;
using Newtonsoft.Json;
using TransactionStatus = FiiiPay.Entities.Enums.TransactionStatus;

namespace FiiiPOS.Business.FiiiPOS
{
    public class MerchantWalletComponent
    {
        private const string FIIICOIN_CODE = "FIII";
        private const short FIIICOIN_SEQUENCE = -1;
        private const short WHITECOIN_SEQUENCE = -2;

        public List<CryptocurrencyDTO> GetMerchentCryptoList(Guid accountId)
        {
            var account = new MerchantAccountDAC().GetById(accountId);
            var pos = new POSDAC().GetById(account.POSId.Value);
            var ccDac = new CryptocurrencyDAC();
            var list = ccDac.GetAllActived();
            if (!pos.IsWhiteLabel)
            {
                list.RemoveAll(t => t.IsWhiteLabel == 1);
            }

            return list.Select(e => new CryptocurrencyDTO
            {
                Id = e.Id,
                Code = e.Code,
                Status = e.Status,
                IconURL = e.IconURL,
                Name = e.Name,
                CryptoEnable = e.Enable
            }).ToList();
        }

        public List<MerchantSupportReceiptWalletDTO> SupportReceiptList(Guid accountId)
        {
            var account = new MerchantAccountDAC().GetById(accountId);
            var dac = new MerchantWalletDAC();

            var cryptoList = new CryptocurrencyDAC().GetAllActived();

            var fiiiCrypto = cryptoList.First(w => w.Code == FIIICOIN_CODE);
            var fiiiWallet = dac.GetByAccountId(account.Id, fiiiCrypto.Id) ?? GenerateWallet(accountId, fiiiCrypto.Id, fiiiCrypto.Code, FIIICOIN_SEQUENCE, true);
            if (!fiiiWallet.SupportReceipt || fiiiWallet.Sequence != FIIICOIN_SEQUENCE)
            {
                dac.Support(account.Id, fiiiWallet.CryptoId, FIIICOIN_SEQUENCE);
            }

            List<MerchantWallet> supportReceiptList;
            var pos = new POSDAC().GetById(account.POSId.Value);
            if (pos.IsWhiteLabel)
            {
                var whiteCrypto = cryptoList.First(w => w.Code == pos.FirstCrypto);
                var whiteWallet = dac.GetByAccountId(account.Id, whiteCrypto.Id);
                if (whiteWallet == null)
                {
                    GenerateWallet(accountId, whiteCrypto.Id, whiteCrypto.Code, WHITECOIN_SEQUENCE, true);
                }
                else if (!whiteWallet.SupportReceipt || whiteWallet.Sequence != WHITECOIN_SEQUENCE)
                {
                    dac.Support(account.Id, whiteWallet.CryptoId, WHITECOIN_SEQUENCE);
                }

                supportReceiptList = dac.SupportReceiptList(accountId).OrderBy(e => e.Sequence).ToList();
            }
            else
            {
                var whiteCryptoList = cryptoList.Where(e => e.IsWhiteLabel == 1).ToList();
                supportReceiptList = dac.SupportReceiptList(accountId).OrderBy(e => e.Sequence).ToList();
                supportReceiptList.RemoveAll(e => whiteCryptoList.Exists(a => a.Id == e.CryptoId));
            }

            var marketPriceList = new PriceInfoDAC().GetPrice(account.FiatCurrency);
            return supportReceiptList.Select(e =>
            {
                var crypto = cryptoList.FirstOrDefault(w => w.Id == e.CryptoId);
                if (crypto == null)
                {
                    return null;
                }
                return new MerchantSupportReceiptWalletDTO
                {
                    WalletId = e.Id,
                    CryptoId = crypto.Id,
                    CryptoStatus = crypto.Status,
                    CryptoCode = crypto.Code,
                    CryptoName = crypto.Name,
                    IconURL = crypto.IconURL,
                    DecimalPlace = crypto.DecimalPlace,
                    Markup = account.Markup,
                    MarketPrice = marketPriceList.FirstOrDefault(m => crypto.Code == m.CryptoName)?.Price.ToString(4),
                    Balance = string.Equals("FIII", crypto.Code, StringComparison.CurrentCultureIgnoreCase) ? e.Balance : 0,
                    IsDefault = e.CryptoId == fiiiCrypto.Id || (pos.IsWhiteLabel && crypto.Code == pos.FirstCrypto) ? 1 : 0,
                    CryptoEnable = crypto.Enable
                };
            }).Where(w => w != null).ToList();
        }

        public MerchantSupportReceiptWalletDTO GetSupportReceiptByFiatCurrency(Guid accountId, string fiatCurrency, int coinId)
        {
            var account = new MerchantAccountDAC().GetById(accountId);
            var crypto = new CryptocurrencyDAC().GetById(coinId);

            var marketPriceComponent = new MarketPriceComponent();
            var marketPrice = marketPriceComponent.GetMarketPrice(fiatCurrency, crypto.Code);

            var supportReceiptWallets = new MerchantWalletDAC().SupportReceiptList(accountId);
            var singleSupportWallet = supportReceiptWallets.FirstOrDefault(e => e.CryptoId == crypto.Id);
            if (singleSupportWallet == null)
                return new MerchantSupportReceiptWalletDTO();

            return new MerchantSupportReceiptWalletDTO
            {
                WalletId = singleSupportWallet.Id,
                CryptoId = crypto.Id,
                CryptoStatus = crypto.Status,
                CryptoCode = crypto.Code,
                CryptoName = crypto.Name,
                IconURL = crypto.IconURL,
                DecimalPlace = crypto.DecimalPlace,
                Markup = account.Markup,
                MarketPrice = marketPrice?.Price.ToString("F"),
                CryptoEnable = crypto.Enable
            };
        }

        public void SettingCrytocurrencies(Guid accountId, List<int> cryptoIds)
        {
            var mwDac = new MerchantWalletDAC();
            var mwList = mwDac.GetByAccountId(accountId).ToList();
            var cryptoList = new CryptocurrencyDAC().GetByIds(cryptoIds.ToArray());

            // FIII 不参与排序
            var fiiiCrypto = cryptoList.FirstOrDefault(e => e.Code == FIIICOIN_CODE);
            if (fiiiCrypto != null)
            {
                cryptoIds.Remove(fiiiCrypto.Id);
                var fiiiWallet = mwList.FirstOrDefault(e => e.CryptoId == fiiiCrypto.Id);
                if (fiiiWallet != null)
                    mwList.Remove(fiiiWallet);
            }

            // 白标POS机的白标币不参与排序
            var account = new MerchantAccountDAC().GetById(accountId);
            var pos = new POSDAC().GetById(account.POSId.Value);
            if (pos.IsWhiteLabel)
            {
                var whiteCrypto = cryptoList.FirstOrDefault(e => e.Code == pos.FirstCrypto);
                if (whiteCrypto != null)
                {
                    cryptoIds.Remove(whiteCrypto.Id);
                    var whiteWallet = mwList.FirstOrDefault(e => e.CryptoId == whiteCrypto.Id);
                    if (whiteWallet != null)
                        mwList.Remove(whiteWallet);
                }
            }


            for (int i = 0; i < cryptoIds.Count; i++)
            {
                int seq = i + 1;
                var crypto = cryptoList.FirstOrDefault(e => e.Id == cryptoIds[i]);
                if (crypto == null)
                    continue;

                var wallet = mwList.FirstOrDefault(e => e.CryptoId == cryptoIds[i]);
                // 新增
                if (wallet == null)
                {
                    GenerateWallet(accountId, crypto.Id, crypto.Code, seq, true);
                    continue;
                }
                // 启用
                if (!wallet.SupportReceipt || wallet.Sequence != seq)
                {
                    mwDac.Support(accountId, crypto.Id, seq);
                }
                mwList.Remove(wallet);
            }

            mwList.RemoveAll(e => !e.SupportReceipt);
            // 禁用
            foreach (var wallet in mwList)
            {
                mwDac.Reject(accountId, wallet.CryptoId);
            }
        }

        public MerchantTotalAssetsDTO GetMerchantTotalAssets(Guid accountId)
        {
            var account = new MerchantAccountDAC().GetById(accountId);
            var walletList = new MerchantWalletDAC().GetByAccountId(accountId);

            decimal totalPrice = 0m;

            if (walletList.Count > 0)
            {
                var cryptoList = new CryptocurrencyDAC().GetAllActived();
                if (account.POSId.HasValue)
                {
                    var pos = new POSDAC().GetById(account.POSId.Value);
                    if (!pos.IsWhiteLabel)
                        cryptoList.RemoveAll(t => t.IsWhiteLabel == 1);
                }

                var marketPriceList = new MarketPriceComponent().GetMarketPrice(account.FiatCurrency);

                totalPrice = walletList.Sum(e =>
                {
                    var coin = cryptoList.FirstOrDefault(c => c.Id == e.CryptoId);
                    var marketPrice = marketPriceList.FirstOrDefault(m => m.CryptoName == coin?.Code);

                    return (e.Balance + e.FrozenBalance) * (marketPrice?.Price ?? 0);
                });
            }

            return new MerchantTotalAssetsDTO
            {
                FiatCurrency = account.FiatCurrency,
                Amount = totalPrice.ToString("N", CultureInfo.InvariantCulture),
                IsAllowWithDrawal = account.IsAllowWithdrawal
            };
        }

        public MerchantWalletDTO GetWalletInfoById(Guid accountId, int cryptoId)
        {
            var crypto = new CryptocurrencyDAC().GetById(cryptoId);
            var wallet = new MerchantWalletDAC().GetByAccountId(accountId, cryptoId);
            var merchant = new MerchantAccountDAC().GetById(accountId);
            var currencyId = new CurrenciesDAC().GetByCode(merchant.FiatCurrency).ID;
            var exchangeRate = new PriceInfoDAC().GetByCurrencyId(currencyId).ToDictionary(item => item.CryptoID);
            return new MerchantWalletDTO
            {
                WalletId = wallet?.Id ?? 0,
                CryptoId = crypto.Id,
                CryptoCode = crypto.Code,
                CryptoName = crypto.Name,
                IconURL = crypto.IconURL,
                DecimalPlace = crypto.DecimalPlace,
                Balance = (wallet?.Balance ?? 0).ToString(crypto.DecimalPlace),
                FrozenBalance = (wallet?.FrozenBalance ?? 0).ToString(crypto.DecimalPlace),
                FiatExchangeRate = (exchangeRate.ContainsKey(crypto.Id) ? exchangeRate[crypto.Id].Price : 0m).ToString(4),
                FiatBalance = (((wallet?.Balance ?? 0) + (wallet?.FrozenBalance ?? 0)) * (exchangeRate.ContainsKey(crypto.Id) ? exchangeRate[crypto.Id].Price : 0m)).ToString(4),
                FiatCode = merchant.FiatCurrency
            };
        }

        public List<MerchantWalletDTO> GetMerchantWalletList(Guid merchantAccountId)
        {
            var account = new MerchantAccountDAC().GetById(merchantAccountId);
            var cryptoList = new CryptocurrencyDAC().GetAllActived();

            cryptoList.MoveTop(t => t.Code == FIIICOIN_CODE);

            var pos = new POSDAC().GetById(account.POSId.Value);
            if (!pos.IsWhiteLabel)
            {
                cryptoList.RemoveAll(t => t.IsWhiteLabel == 1);
            }
            else
            {
                cryptoList.MoveTop(t => t.Code == pos.FirstCrypto);
            }

            var walletList = new MerchantWalletDAC().GetByAccountId(merchantAccountId);
            var currencyId = new CurrenciesDAC().GetByCode(account.FiatCurrency).ID;
            var exchangeRate = new PriceInfoDAC().GetByCurrencyId(currencyId).ToDictionary(item => item.CryptoID);
            return cryptoList.Select(e =>
            {
                var wallet = walletList.FirstOrDefault(w => w.CryptoId == e.Id);
                return new MerchantWalletDTO
                {
                    WalletId = wallet?.Id ?? 0,
                    CryptoId = e.Id,
                    CryptoStatus = e.Status,
                    CryptoCode = e.Code,
                    CryptoName = e.Name,
                    IconURL = e.IconURL,
                    DecimalPlace = e.DecimalPlace,
                    Balance = (wallet?.Balance ?? 0).ToString(e.DecimalPlace),
                    FrozenBalance = (wallet?.FrozenBalance ?? 0).ToString(e.DecimalPlace),
                    FiatExchangeRate = (exchangeRate.ContainsKey(e.Id) ? exchangeRate[e.Id].Price : 0m).ToString(4),
                    FiatBalance = (((wallet?.Balance ?? 0) + (wallet?.FrozenBalance ?? 0)) * (exchangeRate.ContainsKey(e.Id) ? exchangeRate[e.Id].Price : 0m)).ToString(4),
                    FiatCode = account.FiatCurrency,
                    CryptoEnable = e.Enable
                };
            }).ToList();
        }

        public DepositAddressInfo GetDepositAddressById(Guid accountId, int cryptoId)
        {
            var crypto = new CryptocurrencyDAC().GetById(cryptoId);
            if (crypto == null)
                throw new CommonException(ReasonCode.RECORD_NOT_EXIST, Resources.不支持的币种);

            if (!crypto.Status.HasFlag(CryptoStatus.Deposit))
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, Resources.CurrencyForbidden);

            if (crypto.Enable == (byte)CurrencyStatus.Forbidden)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, Resources.CurrencyForbidden);

            var dac = new MerchantWalletDAC();
            var wallet = dac.GetByAccountId(accountId, cryptoId) ?? GenerateWallet(accountId, cryptoId, crypto.Code);

            if (string.IsNullOrEmpty(wallet.Address))
            {
                var accountDAC = new MerchantAccountDAC();
                var account = accountDAC.GetById(accountId);

                var result = new FiiiFinanceAgent().CreateWallet(crypto.Code, account.Id, AccountTypeEnum.Merchant, account.Email, $"{account.PhoneCode}{account.Cellphone}");
                if (string.IsNullOrWhiteSpace(result.Address))
                    throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.FailedGenerateAddress);

                wallet.Address = result.Address;
                wallet.Tag = result.DestinationTag;

                dac.UploadAddress(wallet.Id, wallet.Address, wallet.Tag);
            }

            return new DepositAddressInfo { Address = wallet.Address, Tag = wallet.Tag, NeedTag = crypto.NeedTag };
        }

        public MerchantWallet GenerateWallet(Guid accountId, int cryptoId, string cryptoCode, int sequence = 10000, bool supportReceipt = false)
        {

            var merchantWallet = new MerchantWallet
            {
                MerchantAccountId = accountId,
                CryptoId = cryptoId,
                CryptoCode = cryptoCode,
                Status = MerchantWalletStatus.Displayed,
                Balance = 0,
                FrozenBalance = 0,
                SupportReceipt = supportReceipt,
                Sequence = sequence
            };
            merchantWallet = new MerchantWalletDAC().Insert(merchantWallet);

            return merchantWallet;
        }

        public void VerifyWithdrawPIN(Guid accountId, string pin)
        {
            MerchantAccount account = new MerchantAccountDAC().GetById(accountId);
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), account.PIN, pin);

            var model = new WithdrawVerify
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("MerchantWithdraw"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }

        public void VerifyWithdrawCombine(Guid accountId, string smsCode, string googleCode, string divisionCode)
        {
            MerchantAccount account = new MerchantAccountDAC().GetById(accountId);
            List<CombinedVerifyOption> options = new List<CombinedVerifyOption>
            {
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.Cellphone, Code=smsCode },
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.GooogleAuthenticator, Code=googleCode }
            };
            UserSecrets userSecrets = new UserSecrets
            {
                ValidationFlag = account.ValidationFlag,
                GoogleAuthSecretKey = account.AuthSecretKey
            };

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPOS, account.Id.ToString(), userSecrets, options, divisionCode);

            var model = SecurityVerify.GetModel<WithdrawVerify>(new CustomVerifier("MerchantWithdraw"), SystemPlatform.FiiiPOS, account.Id.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("MerchantWithdraw"), SystemPlatform.FiiiPOS, account.Id.ToString(), model);
        }

        public MerchantWithdrawal Withdrawal(Guid accountId, decimal amount, int cryptoId, string address, string tag, string clientIP)
        {
            SecurityVerify.Verify<WithdrawVerify>(new CustomVerifier("MerchantWithdraw"), SystemPlatform.FiiiPOS, accountId.ToString(), (model) =>
            {
                return model.PinVerified && model.CombinedVerified;
            });

            var cryptocurrency = new CryptocurrencyDAC().GetById(cryptoId);
            CryptoAddressValidation.ValidateAddress(cryptocurrency.Code, address);

            if (!string.IsNullOrEmpty(tag))
            {
                CryptoAddressValidation.ValidateTag(cryptocurrency.Code, tag);
            }

            var account = new MerchantAccountDAC().GetById(accountId);

            if (!new ProfileComponent().ValidateLv1(accountId))
                throw new CommonException(ReasonCode.NOT_VERIFY_LV1, Resources.需要Lv1认证才能使用相关功能);

            if (!account.IsAllowWithdrawal)
                throw new CommonException(ReasonCode.Not_Allow_Withdrawal, Resources.禁止提币);

            if (!cryptocurrency.Status.HasFlag(CryptoStatus.Withdrawal))
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, Resources.CurrencyForbidden);

            if (cryptocurrency.Enable == (byte)CurrencyStatus.Forbidden)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, Resources.CurrencyForbidden);

            var fromWallet = new MerchantWalletDAC().GetByAccountId(accountId, cryptoId);
            if (fromWallet == null)
                throw new CommonException(ReasonCode.Not_Allow_Withdrawal, Resources.禁止提币);

            if (fromWallet.Balance < amount)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.余额不足);

            var profileAgent = new MerchantProfileAgent();
            var profile = profileAgent.GetMerchantProfile(accountId);

            int level = profile.L2VerifyStatus == VerifyStatus.Certified ? 2 : profile.L1VerifyStatus == VerifyStatus.Certified ? 1 : 0;

            var masterSetting = GetMerchantWithdrawalMasterSettingWithCrypto(cryptocurrency, level);

            if (amount > masterSetting.PerTxLimit)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.不能高于单次提币量);
            var dac = new MerchantWithdrawalDAC();
            var today = DateTime.UtcNow.Date;
            decimal dailyWithdrawal = dac.DailyWithdrawal(accountId, cryptoId, today);
            if (amount > masterSetting.PerDayLimit - dailyWithdrawal)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.今日提币达到限额);
            decimal monthlyWithdrawal = dac.MonthlyWithdrawal(accountId, cryptoId, new DateTime(today.Year, today.Month, 1));
            if (amount > masterSetting.PerMonthLimit - monthlyWithdrawal)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.本月提币达到限额);

            var fromWithdraw = new MerchantWithdrawal
            {
                MerchantAccountId = accountId,
                MerchantWalletId = fromWallet.Id,
                Address = address,
                Tag = tag,
                Amount = amount,
                Status = TransactionStatus.UnSubmit,
                Timestamp = DateTime.UtcNow,
                OrderNo = NumberGenerator.GenerateUnixOrderNo(),
                CryptoId = fromWallet.CryptoId,
                CryptoCode = fromWallet.CryptoCode
            };

            var merchantWalletDac = new MerchantWalletDAC();
            var userWalletDac = new UserWalletDAC();

            //是否是商户地址
            var toMerchantWallet = merchantWalletDac.GetByAddress(address, cryptocurrency.NeedTag ? tag : null);
            if (toMerchantWallet != null)
            {
                //if (toMerchantWallet.CryptoId != cryptoId)
                //    throw new CommonException(10000, string.Format(Resources.提币地址不是有效的地址, cryptocurrency.Code));
                //if (toMerchantWallet.MerchantAccountId == accountId)
                //    throw new CommonException(ReasonCode.CANNOT_TRANSFER_TO_YOURSELF, Resources.提币地址不能是自己账户的地址);
                //return WithdrawalToMerchantAccount(fromWallet, fromWithdraw, toMerchantWallet);
                // 042018
                throw new CommonException(ReasonCode.CAN_NOT_WITHDRAW_TO_FiiiPOS, Resources.FiiiPOSCantWithdrawToFiiiPOS);
            }

            //是否是用户地址
            var toUserWallet = userWalletDac.GetByAddressAndCrypto(cryptoId, address, cryptocurrency.NeedTag ? tag : null);
            if (toUserWallet != null)
            {
                if (toUserWallet.CryptoId != cryptoId)
                {
                    throw new CommonException(ReasonCode.GENERAL_ERROR, FiiiPay.Framework.Component.Properties.GeneralResources.EMInvalidAddress);
                }

                if (amount < masterSetting.ToUserMinAmount)
                    throw new CommonException(10000, Resources.不能低于最低提币量);

                var fee = (fromWithdraw.Amount * masterSetting.ToUserHandleFeeTier).ToSpecificDecimal(cryptocurrency.DecimalPlace);
                if (amount <= fee)
                {
                    throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.到账数量不能为零或者负数);
                }

                return WithdrawalToUserAccount(fromWallet, fromWithdraw, toUserWallet, fee);
            }

            //平台内提币如果tag不对，创建一条失败记录
            if (cryptocurrency.NeedTag && (userWalletDac.IsUserWalletAddress(address) || merchantWalletDac.IsMerchantWalletAddress(address)))
            {
                return CancelWithdrawal(fromWithdraw);
            }

            //如果都不是，提币到场外
            if (amount < masterSetting.ToOutsideMinAmount)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.不能低于最低提币量);

            var baseFee = cryptocurrency.Withdrawal_Fee ?? 0;
            var tier = cryptocurrency.Withdrawal_Tier ?? 0;
            var fee1 = (amount * tier).ToSpecificDecimal(cryptocurrency.DecimalPlace);
            var totalFee = baseFee + fee1;

            if (amount <= totalFee)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.到账数量不能为零或者负数);

            return WithdrawalToOutside(fromWallet, fromWithdraw, cryptocurrency, account, amount, totalFee, address, tag, clientIP);
        }

        private MerchantWithdrawal CancelWithdrawal(MerchantWithdrawal fromWithdraw)
        {
            fromWithdraw.Status = TransactionStatus.Cancelled;
            fromWithdraw.TransactionId = Guid.NewGuid().ToString("N");
            fromWithdraw.SelfPlatform = true;//平台内提币

            using (var scope = new TransactionScope())
            {
                fromWithdraw = new MerchantWithdrawalDAC().Create(fromWithdraw);

                var fromWithdrawFee = new MerchantWithdrawalFee
                {
                    Amount = fromWithdraw.Amount,
                    Fee = 0,
                    Timestamp = DateTime.UtcNow,
                    WithdrawalId = fromWithdraw.Id
                };

                new MerchantWithdrawalFeeDAC().Create(fromWithdrawFee);

                scope.Complete();
            }

            MerchantMSMQ.PubMerchantWithdrawReject(fromWithdraw.Id, 0);

            return fromWithdraw;
        }

        //private MerchantWithdrawal WithdrawalToMerchantAccount(MerchantWallet fromWallet, MerchantWithdrawal fromWithdraw, MerchantWallet toWallet)
        //{
        //    MerchantDeposit deposit = null;
        //    using (var scope = new TransactionScope())
        //    {
        //        //提币
        //        fromWithdraw.Status = TransactionStatus.Confirmed;
        //        fromWithdraw.TransactionId = toWallet.MerchantAccountId.ToString("N");
        //        fromWithdraw.SelfPlatform = true;//平台内提币
        //        fromWithdraw = new MerchantWithdrawalDAC().Create(fromWithdraw);

        //        var fromWithdrawFee = new MerchantWithdrawalFee
        //        {
        //            Amount = fromWithdraw.Amount,
        //            Timestamp = DateTime.UtcNow,
        //            Fee = fromWithdraw.Amount * WithdrawalMasterSetting.ToMerchantHandleFeeTier,
        //            WithdrawalId = fromWithdraw.Id
        //        };
        //        new MerchantWithdrawalFeeDAC().Create(fromWithdrawFee);

        //        new MerchantWalletDAC().Decrease(fromWallet.Id, fromWithdraw.Amount);

        //        new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
        //        {
        //            WalletId = fromWallet.Id,
        //            Action = MerchantWalletStatementAction.Withdrawal,
        //            Amount = -fromWithdraw.Amount,
        //            Balance = fromWallet.Balance - fromWithdraw.Amount,
        //            Timestamp = DateTime.UtcNow
        //        });


        //        //充币
        //        var amount = fromWithdraw.Amount - fromWithdrawFee.Fee;
        //        if (amount <= 0)
        //        {
        //            throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.到账数量不能为零或者负数);
        //        }

        //        deposit = new MerchantDepositDAC().Insert(new MerchantDeposit
        //        {
        //            MerchantAccountId = toWallet.MerchantAccountId,
        //            MerchantWalletId = toWallet.Id,
        //            FromAddress = fromWallet.Address,
        //            ToAddress = toWallet.Address,
        //            Amount = fromWithdraw.Amount - fromWithdrawFee.Fee,
        //            Status = TransactionStatus.Confirmed,
        //            Timestamp = DateTime.UtcNow,
        //            OrderNo = CreateOrderno(),
        //            TransactionId = fromWallet.MerchantAccountId.ToString("N"),
        //            SelfPlatform = true
        //        });
        //        new MerchantWalletDAC().Increase(toWallet.Id, amount);
        //        new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
        //        {
        //            WalletId = toWallet.Id,
        //            Action = MerchantWalletStatementAction.Deposit,
        //            Amount = amount,
        //            Balance = toWallet.Balance + amount,
        //            Timestamp = DateTime.UtcNow
        //        });


        //        scope.Complete();
        //    }

        //    var withdrawId = fromWithdraw.Id;
        //    var depositId = deposit.Id;

        //    var t1 = Task.Run(() => new FiiiPOSPushComponent().PushWithdrawCompleted(withdrawId));
        //    var t2 = Task.Run(() => new FiiiPOSPushComponent().PushDeposit(depositId));
        //    Task.WaitAll(t1, t2);

        //    return fromWithdraw;
        //}

        private MerchantWithdrawal WithdrawalToUserAccount(MerchantWallet fromWallet, MerchantWithdrawal fromWithdraw,
            UserWallet toWallet, decimal fee)
        {
            UserDeposit deposit;
            using (var scope = new TransactionScope())
            {
                //提币
                fromWithdraw.Status = TransactionStatus.Confirmed;
                fromWithdraw.TransactionId = toWallet.UserAccountId.ToString("N");
                fromWithdraw.SelfPlatform = true;//平台内提币
                fromWithdraw = new MerchantWithdrawalDAC().Create(fromWithdraw);

                var fromWithdrawFee = new MerchantWithdrawalFee
                {
                    Amount = fromWithdraw.Amount,
                    Timestamp = DateTime.UtcNow,
                    Fee = fee,
                    WithdrawalId = fromWithdraw.Id
                };
                new MerchantWithdrawalFeeDAC().Create(fromWithdrawFee);

                new MerchantWalletDAC().Decrease(fromWallet.Id, fromWithdraw.Amount);

                new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                {
                    WalletId = fromWallet.Id,
                    Action = MerchantWalletStatementAction.Withdrawal,
                    Amount = -fromWithdraw.Amount,
                    Balance = fromWallet.Balance - fromWithdraw.Amount,
                    Timestamp = DateTime.UtcNow
                });

                //充币
                var amount = fromWithdraw.Amount - fromWithdrawFee.Fee;
                new UserWalletDAC().Increase(toWallet.Id, amount);
                new UserWalletStatementDAC().Insert(new UserWalletStatement
                {
                    WalletId = toWallet.Id,
                    Action = UserWalletStatementAction.Deposit,
                    Amount = amount,
                    Balance = toWallet.Balance + amount,
                    FrozenAmount = 0,
                    FrozenBalance = toWallet.FrozenBalance,
                    Timestamp = DateTime.UtcNow
                });

                deposit = new UserDepositDAC().Insert(new UserDeposit
                {
                    UserAccountId = toWallet.UserAccountId,
                    UserWalletId = toWallet.Id,
                    FromAddress = fromWallet.Address,
                    FromTag = fromWallet.Tag,
                    ToAddress = toWallet.Address,
                    ToTag = toWallet.Tag,
                    Amount = amount,
                    Status = TransactionStatus.Confirmed,
                    Timestamp = DateTime.UtcNow,
                    OrderNo = NumberGenerator.GenerateUnixOrderNo(),
                    TransactionId = fromWallet.MerchantAccountId.ToString("N"),
                    SelfPlatform = true,
                    CryptoCode = toWallet.CryptoCode
                });

                new UserTransactionDAC().Insert(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = toWallet.UserAccountId,
                    CryptoId = toWallet.CryptoId,
                    CryptoCode = toWallet.CryptoCode,
                    Type = UserTransactionType.Deposit,
                    DetailId = deposit.Id.ToString(),
                    Status = (byte)deposit.Status,
                    Timestamp = deposit.Timestamp,
                    Amount = amount,
                    OrderNo = deposit.OrderNo
                });
                
                scope.Complete();
            }

            var withdrawId = fromWithdraw.Id;
            var depositId = deposit.Id;

            MerchantMSMQ.PubMerchantWithdrawCompleted(withdrawId, 0);
            MerchantMSMQ.PubUserDeposit(depositId, 0);

            return fromWithdraw;
        }

        private MerchantWithdrawal WithdrawalToOutside(MerchantWallet fromWallet, MerchantWithdrawal fromWithdraw, Cryptocurrency cryptocurrency,
            MerchantAccount account, decimal amount, decimal fee, string address, string tag, string clientIP)
        {
            var actualAmount = amount - fee;

            ILog _logger = LogManager.GetLogger("LogicError");

            using (var scope = new TransactionScope())
            {
                try
                {
                    fromWithdraw.Status = TransactionStatus.UnSubmit;
                    fromWithdraw = new MerchantWithdrawalDAC().Create(fromWithdraw);
                    var fromWithdrawFee = new MerchantWithdrawalFee
                    {
                        Amount = amount,
                        Fee = fee,
                        Timestamp = DateTime.UtcNow,
                        WithdrawalId = fromWithdraw.Id
                    };
                    new MerchantWithdrawalFeeDAC().Create(fromWithdrawFee);
                    new MerchantWalletDAC().Freeze(fromWallet.Id, amount);

                    //var requestInfo = new FiiiFinanceAgent().TryCreateWithdraw(requestModel);

                    //new MerchantWithdrawalDAC().UpdateTransactionId(fromWithdraw.Id, requestInfo.RequestID,
                    //    requestInfo.TransactionId);

                    scope.Complete();
                }
                catch (CommonException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Info($"Withdraw ApplyWithdrawal faild.WithdrawID:{fromWithdraw.Id},OrderNo:{fromWithdraw.OrderNo}.request Parameter - . Error message:{ex.Message}");
                    throw;
                }
            }
            var requestModel = new CreateWithdrawModel
            {
                AccountID = account.Id,
                AccountType = AccountTypeEnum.Merchant,
                CryptoName = cryptocurrency.Code,
                ReceivingAddress = address,
                DestinationTag = tag,
                Amount = actualAmount,
                IPAddress = clientIP,
                TransactionFee = fee,
                WithdrawalId = fromWithdraw.Id
            };
            RabbitMQSender.SendMessage("WithdrawSubmit", requestModel);

            return fromWithdraw;
        }

        public WithdrawalConditionInfo GetWithdrawalInfo(Guid accountId, int cryptoId)
        {
            var result = new WithdrawalConditionInfo();

            var profileAgent = new MerchantProfileAgent();
            var profile = profileAgent.GetMerchantProfile(accountId);
            var crypto = new CryptocurrencyDAC().GetById(cryptoId);

            result.NeedTag = crypto.NeedTag;

            var wallet = new MerchantWalletDAC().GetByAccountId(accountId, cryptoId) ?? GenerateWallet(accountId, cryptoId, crypto.Code);
            //余额
            result.Balance = wallet.Balance.ToString(crypto.DecimalPlace);

            int level = profile.L2VerifyStatus == VerifyStatus.Certified ? 2 : profile.L1VerifyStatus == VerifyStatus.Certified ? 1 : 0;

            var masterSetting = GetMerchantWithdrawalMasterSettingWithCrypto(crypto, level);

            //最小提币量
            result.MinWithdrawalAmount = masterSetting.ToOutsideMinAmount.ToString(CultureInfo.InvariantCulture);
            // to fiiipay min withdraw amount
            result.ToFiiiPayMinWithdrawalAmount = masterSetting.ToUserMinAmount.ToString(CultureInfo.InvariantCulture);
            //手续费率
            result.HandleFeeTier = (crypto.Withdrawal_Tier ?? 0).ToString(CultureInfo.InvariantCulture);
            result.HandleFee = (crypto.Withdrawal_Fee ?? 0).ToString(CultureInfo.InvariantCulture);
            result.DecimalPlace = crypto.DecimalPlace;
            //限额
            result.VerifyLevel = level;
            result.PerTxLimit = masterSetting.PerTxLimit.ToString(crypto.DecimalPlace);
            result.PerDayLimit = masterSetting.PerDayLimit.ToString(crypto.DecimalPlace);
            result.PerMonthLimit = masterSetting.PerMonthLimit.ToString(crypto.DecimalPlace);
            var dac = new MerchantWithdrawalDAC();
            var today = DateTime.UtcNow.Date;
            decimal dailyWithdrawal = dac.DailyWithdrawal(accountId, cryptoId, today);
            decimal monthlyWithdrawal = dac.MonthlyWithdrawal(accountId, cryptoId, new DateTime(today.Year, today.Month, 1));
            decimal dayUsable = masterSetting.PerDayLimit - dailyWithdrawal;
            decimal monthUsable = masterSetting.PerMonthLimit - monthlyWithdrawal;
            dayUsable = Math.Min(dayUsable, monthUsable);

            result.PerDayUsable = (dayUsable < 0 ? 0 : dayUsable).ToString(crypto.DecimalPlace);
            result.PerMonthUsable = (monthUsable < 0 ? 0 : monthUsable).ToString(crypto.DecimalPlace);

            return result;
        }

        public TransactionFeeDTO TransactionFeeRate(int coinId, string address, string tag)
        {
            var userWalletDAC = new UserWalletDAC();
            var merchantWalletDAC = new MerchantWalletDAC();
            var coin = new CryptocurrencyDAC().GetById(coinId);
            var isUserWallet = userWalletDAC.IsUserWalletAddress(address);
            var isMerchantWallet = merchantWalletDAC.IsMerchantWalletAddress(address);
            bool isFiiiAccount = isUserWallet || isMerchantWallet;

            if (isFiiiAccount)
            {
                var withdrawMasterSetting = GetMerchantWithdrawalMasterSetting();
                var result = new TransactionFeeDTO
                {
                    TransactionFee = "0",
                    TransactionFeeRate = withdrawMasterSetting.ToUserHandleFeeTier.ToString(CultureInfo.InvariantCulture)
                };

                var userWallet = userWalletDAC.GetByAddressAndCrypto(coinId, address, tag);
                if (userWallet?.CryptoId == coinId)
                {
                    result.CryptoAddressType = CryptoAddressType.FiiiPay;
                    return result;
                }

                var merchantWallet = new MerchantWalletDAC().GetByAddress(address, tag);
                if (merchantWallet?.CryptoId == coinId)
                {
                    result.CryptoAddressType = CryptoAddressType.FiiiPOS;
                    return result;
                }

                result.CryptoAddressType = CryptoAddressType.InsideWithError;
                return result;
            }
            else
            {
                return new TransactionFeeDTO
                {
                    CryptoAddressType = CryptoAddressType.Outside,
                    TransactionFee = (coin.Withdrawal_Fee ?? 0).ToString(CultureInfo.InvariantCulture),
                    TransactionFeeRate = (coin.Withdrawal_Tier ?? 0).ToString(CultureInfo.InvariantCulture)
                };
            }
        }

        public List<MarketPriceInfo> GetMarketPrice(int countryId, string fiatCurrency)
        {
            var marketPriceComponent = new MarketPriceComponent();
            return marketPriceComponent.GetMarketPrice(fiatCurrency);
        }

        public string GetMarketPriceString(int countryId, string fiatCurrency)
        {
            return JsonConvert.SerializeObject(GetMarketPrice(countryId, fiatCurrency));
        }

        private MerchantWithdrawalMasterSettingDTO GetMerchantWithdrawalMasterSettingWithCrypto(Cryptocurrency crypto, int level)
        {
            var withdrawMasterSetting = GetMerchantWithdrawalMasterSetting();
            var marketPriceComponent = new MarketPriceComponent();
            var mpInfo = marketPriceComponent.GetMarketPrice("USD", crypto.Code);
            if (mpInfo == null)
                throw new CommonException(ReasonCode.NOT_SUPORT_WITHDRAWAL, Resources.不支持的币种);

            var exchangeRate = mpInfo.Price;

            switch (level)
            {
                case 1:
                    withdrawMasterSetting.PerTxLimit = (withdrawMasterSetting.PerTxLimit1 / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);
                    withdrawMasterSetting.PerDayLimit = (withdrawMasterSetting.PerDayLimit1 / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);
                    withdrawMasterSetting.PerMonthLimit = (withdrawMasterSetting.PerMonthLimit1 / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);
                    break;
                case 2:
                    withdrawMasterSetting.PerTxLimit = (withdrawMasterSetting.PerTxLimit2 / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);
                    withdrawMasterSetting.PerDayLimit = (withdrawMasterSetting.PerDayLimit2 / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);
                    withdrawMasterSetting.PerMonthLimit = (withdrawMasterSetting.PerMonthLimit2 / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);
                    break;
                default:
                    withdrawMasterSetting.PerTxLimit = (withdrawMasterSetting.PerTxLimit / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);
                    withdrawMasterSetting.PerDayLimit = (withdrawMasterSetting.PerDayLimit / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);
                    withdrawMasterSetting.PerMonthLimit = (withdrawMasterSetting.PerMonthLimit / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);
                    break;
            }
            //未设置 默认最小值
            if (withdrawMasterSetting.ToOutsideMinAmount <= 0)
                withdrawMasterSetting.ToOutsideMinAmount = 1 / (decimal)Math.Pow(10, crypto.DecimalPlace);
            else
                withdrawMasterSetting.ToOutsideMinAmount = (withdrawMasterSetting.ToOutsideMinAmount / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);

            //未设置 默认最小值
            if (withdrawMasterSetting.ToUserMinAmount <= 0)
                withdrawMasterSetting.ToUserMinAmount = 1 / (decimal)Math.Pow(10, crypto.DecimalPlace);
            else
                withdrawMasterSetting.ToUserMinAmount = (withdrawMasterSetting.ToUserMinAmount / exchangeRate).ToSpecificDecimal(crypto.DecimalPlace);

            return withdrawMasterSetting;
        }

        public MerchantWithdrawalMasterSettingDTO GetMerchantWithdrawalMasterSetting()
        {
            var msList = new MasterSettingDAC().SelectByGroup("MerchantWithdrawal");
            return new MerchantWithdrawalMasterSettingDTO
            {
                PerTxLimit = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "Withdrawal_PerTx_Limit_Merchant_NotVerified")?.Value),
                PerDayLimit = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "Withdrawal_PerDay_Limit_Merchant_NotVerified")?.Value),
                PerMonthLimit = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "Withdrawal_PerMonth_Limit_Merchant_NotVerified")?.Value),

                PerTxLimit1 = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "Withdrawal_PerTx_Limit_Merchant_Lv1Verified")?.Value),
                PerDayLimit1 = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "Withdrawal_PerDay_Limit_Merchant_Lv1Verified")?.Value),
                PerMonthLimit1 = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "Withdrawal_PerMonth_Limit_Merchant_Lv1Verified")?.Value),

                PerTxLimit2 = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "Withdrawal_PerTx_Limit_Merchant_Lv2Verified")?.Value),
                PerDayLimit2 = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "Withdrawal_PerDay_Limit_Merchant_Lv2Verified")?.Value),
                PerMonthLimit2 = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "Withdrawal_PerMonth_Limit_Merchant_Lv2Verified")?.Value),

                ToOutsideMinAmount = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "Withdrawal_MinAmount")?.Value),
                ToUserMinAmount = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "MerchantWithdrawal_ToUser_MinAmount")?.Value),

                ToUserHandleFeeTier = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "MerchantWithdrawal_ToUser")?.Value),
                ToMerchantHandleFeeTier = Convert.ToDecimal(msList.FirstOrDefault(e => e.Name == "MerchantWithdrawal_ToMerchant")?.Value)
            };
        }
    }
}