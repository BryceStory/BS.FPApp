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
    public class RoleAuthority
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyBy { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public int ModuleId { get; set; }
        public int Value { get; set; }
    }
}

