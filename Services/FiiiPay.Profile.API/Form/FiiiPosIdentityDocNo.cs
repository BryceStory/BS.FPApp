using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Profile.API.Form
{
    public class FiiiPosIdentityDocNo
    {
        [Required]
        public string IdentityDocNo { get; set; }
    }
}