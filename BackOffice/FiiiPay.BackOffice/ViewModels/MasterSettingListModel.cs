using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class MasterSettingListModel
    {
        public string Merchant_TransactionFee { get; set; }
        public string Merchant_Markup { get; set; }
        public string BillerEnable { get; set; }
        public string DiscountRate { get; set; }
        public string Error_Tolerant_Rate { get; set; }
        public string Biller_MaxAmount { get; set; }
        public string Biller_Day_MaxAmount { get; set; }
        public string Biller_Month_MaxAmount { get; set; }
        public string RedPocket_AmountLimit { get; set; }
        public string RedPocket_CountLimit { get; set; }
    }
}