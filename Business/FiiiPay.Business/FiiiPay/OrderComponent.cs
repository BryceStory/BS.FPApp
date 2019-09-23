using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FiiiPay.Business.Properties;
using FiiiPay.Data;
using FiiiPay.Data.Agents;
using FiiiPay.DTO.Order;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Foundation.Entities.Enum;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using Newtonsoft.Json;

namespace FiiiPay.Business
{
    public partial class OrderComponent : BaseComponent
    {
        //private static readonly bool IsPushProduction = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("PushProduction"));

        public PayOrderOM PayExistedOrder(UserAccount user, string orderNo, string pin)
        {
            new SecurityComponent().VerifyPin(user, pin);

            var isAllowExpense = user.IsAllowExpense ?? true;
            if (!isAllowExpense)
                throw new CommonException(ReasonCode.Not_Allow_Expense, MessageResources.PaymentForbidden);

            var order = RedisHelper.Get<RedisOrderDTO>($"fiiipos:order:{orderNo}");
            RedisHelper.KeyDelete($"fiiipos:order:{orderNo}");
            if (order == null)
                throw new ApplicationException(MessageResources.OrderNotFound);

            //if (new OrderDAC().GetByOrderNo(orderNo) != null)
            //{
            //    throw new ApplicationException(Resources.订单已完成或者已退款);
            //}

            if (order.UserId != Guid.Empty)
            {
                if (order.UserId != user.Id)
                    throw new ApplicationException(MessageResources.AccountNotMatch);
            }

            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);
            if (!coin.Status.HasFlag(CryptoStatus.Pay) || coin.Enable == 0)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, MessageResources.CurrencyForbidden);

            var userWallet = new UserWalletDAC().GetByAccountId(user.Id, order.CryptoId);
            if (userWallet == null)
                throw new CommonException(ReasonCode.INSUFFICIENT_BALANCE, MessageResources.InsufficientBalance);
            var exchangeRate = GetExchangeRate(order.CountryId, order.FiatCurrency, coin);
            decimal fiatTotalAmount = (order.FiatAmount * (1 + order.Markup)).ToSpecificDecimal(4);
            decimal cryptoAmount = (fiatTotalAmount / exchangeRate).ToSpecificDecimal(coin.DecimalPlace);
            
            if (userWallet.Balance < cryptoAmount)
                throw new CommonException(ReasonCode.INSUFFICIENT_BALANCE, MessageResources.InsufficientBalance);

            var merchantAccount = new MerchantAccountDAC().GetById(order.MerchantGuid);
            if (!merchantAccount.IsAllowAcceptPayment || merchantAccount.Status == AccountStatus.Locked)
                throw new CommonException(ReasonCode.Not_Allow_Withdrawal, MessageResources.MerchantExceptionTransClose);

