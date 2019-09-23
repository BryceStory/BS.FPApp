using FiiiPay.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
   public class MerchantInformation
    {
        public Guid Id { get; set; }

        public DateTime CreateTime { get; set; }
        public string LastModifyBy { get; set; }
        public DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 录入来源
        /// </summary>
        public InputFromType FromType { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 营业日期
        /// </summary>
        public Week Week { get; set; }

        /// <summary>
        /// 营业开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 营业结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 营业时间描述(FiiiPay门店使用)
        /// </summary>
        public string WeekTxt { get; set; }

        /// <summary>
        /// 产品/服务
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 商家介绍
        /// </summary>
        public string Introduce { get; set; }
        /// <summary>
        /// 所在国家ID
        /// </summary>
        public int CountryId { get; set; }
        /// <summary>
        /// 省/州Id
        /// </summary>
        public long? StateId { get; set; }
        /// <summary>
        /// 市Id
        /// </summary>
        public long? CityId { get; set; }
        /// <summary>
        /// 国家区号
        /// </summary>
        public string PhoneCode { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal Lng { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Lat { get; set; }

        /// <summary>
        /// 商家停用状态
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 商家审核状态
        /// </summary>
        public VerifyStatus VerifyStatus { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid MerchantAccountId { get; set; }

        /// <summary>
        /// 审核Remark
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? VerifyDate { get; set; }

        /// <summary>
        /// 商家信息是否显示
        /// </summary>
        public Status IsPublic { get; set; }

        public Guid FileId { get; set; }
        public Guid ThumbnailId { get; set; }
        /// <summary>
        /// 溢价率，仅FiiiPay门店设置
        /// </summary>
        public decimal Markup { get; set; }
        /// <summary>
        /// 交易手续费率，仅FiiiPay门店设置
        /// </summary>
        public decimal FeeRate { get; set; }
        /// <summary>
        /// 是否允许交易
        /// </summary>
        public bool IsAllowExpense { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplicantName { get; set; }

        /// <summary>
        /// 是否用Fiii抵扣交易手续费
        /// </summary>
        public bool UseFiiiDeduct { get; set; }
    }

    /// <summary>
    /// 营业日期
    /// </summary>
    public enum Week
    {
        Monday = 1,
        Tuesday = 1 << 1,
        Wednesday = 1 << 2,
        Thursday = 1 << 3,
        Friday = 1 << 4,
        Saturday = 1 << 5,
        Sunday = 1 << 6
    }

    /// <summary>
    /// 商家停用状态
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// 启用
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// 停止
        /// </summary>
        Stop = 0,
    }

    /// <summary>
    /// 录入来源
    /// </summary>
    public enum InputFromType
    {
        UserInput,
        BOInput
    }
}
