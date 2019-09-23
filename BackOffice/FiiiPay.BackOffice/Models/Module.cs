using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Summary description for Class1
/// </summary>
/// 
namespace FiiiPay.BackOffice.Models
{
    public class Module
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyBy { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public int Sort { get; set; }
        public string PathAddress { get; set; }
    }
}

