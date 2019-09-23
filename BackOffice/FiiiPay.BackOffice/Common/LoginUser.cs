using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Common
{
    public class LoginUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsAdmin { get; set; }
        public List<UserPermission> PerimissionList { get; set; }
    }
    [Serializable]
    public class UserPermission
    {
        public bool IsDefault { get; set; }
        public string PerimCode { get; set; }
        public int ModuleId { get; set; }
        public string ModuleCode { get; set; }
        public Int32 Value { get; set; }
    }
}