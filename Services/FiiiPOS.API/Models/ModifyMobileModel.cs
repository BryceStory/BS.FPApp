using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{

    public class CellphoneSMSIM
    {
        /// <summary>
        /// 手机号码(不带区号)
        /// </summary>
        [Required]
        public string Cellphone { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ModifyMobileModel
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string Cellphone { get; set; }
    }
}