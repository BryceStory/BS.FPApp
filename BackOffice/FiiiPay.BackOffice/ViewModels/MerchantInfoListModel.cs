using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    /// <summary>
    /// FiiipayMerchantInfo
    /// </summary>
    public class FiiipayMerchantInfoListModel
    {
        public Guid Id { get; set; }
        public string FiiiPayAccount { get; set; }
        public string MerchantName { get; set; }
        public int CountryId { get; set; }
        public byte Status { get; set; }
        public byte VerifyStatus { get; set; }
        public bool IsAllowExpense { get; set; }
        public byte FromType { get; set; }
    }
    public class FiiiPosMerchantInfoListModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string MerchantName { get; set; }
        public string Cellphone { get; set; }
        public int CountryId { get; set; }
        public byte Status { get; set; }
        public byte VerifyStatus { get; set; }
    }
}