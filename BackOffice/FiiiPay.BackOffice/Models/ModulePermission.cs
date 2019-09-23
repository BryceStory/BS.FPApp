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
    public class ModulePermission
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyBy { get; set; }
        public int ModuleId { get; set; }
        /// <summary>
        /// 是否菜单权限
        /// </summary>
        public bool IsDefault { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
}

