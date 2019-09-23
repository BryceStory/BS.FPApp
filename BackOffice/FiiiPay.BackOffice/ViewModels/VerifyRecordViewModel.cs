using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{

    [Serializable]
    public class VerifyRecordViewModel
    {
        public string VerifyAccount { get; set; }
        public int VerifyCount { get; set; }
    }
}