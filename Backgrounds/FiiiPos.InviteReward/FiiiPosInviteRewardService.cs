using FiiiPay.Foundation.Data;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Queue;
using log4net;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.ServiceProcess;

namespace FiiiPos.InviteReward
{
    public partial class FiiiPosInviteRewardService : ServiceBase
    {
        public FiiiPosInviteRewardService()
        {
            InitializeComponent();
        }

        private static int cryptoId = 0;
        private readonly ILog _logger = LogManager.GetLogger("LogicError");

        protected override void OnStart(string[] args)
        {
            try
            {
                LogHelper.Config();

                RabbitMQWorker.Start("FiiiPosInviteReward", (msg) =>
                {
                    LogHelper.Info($"Received MQ message:{msg}");
                    FiiiPayRewardMessage message = JsonConvert.DeserializeObject<FiiiPayRewardMessage>(msg);
                    try
                    {
                        if (cryptoId == 0)
                        {
                            var fiiicoin = new CryptocurrencyDAC().GetByCode("FIII");
                            cryptoId = fiiicoin.Id;
                        }
                        new InviteReward().DistributeRewards(message, cryptoId);
                    }
                    catch (CommonException cex)
                    {
                        LogHelper.Info($"DistributeRewards faild, error message:{cex.Message}, param:{msg}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Info($"InviteFaild - ErrorMessage:{ex.Message}, param:{msg}");
                        throw ex;
                    }
                });

                LogHelper.Info("FiiiPos.InviteReward process service started.");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        protected override void OnStop()
        {
            LogHelper.Info("FiiiPos.InviteReward process service stopped.");
        }
    }
}
