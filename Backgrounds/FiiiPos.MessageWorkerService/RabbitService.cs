using System;
using System.ServiceProcess;
using FiiiPay.Framework.Queue;
using log4net;
using log4net.Config;

namespace FiiiPos.MessageWorkerService
{
    public partial class RabbitService : ServiceBase
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(RabbitService));

        public RabbitService()
        {
            InitializeComponent();

            XmlConfigurator.Configure();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                #region 用于消息推送的消息队列

                RabbitMQWorker.Start("MerchantArticle", message =>
                {
                    _log.Info("ReciveMessage:MerchantArticle");
                    try
                    {
                        int.TryParse(message, out int id);
                        new FiiiPOSPushComponent().PushArticle(id);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("FiiiPos_BackOfficeRefundOrder", message =>
                {
                    _log.Info("ReciveMessage:FiiiPos_BackOfficeRefundOrder");
                    try
                    {
                        new FiiiPOSPushComponent().PushRefundOrder(message);
                    }
                    catch (FiiiPay.Framework.Exceptions.CommonException x)
                    {
                        _log.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                });

                RabbitMQWorker.Start("MerchantDepositCancel", message =>
                {
                    _log.Info("ReciveMessage:MerchantDepositCancel");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().PushDepositCancel(id);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("MerchantDeposit", message =>
                {
                    _log.Info("ReciveMessage:MerchantDeposit");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().PushDeposit(id);
                    }
                    catch (FiiiPay.Framework.Exceptions.CommonException x)
                    {
                        _log.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                });

                RabbitMQWorker.Start("MerchantInviteSuccessed", message =>
                {
                    _log.Info("ReciveMessage:MerchantInviteSuccessed");
                    try
                    {
                        long.TryParse(message, out long id);
                        _log.Info("message id " + id);
                        new FiiiPOSPushComponent().PushInviteSuccess(id);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("MerchantLv1VerifiedFailed", message =>
                {
                    _log.Info("ReciveMessage:MerchantLv1VerifiedFailed");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().MerchantLv1Reject(id);
                    }
                    catch (FiiiPay.Framework.Exceptions.CommonException x)
                    {
                        _log.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                });

                RabbitMQWorker.Start("MerchantLv1Verified", message =>
                {
                    _log.Info("ReciveMessage:MerchantLv1Verified");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().MerchantLv1Verified(id);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("MerchantLv2VerifiedFailed", message =>
                {
                    _log.Info("ReciveMessage:MerchantLv2VerifiedFailed");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().MerchantLv2Reject(id);
                    }
                    catch (FiiiPay.Framework.Exceptions.CommonException x)
                    {
                        _log.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                });

                RabbitMQWorker.Start("MerchantLv2Verified", message =>
                {
                    _log.Info("ReciveMessage:MerchantLv2Verified");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().MerchantLv2Verified(id);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("OrderPayed", message =>
                {
                    _log.Info("ReciveMessage:OrderPayed");
                    try
                    {
                        Guid.TryParse(message, out Guid orderId);
                        new FiiiPOSPushComponent().PushOrderPayed(orderId);
                    }
                    catch (FiiiPay.Framework.Exceptions.CommonException x)
                    {
                        _log.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                });

                RabbitMQWorker.Start("MerchantTransferFromEx", message =>
                {
                    _log.Info("ReciveMessage:MerchantTransferFromEx");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().PushTransferFromEx(id);
                    }
                    catch (FiiiPay.Framework.Exceptions.CommonException x)
                    {
                        _log.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                });

                RabbitMQWorker.Start("MerchantTransferToEx", message =>
                {
                    _log.Info("ReciveMessage:MerchantTransferToEx");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().PushTransferToEx(id);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("MerchantWithdrawCompleted", message =>
                {
                    _log.Info("ReciveMessage:MerchantWithdrawCompleted");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().PushWithdrawCompleted(id);
                    }
                    catch (FiiiPay.Framework.Exceptions.CommonException x)
                    {
                        _log.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                });

                RabbitMQWorker.Start("MerchantWithdrawReject", message =>
                {
                    _log.Info("ReciveMessage:MerchantWithdrawReject");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().PushWithdrawReject(id);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception.Message, exception);
                    }
                });
                RabbitMQWorker.Start("STORE_VERIFIED", message =>
                {
                    _log.Info("ReciveMessage:STORE_VERIFIED");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().PushStoreVerified(id);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception.Message, exception);
                    }
                });
                RabbitMQWorker.Start("STORE_REJECT", message =>
                {
                    _log.Info("ReciveMessage:STORE_REJECT");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPOSPushComponent().PushStoreReject(id);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception.Message, exception);
                    }
                });


                #endregion
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        protected override void OnStop()
        {
            RabbitMQWorker.Stop();
        }
    }
}
