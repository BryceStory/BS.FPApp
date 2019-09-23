using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FiiiPay.Entities;
using SqlSugar;

namespace FiiiPay.BackOffice.Models
{
    public class StoreTypes : StoreType
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public new int Id { get; set; }
    }
}