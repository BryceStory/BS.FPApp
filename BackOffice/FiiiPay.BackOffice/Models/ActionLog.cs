using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FiiiPay.BackOffice.Models
{
    public class ActionLog
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string IPAddress { get; set; }
        public string ModuleCode { get; set; }
        public int LogLevel { get; set; }
        public string LogContent { get; set; }
    }
}

