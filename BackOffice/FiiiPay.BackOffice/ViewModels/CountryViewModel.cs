using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.BackOffice.ViewModels
{
    public class CountryViewModel : Country
    {
        public string BlobServerAddress { get; set; }
        public string BlobClientKey { get; set; }
        public string BlobSecretKey { get; set; }

        public string ProfileServerAddress { get; set; }
        public string ProfileClientKey { get; set; }
        public string ProfileSecretKey { get; set; }
    }
}