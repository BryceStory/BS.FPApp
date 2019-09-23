using FiiiPay.Framework.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FiiiPay.Entities.Enums;

namespace FiiiPay.BackOffice.ViewModels
{
    public class InvestorViewModel
    {
        public int Id { get; set; }
        public string Cellphone { get; set; }
        public string Username { get; set; }
        public string InvestorName { get; set; }
        public AccountStatus Status { get; set; }
        public string Email { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public decimal? Balance { get; set; }
        public string Password { get; set; }
        public string PIN { get; set; }
        public string Remark { get; set; }
        public bool? IsUpdatePassword { get; set; }
        public bool? IsUpdatePin { get; set; }
    }
}