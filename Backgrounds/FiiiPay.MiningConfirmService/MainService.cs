using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using FiiiPay.Framework.Queue;
using FiiiPay.MiningConfirmService.Components;
using FiiiPay.MiningConfirmService.Models;
using log4net;
using log4net.Config;
using Newtonsoft.Json;

namespace FiiiPay.MiningConfirmService
{
    public partial class MainService : ServiceBase
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(MainService));

        public MainService()
        {
            InitializeComponent();

            XmlConfigurator.Configure();
        }

        protected override void OnStart(string[] args)
        {
            RabbitMQWorker.Start("MiningConfirmed", message =>
            {
                try
                {
                    var model = JsonConvert.DeserializeObject<MiningConfirmedModel>(message);
                    if (model == null)
                    {
                        _log.Info($"MiningConfirmed message deserialize error,message:{message}");
                    }
                    new MiningComponent().MiningConfirmed(model);
                }
                catch (Exception exception)
                {
                    _log.Error(exception.Message, exception);
                }
            });
            _log.Info("FiiiPayMQBussiness process service started");
        }

        protected override void OnStop()
        {
            RabbitMQWorker.Stop();

            _log.Info("FiiiPayMQBussiness process service stopped.");
        }
    }
}
