using FiiiPay.Foundation.Data;
using FiiiPay.Framework.Exceptions;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FiiiPos.InviteReward.ComplementData
{
    public class InviteStart
    {
        private readonly ILog _logger = LogManager.GetLogger("LogicError");

        private Tuple<int, int, int> RunData = Tuple.Create(0, 0, 0);

        public void Invite(DateTime startTime, DateTime endTime, DateTime saveTime, Action<Tuple<int, int, int>> action)
        {
            List<FiiiPayRewardMessage> oList;
            try
            {
                oList = new TempDataDAC().GetMessageList(startTime, endTime);

                if (oList == null || oList.Count <= 0)
                {
                    LogHelper.Info("没有读取到数据");
                    return;
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error(ex);
                LogHelper.Info("读取数据错误");
                return;
            }
            //List<FiiiPayRewardMessage> oList = new List<FiiiPayRewardMessage>();
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());
            //oList.Add(new FiiiPayRewardMessage());

            int i = 0, j = 0;

            //RunData = Tuple.Create(oList.Count, i, j);
            LogHelper.Info($"读取到{oList.Count}条数据");

            //action(RunData);

            foreach (var message in oList)
            {
                try
                {
                    new InviteReward().DistributeRewards(message, saveTime);
                    i++;
                }
                catch (CommonException cex)
                {
                    new TempDataDAC().MessageComplated(message.Id);
                    LogHelper.Info($"DistributeRewards faild, error message:{cex.Message}, param:{JsonConvert.SerializeObject(message)}");
                    j++;
                }
                catch (Exception ex)
                {
                    _logger.Info($"InviteFaild - ErrorMessage:{ex.Message}, param:{JsonConvert.SerializeObject(message)}");
                    j++;
                }

                //if ((i % 10) == 1)
                //{
                //    j++;
                //}
                //else
                //{
                //    i++;
                //}

                //Thread.Sleep(100);

                //RunData = Tuple.Create(oList.Count, i, j);
                //action(RunData);
            }
        }
    }
}
