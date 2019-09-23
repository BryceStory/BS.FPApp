using FiiiPay.Foundation.Entities;
using System;

namespace FiiiPay.Foundation.API.Models
{
    public class BillerCountryOM : Country
    {
        /// <summary>
        /// 当前国家是否支持缴费功能
        /// </summary>
        public bool Enabled { get; set; }
        public Guid FiatCurrencySymbol { get; set; }
    }
}