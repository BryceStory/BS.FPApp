using System;

namespace FiiiPay.Entities
{
    public class RenewRecord
    {

        /// <summary>
        /// 
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// 用户卡Id
        /// </summary>
        public int UserCardId { set; get; }

        /// <summary>
        /// 续费开始日期
        /// </summary>
        public DateTime StartDate { set; get; }

        /// <summary>
        /// 续费结束日期
        /// </summary>
        public DateTime EndDate { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { set; get; }

        /// <summary>
        /// 后台操作人员Id
        /// </summary>
        public int AdminUserId { set; get; }
    }
}