using FiiiPay.Framework.Queue;
using Newtonsoft.Json;
using System;

namespace FiiiiPay.MQBussiness
{
    public class MainService
    {
        public void Start()
        {
            RabbitMQWorker.Start("MiningConfirmed", message =>
            {
                try
                {
                    var model = JsonConvert.DeserializeObject<MiningConfirmedModel>(message);
                    if (model == null)
                    {
                        LogHelper.Info($"MiningConfirmed message deserialize error,message:{message}");
                    }
                    new MiningComponent().MiningConfirmed(model);
                }
                catch (Exception exception)
                {
                    LogHelper.Error(exception.Message, exception);
                }
            });
            LogHelper.Info("FiiiPayMQBussiness process service started");
        }

        public void Stop()
        {
            RabbitMQWorker.Stop();
            LogHelper.Info("FiiiPayMQBussiness process service stopped.");
        }
    }
}
