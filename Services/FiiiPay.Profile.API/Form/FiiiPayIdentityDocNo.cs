using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPay.Profile.API.Form
{
    public class FiiiPayIdentityDocNo
    {
        [Required]
        public string IdentityDocNo { get; set; }
    }
}