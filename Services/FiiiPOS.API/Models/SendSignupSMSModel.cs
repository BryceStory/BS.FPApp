using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class SendSignupSMSModel
    {
        /// <summary>
        /// 国家ID
        /// </summary>
        [Required, Range(0, 100)]
        public int CountryId { get; set; }
        /// <summary>
        /// （区号）手机号
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Cellphone { get; set; }

        /// <summary>
        /// POSSN
        /// </summary>
        [Required]
        public string POSSN { get; set; }

    }

    public class SendBindingSMSModel
    {
        /// <summary>
        /// 商户账号
        /// </summary>
        public string MerchantAccount { get; set; }
        /// <summary>
        /// 国家ID
        /// </summary>
        [Required, Range(0, 100)]
        public int CountryId { get; set; }
        /// <summary>
        /// （区号）手机号
        /// </summary>
        [Required, StringLength(50)]
        public string Cellphone { get; set; }        /// <summary>
        /// POSSN
        /// </summary>
        [Required]
        public string POSSN { get; set; }

    }
}