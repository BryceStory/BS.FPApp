using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FiiiPay.BackOffice.Models
{
    public class Salesperson
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyBy { get; set; }
        public string SaleCode { get; set; }
        public string SaleName { get; set; }
        public int Gender { get; set; }
        public string Position { get; set; }
        public string Mobile { get; set; }
    }
}

