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
    public class Account
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyBy { get; set; }
        public int? RoleId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public string Cellphone { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginTime { get; set; }
    }
}

