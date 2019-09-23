using System;
using System.ServiceProcess;
using FiiiPay.Framework.Queue;
using log4net;
using log4net.Config;

namespace FiiiPay.MallPayment
{
    public partial class MainService : ServiceBase
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(MainService));
        private readonly PaymentProcess _process = new PaymentProcess();

        public MainService()
        {
            InitializeComponent();

            XmlConfigurator.Configure();
        }

        protected override void OnStart(string[] args)
        {
            StartService();
        }

        protected override void OnStop()
        {
            RabbitMQWorker.Stop();
        }
        
        public void StartService()
        {
            RabbitMQWorker.Start("PaymentGatewayPayOrder", Payment);
            RabbitMQWorker.Start("PaymentGatewayRefund", Refund);
            RabbitMQWorker.Start("PaymentNotification", Notification);

            _process.ReNotification();
        }

        public void Payment(string message)
        {
            _log.Info("ReciveMessage:PaymentGatewayPayOrder");
            try
            {
                _process.Payment(message);
            }
            catch (Framework.Exceptions.CommonException x)
            {
                _log.Error(x.ReasonCode, x);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        public void Refund(string message)
        {
            _log.Info("ReciveMessage:PaymentGatewayRefund");
            try
            {
                _process.Refund(message);
            }
            catch (Framework.Exceptions.CommonException x)
            {
                _log.Error(x.ReasonCode, x);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        public void Notification(string message)
        {
            _log.Info("ReciveMessage:PaymentNotification");
            try
            {
                _process.Notification(message);
            }
            catch (Framework.Exceptions.CommonException x)
            {
                _log.Error(x.ReasonCode, x);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
    }
}
