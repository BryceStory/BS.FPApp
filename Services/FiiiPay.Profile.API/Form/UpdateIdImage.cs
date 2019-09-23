using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.Profile.API.Form
{
    public class UpdateIdImage
    {
        public Guid Id { get; set; }
        public Guid FrontImage { get; set; }
        public Guid BackImage { get; set; }
        public Guid HandHoldImage { get; set; }
    }
}