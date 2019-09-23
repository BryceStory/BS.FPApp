using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.Web.API.Models.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class RefundOrderInModel
    {
        /// <summary>
        /// OrderNo
        /// </summary>
        [Required]
        public string OrderNo { set; get; }

        /// <summary>
        /// PIN码
        /// </summary>
        [Required]
        public string PinPassword { set; get; }
    }
}