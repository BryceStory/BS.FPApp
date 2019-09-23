using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Profile
{
    public class UpdateBirthdayIM
    {
        /// <summary>
        /// 精确到天的日期，比如：1986-08-12
        /// </summary>
        [Required(AllowEmptyStrings = false), MaxLength(50)]
        public string Date { get; set; }
    }
}
