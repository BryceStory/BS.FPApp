using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class ModifyAddressModel
    {
        /// <summary>
        /// Address1
        /// </summary>
        [StringLength(100, MinimumLength = 1)]
        public string Address1 { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        [StringLength(50, MinimumLength = 1)]
        public string Postcode { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        [StringLength(50, MinimumLength = 1)]
        public string City { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        [StringLength(50, MinimumLength = 1)]
        public string State { get; set; }
    }

    public class ModifyAddress2Model
    {
        /// <summary>
        /// Address1
        /// </summary>
        [Required, StringLength(100, MinimumLength = 1)]
        public string Address2 { get; set; }
    }
}