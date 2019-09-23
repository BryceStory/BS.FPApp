using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities.EntitySet
{
    public class FiiiCoinRecordES
    {
        /// <summary>
        /// RewardList表主键
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string MerchantUsername { get; set; }
        /// <summary>
        /// 类型 0：POS，1：手机
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 原始奖励
        /// </summary>
        public long OriginalReward { get; set; }
        /// <summary>
        /// 实际奖励
        /// </summary>
        public long ActualReward { get; set; }
        /// <summary>
        /// 是否支付 0：未支付，1已支付，2作废
        /// </summary>
        public int Paid { get; set; }
        /// <summary>
        /// 生成时间时间戳
        /// </summary>
        public long GenerateTime { get; set; }
        /// <summary>
        /// 支付时间时间戳
        /// </summary>
        public long PaidTime { get; set; }
        /// <summary>
        /// 提成是否发放 0：未发放，1已发放，2未被邀请，无需发放，3未达到奖励条件
        /// </summary>
        public int IsCommissionProcessed { get; set; }
        /// <summary>
        /// 提成发放时间
        /// </summary>
        public long? CommissionProcessedTime { get; set; }
    }
}
