using System;
using System.Transactions;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework;
using log4net;

namespace FiiiPay.MessageWorkerService
{
    internal class RedPocketComponent
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RedPocketComponent));

        private static readonly Identity _identity = new Identity(56);
        private static readonly RedPocketDAC _redPocketDac = new RedPocketDAC();
        private static readonly RedPocketRefundDAC _refundDac = new RedPocketRefundDAC();

        public void Refund(long id)
        {
            var redPocket = _redPocketDac.GetById(id);
            if (redPocket == null)
            {
                _log.Error($"Red pocket is not found. {id}");
                return;
            }

            if (redPocket.RemainCount > 0)
            {
                var userWalletDAC = new UserWalletDAC();
                var userWalletStatementDAC = new UserWalletStatementDAC();
                var userTransactionDAC = new UserTransactionDAC();

                var wallet = userWalletDAC.GetByAccountId(redPocket.AccountId, redPocket.CryptoId);
                if (wallet == null)
                {
                    _log.Error($"Wallet exception. {redPocket.AccountId} RedPocketId={id}");
                    return;
                }

                using (var scope = new TransactionScope())
                {
                    userWalletDAC.Increase(wallet.Id, redPocket.Balance);
                    userWalletStatementDAC.Insert(new UserWalletStatement
                    {
                        WalletId = wallet.Id,
                        Balance = wallet.Balance + redPocket.Balance,
                        Amount = redPocket.Balance,
                        FrozenAmount = 0,
                        FrozenBalance = wallet.FrozenBalance,
                        Action = "Refund Red Pocket",
                        Timestamp = DateTime.UtcNow
                    });

                    _refundDac.Insert(new RedPocketRefund
                    {
                        Id = redPocket.Id,
                        Amount = redPocket.Balance,
                        OrderNo = _identity.StringId(),
                        Timestamp = DateTime.UtcNow
                    });

                    _redPocketDac.UpdateStatus(redPocket.Id, redPocket.RemainCount == redPocket.Count ? RedPocketStatus.FullRefund : RedPocketStatus.Refund);

                    userTransactionDAC.UpdateStatus(UserTransactionType.PushRedPocket, redPocket.Id.ToString(), redPocket.AccountId, (byte)RedPocketStatus.Refund);

                    scope.Complete();
                }
            }
        }
    }
}
