using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.Web.API.Models.Input
{
    public class UpdateAddressModel
    {
        /// <summary>
        /// 邮编
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 1)]
        public string Postcode { get; set; }

        /// <summary>
        /// 州/省
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(32, MinimumLength = 1)]
        public string State { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(32, MinimumLength = 1)]
        public string City { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(200, MinimumLength = 1)]
        public string Address { get; set; }
    }
}