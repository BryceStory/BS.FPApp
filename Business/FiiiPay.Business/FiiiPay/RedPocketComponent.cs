using FiiiPay.Business.Properties;
using FiiiPay.Data;
using FiiiPay.DTO;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Exceptions;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Transactions;

namespace FiiiPay.Business.FiiiPay
{
    public class RedPocketComponent
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(RedPocketComponent));

        private const int Argument_Error = 60000;
        private const int Push_MinAmount = 60001;
        private const int MaxError = 60002;
        private const int PassCodeError = 60003;
        private const int PassCodeExpired = 60004;

        private const int LockDbIndex = 2;
        private const string KeyFormat = "FiiiPay:RedPocket:{0}";

        private const int _expried = 24 * 60;

        public static decimal MaxAmount
        {
            get
            {
                var data = new MasterSettingDAC().Single("RedPocket", "RedPocket_AmountLimit");
                if (data == null) return 300;

                return decimal.Parse(data.Value);
            }
        }

        public static int MaxCount
        {
            get
            {
                var data = new MasterSettingDAC().Single("RedPocket", "RedPocket_CountLimit");
                if (data == null) return 100;

                return int.Parse(data.Value);
            }
        }

        public RedPocket Push(UserAccount userAccount, Guid accountId, int cryptoId, decimal amount, int count, string userPIN, string pin, string message = "")
        {
            if (userAccount.L1VerifyStatus != Entities.Enums.VerifyStatus.Certified)
            {
                throw new CommonException(ReasonCode.NOT_VERIFY_LV1, Resources.EMNeedLV1Verfied);
            }

            if (amount <= 0 || cryptoId <= 0)
            {
                throw new CommonException(Argument_Error, MessageResources.InvalidDataFormat);
            }
            if (count <= 0)
            {
                throw new CommonException(Argument_Error, MessageResources.InvalidDataFormat);
            }

            if (count > MaxCount)
            {
                throw new CommonException(Argument_Error, string.Format(MessageResources.RedPocket_MaxCount, MaxCount));
            }

            if (!string.IsNullOrWhiteSpace(message) && message.Length > 46)
            {
                throw new CommonException(Argument_Error, MessageResources.RedPocket_MaxMessage);
            }

            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, accountId.ToString(), userPIN, pin);

            var cryptoDAC = new CryptocurrencyDAC();
            var priceDAC = new PriceInfoDAC();

            var crypto = cryptoDAC.GetById(cryptoId);
            if (crypto == null)
            {
                throw new SystemErrorException();
            }

            if (!crypto.Status.HasFlag(Foundation.Entities.Enum.CryptoStatus.RedPocket) || crypto.Enable == 0)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, MessageResources.CurrencyForbidden);

            var min = crypto.DecimalPlace == 8 ? 0.00000001M : crypto.DecimalPlace == 6 ? 0.000001M : 0.00000001M;

            var minAmount = count * min;
            if (amount < minAmount)
            {
                throw new CommonException(Push_MinAmount, string.Format(MessageResources.RedPocket_MinAmount, minAmount, crypto.Code));
            }

            var price = priceDAC.GetPriceByName("USD", crypto.Code);

            var max = crypto.DecimalPlace == 8 ? 999999999.999999999M : crypto.DecimalPlace == 6 ? 999999999.999999M : 999999999.999999999M;
            var maxAmount = Math.Round(MaxAmount / price, crypto.DecimalPlace);

            if (count > 1)
            {
                maxAmount = Math.Round(MaxAmount * count / price, crypto.DecimalPlace);
            }

            if (amount >= max || Math.Round(amount, crypto.DecimalPlace) > maxAmount)
            {
                throw new CommonException(Argument_Error, string.Format(MessageResources.RedPocket_MaxAmount, maxAmount, crypto.Code));
            }

            var userWalletDAC = new UserWalletDAC();
            var redPocketDAC = new RedPocketDAC();
            var userWalletStatementDAC = new UserWalletStatementDAC();
            var userTransactionDAC = new UserTransactionDAC();

            var wallet = userWalletDAC.GetByAccountId(accountId, cryptoId);
            if (wallet == null || wallet.Balance < amount)
            {
                throw new CommonException(ReasonCode.INSUFFICIENT_BALANCE, MessageResources.InsufficientBalance);
            }

            var fiatAmount = price * amount;
            using (var scope = new TransactionScope())
            {
                try
                {
                    var redPocket = new RedPocket
                    {
                        AccountId = accountId,
                        Status = RedPocketStatus.Actived,
                        PassCode = GeneralPassCode(),
                        CryptoCode = crypto.Code,
                        Amount = amount,
                        Balance = amount,
                        Count = count,
                        RemainCount = count,
                        Message = message,
                        Timestamp = DateTime.UtcNow,
                        ExpirationDate = DateTime.UtcNow.AddMinutes(_expried),
                        CryptoId = cryptoId,
                        OrderNo = IdentityHelper.OrderNo(),
                        FiatAmount = fiatAmount <= 0 ? 0M : Math.Round(fiatAmount, 8)
                    };

                    var redPocketId = redPocketDAC.Insert(redPocket);
                    userTransactionDAC.Insert(new UserTransaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = accountId,
                        CryptoId = redPocket.CryptoId,
                        CryptoCode = redPocket.CryptoCode,
                        Type = UserTransactionType.PushRedPocket,
                        DetailId = redPocketId.ToString(),
                        Status = (byte)redPocket.Status,
                        Timestamp = redPocket.Timestamp,
                        Amount = redPocket.Amount,
                        OrderNo = redPocket.OrderNo
                    });

                    userWalletDAC.Decrease(wallet.Id, amount);
                    userWalletStatementDAC.Insert(new UserWalletStatement
                    {
                        WalletId = wallet.Id,
                        Balance = wallet.Balance - amount,
                        Amount = 0 - amount,
                        FrozenAmount = 0,
                        FrozenBalance = wallet.FrozenBalance,
                        Action = "Push Red Pocket",
                        Timestamp = DateTime.UtcNow
                    });

                    QueueHelper.DelaySender.Send("FiiiPay_RedPocket", redPocketId);

                    scope.Complete();

                    return redPocket;
                }
                catch (Exception ex)
                {
                    _log.Error(ex);

                    throw new SystemErrorException();
                }
            }
        }

        public RedPocket RePush(Guid accountId, long pocketId)
        {
            var redPocketDAC = new RedPocketDAC();
            var pocket = redPocketDAC.GetById(pocketId);

            if (pocket == null) throw new SystemErrorException();

            var crypto = new CryptocurrencyDAC().GetById(pocket.CryptoId);
            if (!crypto.Status.HasFlag(Foundation.Entities.Enum.CryptoStatus.RedPocket) || crypto.Enable == 0)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, MessageResources.CurrencyForbidden);

            if (pocket.ExpirationDate > DateTime.UtcNow && pocket.AccountId == accountId && pocket.Status == RedPocketStatus.Actived)
            {
                return pocket;
            }

            throw new SystemErrorException();
        }

        public RedPocketDetailOM Receive(UserAccount userAccount, string passcode, bool isZH = false)
        {
            if (userAccount == null) throw new SystemErrorException();

            if (string.IsNullOrWhiteSpace(passcode))
            {
                throw new CommonException(Argument_Error, MessageResources.InvalidDataFormat);
            }

            if (userAccount.L1VerifyStatus != Entities.Enums.VerifyStatus.Certified)
            {
                throw new CommonException(ReasonCode.NOT_VERIFY_LV1, Resources.EMNeedLV1Verfied);
            }

            var count = RedisHelper.StringGet(LockDbIndex, string.Format(KeyFormat, userAccount.Id));
            if (!string.IsNullOrWhiteSpace(count) && int.Parse(count) >= 10)
            {
                var ttl = RedisHelper.KeyTimeToLive(LockDbIndex, string.Format(KeyFormat, userAccount.Id));
                if (ttl != null)
                {
                    var message = "";
                    var t = "";
                    try
                    {
                        t = TimeConvert(ttl.Value, isZH);
                        message = string.Format(MessageResources.RedPocket_PassCodeErrorMaxCount, t);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception.Message + "    " + MessageResources.RedPocket_PassCodeErrorMaxCount + "    " + t);
                    }

                    throw new CommonException(MaxError, message);
                }
            }

            var redPocketDAC = new RedPocketDAC();

            var redPocket = redPocketDAC.GetByPassCode(passcode);
            if (redPocket == null)
            {
                var errorCount = ErrorCount(userAccount.Id, count, isZH);
                throw new CommonException(PassCodeError, string.Format(MessageResources.RedPocket_PassCodeError, errorCount, 10 - errorCount));
            }

            if (redPocket.ExpirationDate < DateTime.UtcNow)
            {
                var errorCount = ErrorCount(userAccount.Id, count, isZH);
                throw new CommonException(PassCodeExpired, MessageResources.RedPocket_ReceiveExpired + Environment.NewLine + string.Format(MessageResources.RedPocket_PassCodeError, errorCount, 10 - errorCount));
            }

            var crypto = new CryptocurrencyDAC().GetByCode(redPocket.CryptoCode);
            if (!crypto.Status.HasFlag(Foundation.Entities.Enum.CryptoStatus.RedPocket) || crypto.Enable == 0)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, MessageResources.CurrencyForbidden);

            var om = new RedPocketDetailOM();
            var redPocketReceiveDAC = new RedPocketReceiverDAC();

            var hasReceive = redPocketReceiveDAC.HasReceive(userAccount.Id, redPocket.Id);

            if (redPocket.Status == RedPocketStatus.Actived && hasReceive == null)
            {
                var userWalletDAC = new UserWalletDAC();
                var userWalletStatementDAC = new UserWalletStatementDAC();
                var uwComponent = new UserWalletComponent();
                var userTransactionDAC = new UserTransactionDAC();

                var wallet = userWalletDAC.GetByCryptoCode(userAccount.Id, redPocket.CryptoCode);
                if (wallet == null)
                    wallet = uwComponent.GenerateWallet(userAccount.Id, redPocket.CryptoCode);

                var min = crypto.DecimalPlace == 8 ? 0.00000001M : crypto.DecimalPlace == 6 ? 0.000001M : 0.00000001M;
                var n = crypto.DecimalPlace == 8 ? 100000000 : crypto.DecimalPlace == 6 ? 1000000 : 100000000;

                var amount = GetRandomMoney(redPocket, min, n);

                var priceDAC = new PriceInfoDAC();
                var price = priceDAC.GetPriceByName("USD", crypto.Code);
                var fiatAmount = price * amount;
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        var redPocketReceiver = new RedPocketReceiver
                        {
                            PocketId = redPocket.Id,
                            AccountId = userAccount.Id,
                            SendAccountId = redPocket.AccountId,
                            CryptoCode = redPocket.CryptoCode,
                            Amount = amount,
                            Timestamp = DateTime.UtcNow,
                            IsBestLuck = redPocket.Count == 1,
                            OrderNo = IdentityHelper.OrderNo(),
                            FiatAmount = fiatAmount <= 0 ? 0M : Math.Round(fiatAmount, 8)
                        };
                        var id = redPocketReceiveDAC.Insert(redPocketReceiver);

                        userTransactionDAC.Insert(new UserTransaction
                        {
                            Id = Guid.NewGuid(),
                            AccountId = redPocketReceiver.AccountId,
                            CryptoId = redPocket.CryptoId,
                            CryptoCode = redPocket.CryptoCode,
                            Type = UserTransactionType.ReceiveRedPocket,
                            DetailId = id.ToString(),
                            Status = (byte)redPocket.Status,
                            Timestamp = redPocketReceiver.Timestamp,
                            Amount = redPocketReceiver.Amount,
                            OrderNo = redPocketReceiver.OrderNo
                        });

                        userWalletDAC.Increase(wallet.Id, amount);
                        userWalletStatementDAC.Insert(new UserWalletStatement
                        {
                            WalletId = wallet.Id,
                            Balance = wallet.Balance + amount,
                            Amount = amount,
                            FrozenAmount = 0,
                            FrozenBalance = wallet.FrozenBalance,
                            Action = "Receive Red Pocket",
                            Timestamp = DateTime.UtcNow
                        });

                        redPocketDAC.UpdateRemain(redPocket.Id, amount);

                        if (redPocket.RemainCount == 0)
                        {
                            redPocketDAC.UpdateStatus(redPocket.Id, RedPocketStatus.Complate);
                            userTransactionDAC.UpdateStatus(UserTransactionType.PushRedPocket, redPocket.Id.ToString(), redPocket.AccountId, (byte)RedPocketStatus.Complate);

                            if (redPocket.Count > 1)
                            {
                                redPocketReceiveDAC.UpdateBestLuck(redPocket.Id);
                            }
                        }

                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                        throw new CommonException();
                    }
                }
                om.SelfAmount = amount.ToString();
                om.ReceiveStatus = ReceiveStatusEnum.Receive;
                om.HasExpried = false;
                redPocket.Balance -= amount;
            }
            else
            {
                if (hasReceive != null)
                {
                    om.ReceiveStatus = ReceiveStatusEnum.HasReceive;
                    om.SelfAmount = hasReceive.Amount.ToString();
                }
                else
                {
                    om.ReceiveStatus = ReceiveStatusEnum.None;
                    om.SelfAmount = string.Empty;
                }
            }

            var account = new UserAccountComponent().GetById(redPocket.AccountId);

            om.Message = redPocket.Message;
            om.SnederNickname = account.Nickname;

            om.TotalAmount = redPocket.Amount.ToString();
            om.TotalCount = redPocket.Count;
            om.CryptoCode = redPocket.CryptoCode;
            om.ReceiveAmount = (redPocket.Amount - redPocket.Balance).ToString();
            om.ReceiveCount = redPocket.Count - redPocket.RemainCount;
            om.Id = redPocket.Id;
            om.HasExpried = redPocket.ExpirationDate < DateTime.UtcNow;
            om.HasSelfSned = redPocket.AccountId == userAccount.Id;

            return om;
        }

        public RedPocketDetailOM Detail(Guid accountId, long pocketId)
        {
            var redPocketDAC = new RedPocketDAC();

            var redPocket = redPocketDAC.GetById(pocketId);
            if (redPocket == null)
            {
                throw new SystemErrorException();
            }

            var om = new RedPocketDetailOM();

            //if (redPocket.ExpirationDate < DateTime.UtcNow)
            //{
            //    if (accountId != redPocket.AccountId)
            //    {
            //        throw new CommonException(Expired, MessageResources.系统错误);
            //    }
            //}

            var account = new UserAccountComponent().GetById(redPocket.AccountId);

            om.Message = redPocket.Message;
            om.SnederNickname = account.Nickname;

            om.TotalAmount = redPocket.Amount.ToString();
            om.TotalCount = redPocket.Count;
            om.ReceiveAmount = (redPocket.Amount - redPocket.Balance).ToString();
            om.ReceiveCount = redPocket.Count - redPocket.RemainCount;
            om.HasSelfSned = redPocket.AccountId == accountId;
            om.CryptoCode = redPocket.CryptoCode;
            var redPocketReceiveDAC = new RedPocketReceiverDAC();
            var hasReceive = redPocketReceiveDAC.HasReceive(accountId, redPocket.Id);
            om.ReceiveStatus = hasReceive != null ? ReceiveStatusEnum.HasReceive : ReceiveStatusEnum.None;
            om.SelfAmount = hasReceive != null ? hasReceive.Amount.ToString() : "";
            om.HasExpried = redPocket.Status == RedPocketStatus.Refund || redPocket.ExpirationDate < DateTime.UtcNow;
            if (redPocket.AccountId == accountId)
            {
                om.PassCode = redPocket.PassCode.ToUpper();
                om.ExpirationDate = redPocket.ExpirationDate.ToUtcTimeTicks().ToString();
            }

            if (hasReceive != null)
            {
                om.FiatAmount = ConvertFiatAmount(hasReceive.FiatAmount, account.FiatCurrency).ToString();
            }

            return om;
        }

        private static decimal ConvertFiatAmount(decimal fiatAmountUSD, string currency)
        {
            if (fiatAmountUSD <= 0) return 0M;

            var price = new PriceInfoDAC().GetPriceByName("USD", "USDT");
            if (price == 0) price = 0.01M;
            var usdt = fiatAmountUSD / price;

            var cprice = new PriceInfoDAC().GetPriceByName(currency, "USDT");

            var result = Math.Round(cprice * usdt, 2);

            return result <= 0 ? 0.01M : result;
        }

        public List<RedPocketListDetailOM> DetailList(long redPocketId, int index, int size)
        {
            if (index < 0)
                throw new CommonException(Argument_Error, MessageResources.InvalidDataFormat);

            if (size > 20)
                size = 20;

            var redPocketReceiveDAC = new RedPocketReceiverDAC();

            var list = redPocketReceiveDAC.GetList(redPocketId, index, size).Select(s => new RedPocketListDetailOM
            {
                Amount = s.Amount.ToString(),
                CryptoCode = s.CryptoCode,
                IsBestLuck = s.IsBestLuck,
                Nickname = s.Nickname,
                Timestamp = s.Timestamp.ToUtcTimeTicks().ToString()
            });

            var om = list.ToList();

            return om;
        }

        public PushRedPocketListOM PushList(Guid accountId, string fiatCurrency, int index, int size)
        {
            if (index < 0)
                throw new CommonException(Argument_Error, MessageResources.InvalidDataFormat);

            if (size > 20)
                size = 20;

            var om = new PushRedPocketListOM();
            var redPocketDAC = new RedPocketDAC();

            var list = redPocketDAC.GetList(accountId, index, size).Select(s => new PushRedPockDetail
            {
                Amount = s.Amount.ToString(),
                CryptoCode = s.CryptoCode,
                RedPocketId = s.Id,
                RefundAmount = s.RefundAmount.ToString(),
                Status = (byte)s.Status,
                Timestamp = s.Timestamp.ToUtcTimeTicks().ToString()
            });

            om.Total = redPocketDAC.PushCount(accountId);
            om.TotalFiatAmount = ConvertFiatAmount(redPocketDAC.AccountSum(accountId), fiatCurrency).ToString();
            om.DetailList = list.ToList();

            return om;
        }

        public RedPocketReceiveListOM ReceiveList(Guid accountId, string fiatCurrency, int index, int size)
        {
            if (index < 0)
                throw new CommonException(Argument_Error, MessageResources.InvalidDataFormat);

            if (size > 20)
                size = 20;

            var om = new RedPocketReceiveListOM();
            var redPocketReceiveDAC = new RedPocketReceiverDAC();

            var list = redPocketReceiveDAC.GetList(accountId, index, size).Select(s => new ReceiveRedPockDetail
            {
                Amount = s.Amount.ToString(CultureInfo.InvariantCulture),
                CryptoCode = s.CryptoCode,
                RedPocketId = s.PocketId,
                Nickname = s.Nickname,
                Timestamp = s.Timestamp.ToUtcTimeTicks().ToString(),
            });

            om.DetailList = list.ToList();
            om.TotalFiatAmount = ConvertFiatAmount(redPocketReceiveDAC.AccountSum(accountId), fiatCurrency).ToString();
            om.Total = redPocketReceiveDAC.ReceiveCount(accountId);

            return om;
        }

        public StatementDetailOM StatementDetail(Guid accountId, int type, long id)
        {
            if (type == 14)
            {
                var redPocketDAC = new RedPocketDAC();
                var data = redPocketDAC.GetById(id);

                if (data == null) throw new SystemErrorException();

                if (accountId != data.AccountId) throw new SystemErrorException();

                var status = data.Status;
                if (status == RedPocketStatus.FullRefund)
                    status = RedPocketStatus.Refund;

                var result = new StatementDetailOM
                {
                    Status = (byte)status,
                    Amount = data.Amount.ToString(),
                    CryptoCode = data.CryptoCode,
                    OrderNo = data.OrderNo,
                    PocketId = data.Id,
                    Timestamp = data.Timestamp.ToUtcTimeTicks().ToString(),
                    Type = 14,
                    HasRefund = false
                };

                if (data.Status == RedPocketStatus.Refund)
                {
                    var redPocketRefundDAC = new RedPocketRefundDAC();
                    var refund = redPocketRefundDAC.GetById(id);

                    result.HasRefund = true;
                    result.RefundAmount = refund.Amount.ToString();
                    result.RefundTimestamp = refund.Timestamp.ToUtcTimeTicks().ToString();
                }

                return result;
            }

            if (type == 15)
            {
                var redPocketReceiveDAC = new RedPocketReceiverDAC();
                var data = redPocketReceiveDAC.GetById(accountId, id);

                if (data == null) throw new SystemErrorException();

                return new StatementDetailOM
                {
                    Status = 2,
                    Amount = data.Amount.ToString(),
                    CryptoCode = data.CryptoCode,
                    OrderNo = data.OrderNo,
                    PocketId = data.PocketId,
                    HasRefund = false,
                    RefundAmount = "",
                    RefundTimestamp = "",
                    Timestamp = data.Timestamp.ToUtcTimeTicks().ToString(),
                    Type = 15
                };
            }

            return null;
        }

        private static decimal GetRandomMoney(RedPocket redPackage, decimal min, int n)
        {
            // RemainCount 剩余的红包数量
            // Balance 剩余的钱
            if (redPackage.RemainCount <= 0) return 0;

            if (redPackage.RemainCount == 1)
            {
                redPackage.RemainCount--;
                return Math.Round(redPackage.Balance * n) / n;
            }

            var r = new Random();
            var max = redPackage.Balance / redPackage.RemainCount * 2;
            var money = (decimal)r.NextDouble() * max;
            money = money <= min ? min : money;
            money = Math.Floor(money * n) / n;
            redPackage.RemainCount--;
            redPackage.Balance -= money;
            return money;
        }

        //private static bool IsNumber(string s, int precision, int scale)
        //{
        //    if ((precision == 0) && (scale == 0))
        //    {
        //        return false;
        //    }
        //    string pattern = @"(^\d{1," + precision + "}";
        //    if (scale > 0)
        //    {
        //        pattern += @"\.\d{0," + scale + "}$)|" + pattern;
        //    }
        //    pattern += "$)";
        //    return Regex.IsMatch(s, pattern);
        //}

        private static string GeneralPassCode(int n = 8)
        {
            var val = "";
            var random = new Random();
            for (var i = 0; i < n; i++)
            {
                var str = random.Next(2) % 2 == 0 ? "num" : "char";
                if ("char".Equals(str, StringComparison.CurrentCultureIgnoreCase))
                {
                    var nextInt = 0;
                    do
                    {
                        nextInt = random.Next(2) % 2 == 0 ? 65 : 97;
                    } while (nextInt == 73 || nextInt == 79);
                    // 产生字母
                    // System.out.println(nextInt + "!!!!"); 1,0,1,1,1,0,0
                    val += (char)(nextInt + random.Next(26));
                }
                else if ("num".Equals(str, StringComparison.CurrentCultureIgnoreCase))
                {
                    // 产生数字
                    var rn = 0;
                    do
                    {
                        rn = random.Next(10);
                    } while (rn == 0 || rn == 1);

                    val += rn;
                }
            }
            return val.ToUpper();
        }

        private static int ErrorCount(Guid accountId, string count, bool isZH)
        {
            int errorCount;
            if (string.IsNullOrWhiteSpace(count))
            {
                errorCount = 1;
            }
            else
            {
                int.TryParse(count, out var iCount);
                errorCount = iCount + 1;
            }
            RedisHelper.StringSet(LockDbIndex, string.Format(KeyFormat, accountId), errorCount.ToString(), new TimeSpan(6, 0, 0));

            if (errorCount >= 10)
            {
                var ttl = RedisHelper.KeyTimeToLive(LockDbIndex, string.Format(KeyFormat, accountId));
                if (ttl != null)
                {
                    var t = TimeConvert(ttl.Value, isZH);
                    var message = string.Format(MessageResources.RedPocket_PassCodeErrorMaxCount, t);
                    throw new CommonException(MaxError, message);
                }
            }

            return errorCount;
        }

        private static string TimeConvert(TimeSpan timeSpan, bool isZH)
        {
            var str = new StringBuilder();

            if (timeSpan.Hours >= 1)
            {
                str.Append(isZH ? timeSpan.Hours + "小时" : timeSpan.Hours + " hours");
            }

            if (timeSpan.Minutes >= 1)
            {
                var s = timeSpan.Hours > 0 ? " and " : "";
                str.Append(isZH ? (timeSpan.Minutes == 0 ? 1 : timeSpan.Minutes) + "分钟" : s + timeSpan.Minutes + " minutes");
            }

            return str.ToString();
        }
    }
}
