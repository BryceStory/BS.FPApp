using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.BackOffice.Models
{
    public class AccountRole
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

