using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FiiiPay.DTO.Wallet;
using FiiiPay.Entities;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO
{
    /// <summary>
    /// 缴费的参数
    /// </summary>
    public class BillerPayIM
    {
        /// <summary>
        /// billercode
        /// </summary>
        [Required, MinLength(4),MaxLength(50)]
        public string BillerCode { get; set; }
        /// <summary>
        /// referencenumber
        /// </summary>
        [Required, MinLength(4),MaxLength(20)]
        public string ReferenceNumber { get; set; }
        /// <summary>
        /// 法币币种
        /// </summary>
        [Required, MaxLength(20)]
        public string FiatCurrency { get; set; }
        /// <summary>
        /// 所选的加密币简称
        /// </summary>
        [Required, MaxLength(20)]
        public string CryptoCode { get; set; }
        /// <summary>
        /// 加密币的唯一键
        /// </summary>
        [Required]
        public int CryptoId { get; set; }
        /// <summary>
        /// 法币金额
        /// </summary>
        [Required]
        public decimal FiatAmount { get; set; }
        /// <summary>
        /// 加密币金额
        /// </summary>
        [Required]
        public decimal CryptoAmount { get; set; }
        /// <summary>
        /// 当前汇率
        /// </summary>
        [Required]
        public decimal ExchangeRate { get; set; }
        /// <summary>
        /// pin码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Pin { get; set; }
        
        public string Tag { get; set; }
        /// <summary>
        /// 折扣率
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 选择国家的id
        /// </summary>
        public int CountryId { get; set; }
    }
    /// <summary>
    /// 缴费的参数
    /// </summary>
    public class BillerPrePayIM
    {
        /// <summary>
        /// 法币币种
        /// </summary>
        [Required, MaxLength(20)]
        public string FiatCurrency { get; set; }
        /// <summary>
        /// 所选的加密币简称
        /// </summary>
        [Required, MaxLength(20)]
        public string CryptoCode { get; set; }
        /// <summary>
        /// 加密币的唯一键
        /// </summary>
        [Required]
        public int CryptoId { get; set; }
        /// <summary>
        /// 法币金额
        /// </summary>
        [Required]
        public decimal FiatAmount { get; set; }
        /// <summary>
        /// 加密币金额
        /// </summary>
        [Required]
        public decimal CryptoAmount { get; set; }

        /// <summary>
        /// 选择国家的id
        /// </summary>
        [Required]
        public int CountryId { get; set; }
    }
    public class BillerPayOM
    {
        /// <summary>
        /// 订单id
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// 订单号显示用
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 加密币简称
        /// </summary>
        public string CryptoCode { get; set; }
        /// <summary>
        /// 加密币金额
        /// </summary>
        public string CryptoAmount { get; set; }
        /// <summary>
        /// 是否已经保存地址true：已经保存了，false: 未保存
        /// </summary>
        public bool SaveAddress { get; set; }
    }

    public class BillerDetailIM
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
    }

    public class BillerDetailOM
    {
        public Guid Id { get; set; }
        public string FiatAmount { get; set; }

        public string CryptoAmount { get; set; }

        public string CryptoCode { get; set; }

        public string BillerCode { get; set; }

        public string ReferenceNumber { get; set; }

        public string ExchangeRate { get; set; }

        public string Timestamp { get; set; }

        public BillerOrderStatus Status { get; set; }

        public string Remark { get; set; }

        public string Tag { get; set; }

        public string FiatCurrency { get; set; }

        public string OrderNo { get; set; }
        /// <summary>
        /// 当前汇率
        /// </summary>
        public string CurrentExchangeRate { get; set; }
        /// <summary>
        /// 涨幅
        /// </summary>
        public string IncreaseRate { get; set; }
        /// <summary>
        /// 状态提示语
        /// </summary>
        public string StatusStr { get; set; }
        /// <summary>
        /// 类型提示语
        /// </summary>
        public string TypeStr { get; set; }
    }

    public class BillerDeleteAddressIM
    {
        public long Id { get; set; }
    }


    public class IconInfo
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 文件id
        /// </summary>
        public string Id { get; set; }
    }

    public class BillerEditAddressIM : BillerAddAddressIM
    {
        /// <summary>
        /// address唯一主键
        /// </summary>
        public int Id { get; set; }
    }


    public class BillerAddAddressIM
    {
        [Required, MinLength(4), MaxLength(50)]
        public string BillerCode { get; set; }
        [Required, MinLength(4), MaxLength(50)]
        public string ReferenceNumber { get; set; }
        [Required, MaxLength(50)]
        public string Tag { get; set; }
        /// <summary>
        /// 图标的索引
        /// </summary>
        [Required, MaxLength(30)]
        public string IconIndex { get; set; }
    }

    public class BillerGetAddressIM
    {
        /// <summary>
        /// 图标筛选的唯一id
        /// </summary>
        public string IconIndex { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 从0开始
        /// </summary>
        public int PageIndex { get; set; } = 0;
    }

    public class BillerAddressOM
    {
        public int Id { get; set; }

        public string BillerCode { get; set; }

        public string ReferenceNumber { get; set; }

        public string Tag { get; set; }

        public string IconIndex { get; set; }

        public string Timestamp { get; set; }
    }

    public class BillerCryptoIM
    {
        /// <summary>
        /// 法币简称
        /// </summary>
        public string FiatCurrency { get; set; }
    }

    public class BillerCryptoOM
    {
        public string FaitCurrency { get; set; }

        public List<BillerCryptoItemOM> List { get; set; }
    }

    public class BillerCryptoItemOM : ListForDepositOMItem
    {
        /// <summary>
        /// 折扣
        /// </summary>
        public string Discount { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public string ExchangeRate { get; set; }
    }

    public class BillerPrePayOM
    {
        /// <summary>
        /// 0: 可支付 1: 超过单笔的限额 2：超过当天限额 3： 超过当月限额 4：金额计算校验不正确 5：所选国家信息不正确
        /// </summary>
        public byte Status { get; set; }
    }

    public class BillerMessageFailOM
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
    }
}
