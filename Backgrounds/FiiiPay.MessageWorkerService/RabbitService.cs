using FiiiPay.Framework.Queue;
using Newtonsoft.Json;
using System;
using System.ServiceProcess;
using log4net.Config;

namespace FiiiPay.MessageWorkerService
{
    public partial class RabbitService : ServiceBase
    {
        public RabbitService()
        {
            InitializeComponent();

            XmlConfigurator.Configure();
        }

        public void Start()
        {
            try
            {
                #region 用于跨系统访问的消息队列

                RabbitMQWorker.Start("Gateway_FiiiPay_ScanOrder", message =>
                {
                    try
                    {
                        new GatewayOrderComponent().GatewayPay(message);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                RabbitMQWorker.Start("Gateway_FiiiPay_Refund", message =>
                {
                    try
                    {
                        new GatewayOrderComponent().GatewayRefund(message);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                #endregion


                RabbitMQWorker.StartNack("WithdrawSubmit", message =>
                {
                    LogHelper.Info($"ReciveMessage:WithdrawSubmit,message:{message}");
                    try
                    {
                        var model = JsonConvert.DeserializeObject<CreateWithdrawModel>(message);
                        new WithdrawSubmitComponent().SubmitWithdrawal(model);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                        throw exception;
                    }
                });

                RabbitMQWorker.Start("FiiiPay_RedPocket-delay", message =>
                {
                    LogHelper.Info($"ReciveMessage:RedPocket,message:{message}");
                    try
                    {
                        long.TryParse(message, out long id);
                        new RedPocketComponent().Refund(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                        throw exception;
                    }
                });

                #region 用于消息推送的消息队列

                RabbitMQWorker.Start("UserArticle", message =>
                {
                    LogHelper.Info("ReciveMessage:UserArticle");
                    try
                    {
                        int.TryParse(message, out int id);
                        new FiiiPayPushComponent().PushArticle(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("FiiiPay_BackOfficeRefundOrder", message =>
                {
                    LogHelper.Info("ReciveMessage:FiiiPay_BackOfficeRefundOrder");
                    try
                    {
                        new FiiiPayPushComponent().BackOfficeRefundOrder(message);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                RabbitMQWorker.Start("UserDepositCancel", message =>
                {
                    LogHelper.Info("ReciveMessage:UserDepositCancel");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPayPushComponent().PushDepositCancel(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("UserDeposit", message =>
                {
                    LogHelper.Info("ReciveMessage:UserDeposit");
                    try
                    {
                        var id = long.Parse(message);
                        new FiiiPayPushComponent().PushDeposit(id);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                RabbitMQWorker.Start("UserInviteSuccessed", message =>
                {
                    LogHelper.Info("ReciveMessage:UserInviteSuccessed");
                    try
                    {
                        long.TryParse(message, out long id);
                        LogHelper.Info("message id " + id);
                        new FiiiPayPushComponent().PushInviteSuccess(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("UserKYC_LV1_REJECT", message =>
                {
                    LogHelper.Info("ReciveMessage:UserKYC_LV1_REJECT");
                    try
                    {
                        var id = long.Parse(message);
                        new FiiiPayPushComponent().PushKYC_LV1_REJECT(id);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                RabbitMQWorker.Start("UserKYC_LV1_VERIFIED", message =>
                {
                    LogHelper.Info("ReciveMessage:UserKYC_LV1_VERIFIED");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPayPushComponent().PushKYC_LV1_VERIFIED(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("UserKYC_LV2_REJECT", message =>
                {
                    LogHelper.Info("ReciveMessage:UserKYC_LV2_REJECT");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPayPushComponent().PushKYC_LV2_REJECT(id);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                RabbitMQWorker.Start("ConsumeOrder", message =>
                {
                    LogHelper.Info("ReciveMessage:ConsumeOrder");
                    try
                    {
                        new FiiiPayPushComponent().PushConsume(message);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                RabbitMQWorker.Start("UserKYC_LV2_VERIFIED", message =>
                {
                    LogHelper.Info("ReciveMessage:UserKYC_LV2_VERIFIED");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPayPushComponent().PushKYC_LV2_VERIFIED(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("PayOrder", message =>
                {
                    LogHelper.Info("ReciveMessage:PayOrder");
                    try
                    {
                        new FiiiPayPushComponent().PushPayOrder(message);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                RabbitMQWorker.Start("RefundOrder", message =>
                {
                    LogHelper.Info("ReciveMessage:RefundOrder");
                    try
                    {
                        new FiiiPayPushComponent().RefundOrder(message);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("UserTransferFromEx", message =>
                {
                    LogHelper.Info("ReciveMessage:UserTransferFromEx");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPayPushComponent().PushTransferFromEx(id);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                RabbitMQWorker.Start("UserTransferToEx", message =>
                {
                    LogHelper.Info("ReciveMessage:UserTransferToEx");
                    try
                    {
                        long.TryParse(message, out long id);
                        LogHelper.Info("message id " + id);
                        new FiiiPayPushComponent().PushTransferToEx(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("UserWithdrawCompleted", message =>
                {
                    LogHelper.Info("ReciveMessage:UserWithdrawCompleted");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPayPushComponent().PushWithdrawCompleted(id);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                RabbitMQWorker.Start("UserWithdrawReject", message =>
                {
                    LogHelper.Info("ReciveMessage:UserWithdrawReject");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPayPushComponent().PushWithdrawReject(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("UserTransferOutFiiiPay", message =>
                {
                    LogHelper.Info("ReciveMessage:UserTransferOutFiiiPay");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPayPushComponent().PushTransferOut(id);
                    }
                    catch (Framework.Exceptions.CommonException x)
                    {
                        LogHelper.Error(x.ReasonCode, x);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                });

                RabbitMQWorker.Start("UserTransferIntoFiiiPay", message =>
                {
                    LogHelper.Info("ReciveMessage:UserTransferIntoFiiiPay");
                    try
                    {
                        long.TryParse(message, out long id);
                        new FiiiPayPushComponent().PushTransferInto(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });
                RabbitMQWorker.Start("UnBindingAccount", message =>
                {
                    LogHelper.Info("ReciveMessage:UnBindingAccount");
                    try
                    {
                        var data = JsonConvert.DeserializeObject<Tuple<Guid, long>>(message);

                        new FiiiPayPushComponent().UnBindingFiiipos(data);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("InvitePosBindSuccess", message =>
                {
                    LogHelper.Info("ReciveMessage:InvitePosBindSuccess");
                    try
                    {
                        var data = JsonConvert.DeserializeObject<Tuple<Guid, long>>(message);

                        new FiiiPayPushComponent().InviteFiiiposSuccess(data);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("ShopPayment", message =>
                {
                    LogHelper.Info("ReciveMessage:ShopPayment");
                    try
                    {
                        Guid.TryParse(message, out Guid id);

                        new FiiiPayPushComponent().ShopPayment(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("ShopPaymentRefund", message =>
                {
                    LogHelper.Info("ReciveMessage:ShopPaymentRefund");
                    try
                    {
                        Guid.TryParse(message, out Guid id);

                        new FiiiPayPushComponent().ShopPaymentRefund(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("Biller", message =>
                {
                    LogHelper.Info("ReciveMessage:Biller");
                    try
                    {
                        Guid.TryParse(message, out Guid id);

                        new FiiiPayPushComponent().PushBiller(id);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("StoreOrderPayed", message =>
                {
                    LogHelper.Info("ReciveMessage:StoreOrderPayed,Message:" + message);
                    try
                    {
                        var messageEntity = JsonConvert.DeserializeObject<StoreOrderMessage>(message);

                        new FiiiPayPushComponent().StoreOrderPayed(messageEntity);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                RabbitMQWorker.Start("FiiipayMerchantProfileVerified", message =>
                {
                    LogHelper.Info("ReciveMessage:FiiipayMerchantProfileVerified,Message:" + message);
                    try
                    {
                        var messageEntity = JsonConvert.DeserializeObject<FiiiPayMerchantProfileVerified>(message);

                        new FiiiPayPushComponent().FiiipayMerchantProfileVerified(messageEntity);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Error(exception.Message, exception);
                    }
                });

                #endregion

                LogHelper.Info("FiiiPay.Background.GatewayMQPull process service started.");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        protected override void OnStart(string[] args)
        {
            Start();

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            RabbitMQWorker.Stop();
            LogHelper.Info("FiiiPay.Background.GatewayMQPull process service stopped.");
            base.OnStop();
        }
    }
}
