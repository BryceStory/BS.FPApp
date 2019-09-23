﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities.EntitySet
{
    public class MiningInfo
    {
        public byte AccountType { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
