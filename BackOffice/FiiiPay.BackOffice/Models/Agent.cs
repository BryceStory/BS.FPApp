using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FiiiPay.BackOffice.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyBy { get; set; }
        public string AgentCode { get; set; }
        public int? SaleId { get; set; }
        public string CompanyName { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string ContactName { get; set; }
        public string ContactWay { get; set; }
    }
}