            var merchantWallet = new MerchantWalletDAC().GetByAccountId(order.MerchantGuid, order.CryptoId);
            if (merchantWallet == null || !merchantWallet.SupportReceipt)
                throw new ApplicationException(MessageResources.MerchantNotSupperCrypto);
            var country = new CountryComponent().GetById(merchantAccount.CountryId);
            var orderData = new Order
            {
                Id = Guid.NewGuid(),
                OrderNo = order.OrderNo,
                MerchantAccountId = merchantAccount.Id,
                CryptoId = coin.Id,
                CryptoCode = coin.Code,
                FiatAmount = order.FiatAmount,
                PaymentType = order.Type,
                FiatCurrency = order.FiatCurrency,
                Status = OrderStatus.Completed,
                ExpiredTime = DateTime.UtcNow.AddMinutes(30),
                Markup = merchantAccount.Markup,
                ExchangeRate = GetExchangeRate(merchantAccount.CountryId, order.FiatCurrency, coin),
                UnifiedExchangeRate = GetExchangeRate(merchantAccount.CountryId, country.FiatCurrency ?? "USD", coin),
                UnifiedFiatCurrency = country.FiatCurrency ?? "USD",
                MerchantIP = order.ClientIP,
                PaymentTime = DateTime.UtcNow,
                Timestamp = DateTime.UtcNow,
                UserAccountId = user.Id
            };
            if (merchantAccount.WithdrawalFeeType != WithdrawalFeeType.FiiiCoin)
            {
                var calcModel =
                    CalculateAmount(order.FiatAmount, merchantAccount.Markup, merchantAccount.Receivables_Tier, orderData.ExchangeRate, coin);

                orderData.ActualFiatAmount = calcModel.FiatTotalAmount;
                orderData.CryptoAmount = calcModel.CryptoAmount;
                orderData.TransactionFee = calcModel.TransactionFee;
                orderData.ActualCryptoAmount = calcModel.ActualCryptoAmount;

                var model = CalculateUnifiedAmount(orderData.CryptoAmount, orderData.ActualCryptoAmount, orderData.UnifiedExchangeRate);
                orderData.UnifiedFiatAmount = model.UnifiedFiatAmount;
                orderData.UnifiedActualFiatAmount = model.UnifiedActualFiatAmount;
                var orderWithdrawalFee = new OrderWithdrawalFee(){Timestamp = DateTime.UtcNow};
                orderWithdrawalFee.CryptoId = orderData.CryptoId;
                orderWithdrawalFee.Amount = 0;

                Execute(orderWithdrawalFee);
            }
            else
            {
                var orderWithdrawalFee = CalculateOrderAmount(ref orderData, order, merchantAccount, coin);
                var wallet = new MerchantWalletDAC().GetByAccountId(merchantAccount.Id, new CryptocurrencyDAC().GetByCode("FIII").Id);
                if (orderWithdrawalFee.Amount != 0)
                {
                    using (var scope = new TransactionScope())
                    {
                        var dbOrder = new OrderDAC().Create(orderData);
                        new UserTransactionDAC().Insert(new UserTransaction
                        {
                            Id = Guid.NewGuid(),
                            AccountId = userWallet.UserAccountId,
                            CryptoId = userWallet.CryptoId,
                            CryptoCode = userWallet.CryptoCode,
                            Type = UserTransactionType.Order,
                            DetailId = dbOrder.Id.ToString(),
                            Status = (byte)dbOrder.Status,
                            Timestamp = dbOrder.Timestamp,
                            Amount = dbOrder.CryptoAmount,
                            OrderNo = dbOrder.OrderNo,
                            MerchantName = merchantAccount.MerchantName
                        });

                        orderWithdrawalFee.OrderId = dbOrder.Id;
                        var id = new OrderWithdrawalFeeDAC().Insert(orderWithdrawalFee);
                        new MerchantWalletDAC().Decrease(wallet.Id, orderWithdrawalFee.Amount);
                        new MerchantWalletDAC().Increase(merchantWallet.Id, orderData.ActualCryptoAmount);
                        new UserWalletDAC().Decrease(userWallet.Id, cryptoAmount);
                        new UserWalletStatementDAC().Insert(new UserWalletStatement
                        {
                            Action = UserWalletStatementAction.Consume,
                            Amount = orderData.CryptoAmount,
                            Balance = userWallet.Balance - orderData.CryptoAmount,
                            FrozenAmount = 0,
                            FrozenBalance = userWallet.FrozenBalance,
                            Remark = null,
                            Timestamp = DateTime.UtcNow,
                            WalletId = userWallet.Id
                        });
                        new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                        {
                            Action = MerchantWalletStatementAction.Receipt,
                            Amount = orderData.ActualCryptoAmount,
                            Balance = merchantWallet.Balance + orderData.ActualCryptoAmount,
                            Remark = null,
                            Timestamp = DateTime.UtcNow,
                            WalletId = merchantWallet.Id
                        });
                        new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                        {
                            Action = MerchantWalletStatementAction.Withdrawal,
                            Amount = orderWithdrawalFee.Amount,
                            Balance = merchantWallet.Balance - orderData.ActualCryptoAmount,
                            Remark = null,
                            Timestamp = DateTime.UtcNow,
                            WalletId = wallet.Id
                        });
                        scope.Complete();
                    }
                }
                else
                {
                    Execute(orderWithdrawalFee);
                }
            }
            void Execute(OrderWithdrawalFee orderWithdrawalFee)
            {
                using (var scope = new TransactionScope())
                {
                    var dbOrder = new OrderDAC().Create(orderData);
                    new UserTransactionDAC().Insert(new UserTransaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = userWallet.UserAccountId,
                        CryptoId = userWallet.CryptoId,
                        CryptoCode = userWallet.CryptoCode,
                        Type = UserTransactionType.Order,
                        DetailId = dbOrder.Id.ToString(),
                        Status = (byte)dbOrder.Status,
                        Timestamp = dbOrder.Timestamp,
                        Amount = dbOrder.CryptoAmount,
                        OrderNo = dbOrder.OrderNo,
                        MerchantName = merchantAccount.MerchantName
                    });
                    orderWithdrawalFee.OrderId = dbOrder.Id;
                    var id = new OrderWithdrawalFeeDAC().Insert(orderWithdrawalFee);
                    new UserWalletDAC().Decrease(userWallet.Id, cryptoAmount);
                    new MerchantWalletDAC().Increase(merchantWallet.Id, orderData.ActualCryptoAmount);
                    new UserWalletStatementDAC().Insert(new UserWalletStatement
                    {
                        Action = UserWalletStatementAction.Consume,
                        Amount = orderData.CryptoAmount,
                        Balance = userWallet.Balance - orderData.CryptoAmount,
                        FrozenAmount = 0,
                        FrozenBalance = userWallet.FrozenBalance,
                        Remark = null,
                        Timestamp = DateTime.UtcNow,
                        WalletId = userWallet.Id
                    });
                    new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                    {
                        Action = MerchantWalletStatementAction.Receipt,
                        Amount = orderData.ActualCryptoAmount,
                        Balance = merchantWallet.Balance + orderData.ActualCryptoAmount,
                        Remark = null,
                        Timestamp = DateTime.UtcNow,
                        WalletId = merchantWallet.Id
                    });
                    scope.Complete();
                }
            }

            UserMSMQ.PubOrderPayed(orderData.Id, 0);
            //PushConsume(orderData.Id);
            UserMSMQ.PubConsumeOrder(orderData.Id);

            return new PayOrderOM
            {
                Amount = orderData.CryptoAmount.ToString(coin.DecimalPlace),
                Currency = new CryptocurrencyDAC().GetById(order.CryptoId).Code,
                OrderId = orderData.Id.ToString(),
                OrderNo = orderData.OrderNo,
                Timestamp = orderData.PaymentTime?.ToUnixTime().ToString()
            };
        }

        private OrderWithdrawalFee CalculateOrderAmount(ref Order order, RedisOrderDTO orderDto, MerchantAccount account, Cryptocurrency coin)
        {
            var orderWithdrawalFee = new OrderWithdrawalFee() { Timestamp = DateTime.UtcNow };
            
            var wallet = new MerchantWalletDAC().GetByAccountId(account.Id, new CryptocurrencyDAC().GetByCode("FIII").Id);
            if (wallet == null || !wallet.SupportReceipt)
            {
                Excute(ref order);
            }
            else
            {
                var exchangeFiiiRate = GetExchangeRate(orderDto.CountryId, order.FiatCurrency, new CryptocurrencyDAC().GetById(wallet.CryptoId));
                var fiiiCoin = new CryptocurrencyDAC().GetById(wallet.CryptoId);

                //var tempFiatActualAmount = orderDto.FiatAmount * (1 + account.Markup);
                var tempFiatActualAmount = orderDto.FiatAmount + (orderDto.FiatAmount * account.Markup).ToSpecificDecimal(4);

                var fiiiTransactionFee = ((orderDto.FiatAmount + orderDto.FiatAmount * account.Markup) * account.Receivables_Tier / exchangeFiiiRate)
                    .ToSpecificDecimal(
                        fiiiCoin.DecimalPlace);
                if (wallet.Balance < fiiiTransactionFee)
                {
                    Excute(ref order);
                }
                else
                {
                    orderWithdrawalFee.CryptoId = fiiiCoin.Id;
                    orderWithdrawalFee.Amount = fiiiTransactionFee;

                    order.ActualFiatAmount = tempFiatActualAmount.ToSpecificDecimal(4);
                    order.CryptoAmount = ((orderDto.FiatAmount + orderDto.FiatAmount * account.Markup) / order.ExchangeRate).ToSpecificDecimal(coin.DecimalPlace);
                    order.TransactionFee = 0;
                    order.ActualCryptoAmount = order.CryptoAmount;

                    var model = CalculateUnifiedAmount(order.CryptoAmount, order.ActualCryptoAmount, order.UnifiedExchangeRate);
                    order.UnifiedFiatAmount = model.UnifiedFiatAmount;
                    order.UnifiedActualFiatAmount = model.UnifiedActualFiatAmount;
                }
            }
            void Excute(ref Order inOrder)
            {
                var calcModel =
                    CalculateAmount(orderDto.FiatAmount, account.Markup, account.Receivables_Tier, inOrder.ExchangeRate, coin);
                inOrder.ActualFiatAmount = calcModel.FiatTotalAmount;
                inOrder.CryptoAmount = calcModel.CryptoAmount;
                inOrder.TransactionFee = calcModel.TransactionFee;
                inOrder.ActualCryptoAmount = calcModel.ActualCryptoAmount;

                var model = CalculateUnifiedAmount(inOrder.CryptoAmount, inOrder.ActualCryptoAmount, inOrder.UnifiedExchangeRate);

                inOrder.UnifiedFiatAmount = model.UnifiedFiatAmount;
                inOrder.UnifiedActualFiatAmount = model.UnifiedActualFiatAmount;

                orderWithdrawalFee.CryptoId = inOrder.CryptoId;
                orderWithdrawalFee.Amount = 0;
            }
            return orderWithdrawalFee;
        }

        public PrePayExistedOrderOM PrePayExistedOrder(UserAccount user, PrePayExistedOrderIM im)
        {
            var orderno = im.OrderNo;
            var order = RedisHelper.Get<RedisOrderDTO>($"fiiipos:order:{orderno}");
            
            if (order == null)
            {
                throw im.Type == PrePayExistedOrderIMType.ScanCode
                    ? new CommonException(ReasonCode.INVALID_QRCODE, MessageResources.InvalidQRCode)
                    : new CommonException(ReasonCode.GENERAL_ERROR, MessageResources.OrderNotFound);
            }
            //if (order.ExpiredTime < DateTime.UtcNow)
            //    throw new ApplicationException(Resources.订单已过期);
            //if (order.Status != OrderStatus.Pending)
            //    throw new ApplicationException(Resources.订单已完成或者已退款);
            if (order.UserId != Guid.Empty)
            {
                if (order.UserId != user.Id)
                    throw new ApplicationException(MessageResources.AccountNotMatch);
            }
            if (new OrderDAC().GetByOrderNo(orderno) != null)
            {
                throw new ApplicationException(MessageResources.OrderComplated);
            }

            var userWallet = new UserWalletDAC().GetByAccountId(user.Id, order.CryptoId) ?? new UserWallet { Balance = 0, FrozenBalance = 0 };
            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);
            var exchangeRate = GetExchangeRate(order.CountryId, order.FiatCurrency, coin);
            decimal fiatTotalAmount = (order.FiatAmount * (1 + order.Markup)).ToSpecificDecimal(4);
            decimal cryptoAmount = (fiatTotalAmount / exchangeRate).ToSpecificDecimal(coin.DecimalPlace);
            return new PrePayExistedOrderOM
            {
                Amount = cryptoAmount.ToString(coin.DecimalPlace),
                Currency = coin.Code,
                MerchantName = new MerchantAccountDAC().GetById(order.MerchantGuid).MerchantName,
                Balance = userWallet.Balance.ToString(coin.DecimalPlace),
                CoinId = order.CryptoId
            };
        }

        

        public PrePayOM PrePay(UserAccount user, PrePayIM im)
        {
            var merchantAccount = GetMerchantAccountByIdOrCode(im.MerchantId, im.MerchantCode);
            var userWallets = new UserWalletDAC().GetUserWallets(user.Id);
            var merchantWallets = new MerchantWalletDAC().GetByAccountId(merchantAccount.Id);
            var coins = new CryptocurrencyDAC().GetAllActived();
            var priceList = new PriceInfoDAC().GetPrice(merchantAccount.FiatCurrency);

            bool showWhiteLable = false;
            if (merchantAccount.POSId.HasValue)
            {
                var pos = new POSDAC().GetById(merchantAccount.POSId.Value);
                if (pos.IsWhiteLabel)
                    showWhiteLable = true;
            }

            if(!showWhiteLable)
            {
                var whilteLabelCryptoCode = new POSDAC().GetWhiteLabelCryptoCode();
                coins.RemoveAll(t => t.Code == whilteLabelCryptoCode);
            }

            return new PrePayOM
            {
                FiatCurrency = merchantAccount.FiatCurrency,
                MarkupRate = merchantAccount.Markup.ToString(CultureInfo.InvariantCulture),
                WaletList = coins.Select(a =>
                {
                    var userWallet = userWallets.FirstOrDefault(b => b.CryptoId == a.Id);
                    decimal rate = 0;
                    rate = priceList.Where(t => t.CryptoID == a.Id).Select(t => t.Price).FirstOrDefault();
                    return GetItem(userWallet, a, merchantWallets, rate);
                }).OrderByDescending(a => a.MerchantSupported).ThenBy(a => a.PayRank).Select(a => new WalletItem
                {
                    Code = a.Code,
                    NewStatus = a.NewStatus,
                    ExchangeRate = a.ExchangeRate,
                    FrozenBalance = a.FrozenBalance,
                    IconUrl = a.IconUrl,
                    Id = a.Id,
                    MerchantSupported = a.MerchantSupported,
                    Name = a.Name,
                    UseableBalance = a.UseableBalance,
                    FiatBalance = a.FiatBalance,
                    DecimalPlace = a.DecimalPlace,
                    CryptoEnable = a.CryptoEnable
                }).ToList()
            };
        }

        public async Task<ScanMerchantQRCodeOM> ScanMerchantQRCode(UserAccount user, string code)
        {
            var codeEntity = QRCode.Deserialize(code);
            if (codeEntity.SystemPlatform != SystemPlatform.FiiiPOS || codeEntity.QrCodeEnum != QRCodeEnum.MerchantScanPay)
                throw new CommonException(ReasonCode.INVALID_QRCODE, MessageResources.InvalidQRCode);
            Guid merchantId = Guid.Parse(codeEntity.QRCodeKey);
            ScanMerchantQRCodeOM om = new ScanMerchantQRCodeOM();

            var merchantAccount = GetMerchantAccountByIdOrCode(merchantId, null);
            //if (merchantAccount.POSId == null)
            //{
            //    throw new CommonException(ReasonCode.INVALID_QRCODE, MessageResources.InvalidQRCode);
            //}

            if (merchantAccount.Status == AccountStatus.Locked || !merchantAccount.IsAllowAcceptPayment)
                throw new CommonException(ReasonCode.MERCHANT_ACCOUNT_DISABLED, MessageResources.MerchantAccountDisabled);

            Guid.TryParse(merchantAccount.Photo, out Guid merchantAvatar);

            var uwDAC = new UserWalletDAC();
            var mwDAC = new MerchantWalletDAC();

            var userWallets = uwDAC.GetUserWallets(user.Id);
            var merchantWallets = mwDAC.GetByAccountId(merchantAccount.Id);
            var coins = new CryptocurrencyDAC().GetAllActived();
            var priceList = new PriceInfoDAC().GetPrice(merchantAccount.FiatCurrency);

            //判断Pos机是否白标用户
            bool showWhiteLable = false;
            if (merchantAccount.POSId.HasValue)
            {
                var pos = new POSDAC().GetById(merchantAccount.POSId.Value);
                if (pos.IsWhiteLabel)
                    showWhiteLable = true;
            }

            if (!showWhiteLable)
            {
                var whilteLabelCryptoCode = new POSDAC().GetWhiteLabelCryptoCode();
                coins.RemoveAll(t => t.Code == whilteLabelCryptoCode);
            }

            return await Task.FromResult(new ScanMerchantQRCodeOM
            {
                MerchantId = merchantAccount.Id,
                MerchantName = merchantAccount.MerchantName,
                Avatar = merchantAvatar,
                L1VerifyStatus = (byte)merchantAccount.L1VerifyStatus,
                L2VerifyStatus = (byte)merchantAccount.L2VerifyStatus,
                FiatCurrency = merchantAccount.FiatCurrency,
                MarkupRate = merchantAccount.Markup.ToString(CultureInfo.InvariantCulture),
                WaletInfoList = coins.Select(a =>
                {
                    var userWallet = userWallets.FirstOrDefault(b => b.CryptoId == a.Id);
                    decimal rate = 0;
                    rate = priceList.Where(t => t.CryptoID == a.Id).Select(t => t.Price).FirstOrDefault();
                    return GetItem(userWallet, a, merchantWallets, rate);
                }).OrderByDescending(a => a.MerchantSupported).ThenBy(a => a.PayRank).Select(a => new WalletItem
                {
                    Code = a.Code,
                    NewStatus = a.NewStatus,
                    ExchangeRate = a.ExchangeRate,
                    FrozenBalance = a.FrozenBalance,
                    IconUrl = a.IconUrl,
                    Id = a.Id,
                    MerchantSupported = a.MerchantSupported,
                    Name = a.Name,
                    UseableBalance = a.UseableBalance,
                    FiatBalance = a.FiatBalance,
                    DecimalPlace = a.DecimalPlace,
                    CryptoEnable = a.CryptoEnable
                }).ToList()
            });
        }


        public MerchantAccount GetMerchantAccountByIdOrCode(Guid? merchantId, string code)
        {
            if (!merchantId.HasValue)
            {
                merchantId = new TokenAgent().GetMerchantIntoByCode(code)?.MerchantId;
            }

            return merchantId.HasValue ? new MerchantAccountDAC().GetById(merchantId.Value) : null;
        }

        private WalletItemTemp GetItem(UserWallet model, Cryptocurrency coin, List<MerchantWallet> merchantWallets, decimal exchantRate)
        {
            model = model ?? new UserWallet { FrozenBalance = 0, Balance = 0, PayRank = int.MaxValue };

            return new WalletItemTemp
            {
                Id = coin.Id,
                FrozenBalance = model.FrozenBalance.ToString(coin.DecimalPlace),
                IconUrl = coin.IconURL,
                Code = coin.Code,
                NewStatus = coin.Status,
                UseableBalance = model.Balance.ToString(coin.DecimalPlace),
                PayRank = model.PayRank,
                Name = coin.Name,
                ExchangeRate = exchantRate.ToString(4),
                FiatBalance = (model.Balance * exchantRate).ToString(2),
                MerchantSupported = merchantWallets.Exists(a => a.CryptoId == coin.Id && a.SupportReceipt),
                DecimalPlace = coin.DecimalPlace,
                CryptoEnable = coin.Enable
            };
        }

        class WalletItemTemp : WalletItem
        {
            public int PayRank { get; set; }
            
        }

        /// <summary>
        /// 用户主动支付
        /// </summary>
        /// <param name="user"></param>
        /// <param name="im"></param>
        /// <returns></returns>
        public PayOrderOM Pay(UserAccount user, PayIM im)
        {
            if (im.Amount <= 0)
                throw new CommonException(ReasonCode.GENERAL_ERROR, MessageResources.AmountGreater);

            new SecurityComponent().VerifyPin(user, im.Pin);

            var isAllowExpense = user.IsAllowExpense ?? true;
            if (!isAllowExpense)
                throw new CommonException(ReasonCode.Not_Allow_Expense, MessageResources.PaymentForbidden);

            var coin = new CryptocurrencyDAC().GetById(im.CoinId);
            if (!coin.Status.HasFlag(CryptoStatus.Pay) || coin.Enable == 0)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, MessageResources.CurrencyForbidden);

            var merchantAccount = GetMerchantAccountByIdOrCode(im.MerchantId, im.MerchantCode);

            if (merchantAccount == null)
                throw new CommonException(ReasonCode.INVALID_QRCODE, MessageResources.InvalidQRCode);

            if (!merchantAccount.IsAllowAcceptPayment || merchantAccount.Status == AccountStatus.Locked)
                throw new CommonException(ReasonCode.Not_Allow_AcceptPayment, MessageResources.MerchantExceptionTransClose);

            var country = new CountryComponent().GetById(merchantAccount.CountryId);

            var orderData = new Order
            {
                Id = Guid.NewGuid(),
                OrderNo = IdentityHelper.OrderNo(),
                MerchantAccountId = merchantAccount.Id,
                CryptoId = coin.Id,
                CryptoCode = coin.Code,
                FiatAmount = im.Amount,
                PaymentType = im.PaymentType,
                FiatCurrency = merchantAccount.FiatCurrency,
                Status = OrderStatus.Completed,
                ExpiredTime = DateTime.UtcNow.AddMinutes(30),
                Markup = merchantAccount.Markup,
                ExchangeRate = GetExchangeRate(merchantAccount.CountryId, merchantAccount.FiatCurrency, coin),
                UnifiedExchangeRate = GetExchangeRate(merchantAccount.CountryId, country.FiatCurrency ?? "USD", coin),
                UnifiedFiatCurrency = country.FiatCurrency ?? "USD",
                MerchantIP = null,
                PaymentTime = DateTime.UtcNow,
                UserAccountId = user.Id,
                Timestamp = DateTime.UtcNow
            };

            var order = Generate(merchantAccount, coin, country.FiatCurrency ?? "USD", im.Amount, im.PaymentType);

            order.UserAccountId = user.Id;

            var userWallet = new UserWalletDAC().GetByAccountId(user.Id, im.CoinId);

            if (userWallet.Balance < order.CryptoAmount)
                throw new CommonException(ReasonCode.INSUFFICIENT_BALANCE, MessageResources.InsufficientBalance);

            order.Status = OrderStatus.Completed;
            order.PaymentTime = DateTime.UtcNow;

            var merchantWallet = new MerchantWalletDAC().GetByAccountId(order.MerchantAccountId, order.CryptoId);

            if (merchantWallet == null || !merchantWallet.SupportReceipt)
                throw new ApplicationException(MessageResources.MerchantNotSupperCrypto);

            if (merchantAccount.WithdrawalFeeType != WithdrawalFeeType.FiiiCoin)
            {
                var calcModel =
                    CalculateAmount(im.Amount, merchantAccount.Markup, merchantAccount.Receivables_Tier, orderData.ExchangeRate, coin);

                orderData.ActualFiatAmount = calcModel.FiatTotalAmount;
                orderData.CryptoAmount = calcModel.CryptoAmount;
                orderData.TransactionFee = calcModel.TransactionFee;
                orderData.ActualCryptoAmount = calcModel.ActualCryptoAmount;

                var model = CalculateUnifiedAmount(orderData.CryptoAmount, orderData.ActualCryptoAmount, orderData.UnifiedExchangeRate);
                orderData.UnifiedFiatAmount = model.UnifiedFiatAmount;
                orderData.UnifiedActualFiatAmount = model.UnifiedActualFiatAmount;
                var orderWithdrawalFee = new OrderWithdrawalFee
                {
                    Timestamp = DateTime.UtcNow,
                    CryptoId = orderData.CryptoId,
                    Amount = 0
                };

                Execute(orderWithdrawalFee);
            }
            else
            {
                var orderWithdrawalFee = CalculateOrderAmount(ref orderData, new RedisOrderDTO()
                {
                    CountryId = merchantAccount.CountryId, FiatAmount = im.Amount
                }, merchantAccount, coin);
                var wallet = new MerchantWalletDAC().GetByAccountId(merchantAccount.Id, new CryptocurrencyDAC().GetByCode("FIII").Id);
                if (orderWithdrawalFee.Amount != 0)
                {
                    using (var scope = new TransactionScope())
                    {
                        var dbOrder = new OrderDAC().Create(orderData);
                        new UserTransactionDAC().Insert(new UserTransaction
                        {
                            Id = Guid.NewGuid(),
                            AccountId = userWallet.UserAccountId,
                            CryptoId = userWallet.CryptoId,
                            CryptoCode = userWallet.CryptoCode,
                            Type = UserTransactionType.Order,
                            DetailId = dbOrder.Id.ToString(),
                            Status = (byte)dbOrder.Status,
                            Timestamp = dbOrder.Timestamp,
                            Amount = dbOrder.CryptoAmount,
                            OrderNo = dbOrder.OrderNo,
                            MerchantName = merchantAccount.MerchantName
                        });
                        orderWithdrawalFee.OrderId = dbOrder.Id;
                        var id = new OrderWithdrawalFeeDAC().Insert(orderWithdrawalFee);
                        
                        new MerchantWalletDAC().Decrease(wallet.Id, orderWithdrawalFee.Amount);
                        new MerchantWalletDAC().Increase(merchantWallet.Id, orderData.ActualCryptoAmount);
                        new UserWalletDAC().Decrease(userWallet.Id, orderData.ActualCryptoAmount);
                        new UserWalletStatementDAC().Insert(new UserWalletStatement
                        {
                            Action = UserWalletStatementAction.Consume,
                            Amount = orderData.CryptoAmount,
                            Balance = userWallet.Balance - orderData.CryptoAmount,
                            FrozenAmount = 0,
                            FrozenBalance = userWallet.FrozenBalance,
                            Remark = null,
                            Timestamp = DateTime.UtcNow,
                            WalletId = userWallet.Id
                        });
                        new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                        {
                            Action = MerchantWalletStatementAction.Receipt,
                            Amount = orderData.ActualCryptoAmount,
                            Balance = merchantWallet.Balance + orderData.ActualCryptoAmount,
                            Remark = null,
                            Timestamp = DateTime.UtcNow,
                            WalletId = merchantWallet.Id
                        });
                        new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                        {
                            Action = MerchantWalletStatementAction.Withdrawal,
                            Amount = orderWithdrawalFee.Amount,
                            Balance = merchantWallet.Balance - orderData.ActualCryptoAmount,
                            Remark = null,
                            Timestamp = DateTime.UtcNow,
                            WalletId = wallet.Id
                        });
                        scope.Complete();
                    }
                }
                else
                {
                    Execute(orderWithdrawalFee);
                }
            }
            void Execute(OrderWithdrawalFee orderWithdrawalFee)
            {
                using (var scope = new TransactionScope())
                {
                    var dbOrder = new OrderDAC().Create(orderData);
                    new UserTransactionDAC().Insert(new UserTransaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = userWallet.UserAccountId,
                        CryptoId = userWallet.CryptoId,
                        CryptoCode = userWallet.CryptoCode,
                        Type = UserTransactionType.Order,
                        DetailId = dbOrder.Id.ToString(),
                        Status = (byte)dbOrder.Status,
                        Timestamp = dbOrder.Timestamp,
                        Amount = dbOrder.CryptoAmount,
                        OrderNo = dbOrder.OrderNo,
                        MerchantName = merchantAccount.MerchantName
                    });
                    orderWithdrawalFee.OrderId = dbOrder.Id;
                    var id = new OrderWithdrawalFeeDAC().Insert(orderWithdrawalFee);
                    
                    new UserWalletDAC().Decrease(userWallet.Id, orderData.CryptoAmount);
                    new MerchantWalletDAC().Increase(merchantWallet.Id, orderData.ActualCryptoAmount);
                    new UserWalletStatementDAC().Insert(new UserWalletStatement
                    {
                        Action = UserWalletStatementAction.Consume,
                        Amount = orderData.CryptoAmount,
                        Balance = userWallet.Balance - orderData.CryptoAmount,
                        FrozenAmount = 0,
                        FrozenBalance = userWallet.FrozenBalance,
                        Remark = null,
                        Timestamp = DateTime.UtcNow,
                        WalletId = userWallet.Id
                    });
                    new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                    {
                        Action = MerchantWalletStatementAction.Receipt,
                        Amount = orderData.ActualCryptoAmount,
                        Balance = merchantWallet.Balance + orderData.ActualCryptoAmount,
                        Remark = null,
                        Timestamp = DateTime.UtcNow,
                        WalletId = merchantWallet.Id
                    });
                    scope.Complete();
                }
            }

            UserMSMQ.PubOrderPayed(orderData.Id, 0);
            //PushConsume(order.Id);
            UserMSMQ.PubConsumeOrder(orderData.Id);

            return new PayOrderOM
            {
                Amount = orderData.CryptoAmount.ToString(coin.DecimalPlace),
                Currency = new CryptocurrencyDAC().GetById(order.CryptoId).Code,
                OrderId = orderData.Id.ToString(),
                OrderNo = orderData.OrderNo,
                Timestamp = orderData.PaymentTime?.ToUnixTime().ToString()
            };
        }
        public OrderDetailOM Detail(UserAccount user, Guid id, bool isZH)
        {
            var order = new OrderDAC().GetOrderByOrderId(id);
            var gatewayOrder = new GatewayOrderDAC().GetByOrderId(id);

            if ((order == null || order.UserAccountId != user.Id) && (gatewayOrder == null || gatewayOrder.UserAccountId != user.Id))
            {
                throw new ApplicationException(MessageResources.OrderNotFound);
            }


            //if (order == null)
            //{
            //    var coin = new CryptocurrencyDAC().GetById(gatewayOrder.CryptoId);
            //    var er = order.ExchangeRate;
            //    var cer = GetExchangeRate(user.CountryId, order.FiatCurrency, coin);
            //    var iRate = ((cer - er) / er) * 100;
            //    om = new Detail1OM
            //    {
            //        Code = coin.Code,
            //        CryptoAmount = gatewayOrder.ActualCryptoAmount.ToString(coin.DecimalPlace),
            //        FiatAmount = gatewayOrder.ActualFiatAmount?.ToString(2) ?? string.Empty,
            //        ExchangeRate = $"1 {coin.Code} = {gatewayOrder.ExchangeRate.ToString(2)} {gatewayOrder.FiatCurrency}",
            //        FiatCurrency = gatewayOrder.FiatCurrency,
            //        MarkUp = $"{(gatewayOrder.Markup * 100).ToString(2)}%",
            //        MerchantName = gatewayOrder.MerchantName,
            //        Id = gatewayOrder.Id,
            //        Status = new UserStatementComponent().GetStatusStr(2, (int)gatewayOrder.Status, isZH),
            //        Timestamp = gatewayOrder.Timestamp.ToUnixTime().ToString(),
            //        Type = Resources.交易类型支付,
            //        RefundTimestamp = gatewayOrder.Status == GatewayOrderStatus.Refunded ? new RefundGatewayOrderDAC().GetByOrderId(gatewayOrder.Id)?.Timestamp.ToUnixTime().ToString() : "",
            //        OrderNo = gatewayOrder.OrderNo,
            //        CurrentExchangeRate = $"1 {coin.Code} = {cer.ToString(2)} {gatewayOrder.FiatCurrency}",
            //        IncreaseRate = iRate > 0 ? $"+{iRate.ToString(2)}" : iRate.ToString(2)
            //    };
            //}
            //else
            OrderDetailOM om;
            {
                var coin = new CryptocurrencyDAC().GetById(order.CryptoId);
                var er = order.ExchangeRate;
                var cer = GetExchangeRate(user.CountryId, order.FiatCurrency, coin);
                var iRate = ((cer - er) / er) * 100;
                om = new OrderDetailOM
                {
                    Code = coin.Code,
                    CryptoAmount = order.CryptoAmount.ToString(coin.DecimalPlace),
                    FiatAmount = order.FiatAmount.ToString(2),
                    ExchangeRate = $"1 {coin.Code} = {order.ExchangeRate.ToString(4)} {order.FiatCurrency}",
                    FiatCurrency = order.FiatCurrency,
                    MarkUp = $"{(order.Markup * 100).ToString(2)}%",
                    MerchantName = new MerchantAccountDAC().GetById(order.MerchantAccountId).MerchantName,
                    Id = order.Id,
                    Status = new UserStatementComponent().GetStatusStr(2, (int)order.Status, isZH),
                    Timestamp = order.PaymentTime.Value.ToUnixTime().ToString(),
                    Type = Resources.Payment,
                    RefundTimestamp = order.Status == OrderStatus.Refunded ? new RefundDAC().GetByOrderId(order.Id)?.Timestamp.ToUnixTime().ToString() : "",
                    OrderNo = order.OrderNo,
                    CurrentExchangeRate = coin.Enable == 1 ? $"1 {coin.Code} = {cer.ToString(4)} {order.FiatCurrency}" : "--",
                    IncreaseRate = coin.Enable == 1 ? (iRate > 0 ? $"+{iRate.ToString(2)}" : iRate.ToString(2)) : "--"
                };
            }

            return om;
        }

        public PaymentCodeDTO GetPaymentCode(UserAccount user)
        {
            DateTime currentTime = DateTime.UtcNow;

            string paymentCode = $"{Constant.PAYMENT_CODE_PREFIX}{string.Concat(HMACSHA512.Generate15(user.SecretKey, currentTime).Skip(1))}";

            var result = new PaymentCodeDTO
            {
                PaymentCode = paymentCode,
                ExpireTimestamp = currentTime.AddMinutes(Constant.PAYMENT_CODE_EXPIRE_MINUTE).ToUnixTime().ToString(),
                UserId = user.Id
            };

            RedisHelper.StringSet(
               Constant.REDIS_PAYMENT_CODE_DBINDEX,
               $"{Constant.REDIS_PAYMENT_CODE_PREFIX}{paymentCode}",
               JsonConvert.SerializeObject(result),
               TimeSpan.FromMinutes(Constant.PAYMENT_CODE_EXPIRE_MINUTE));

            return result;
        }
    }
}