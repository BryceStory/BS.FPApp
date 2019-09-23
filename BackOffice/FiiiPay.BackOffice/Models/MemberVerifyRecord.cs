using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FiiiPay.BackOffice.Models
{
    public class MemberVerifyRecord
    {
        public int Id { get; set; }
        public int? ApproveId { get; set; }
        public int ProfileId { get; set; }
        public int ProfileType { get; set; }
        public int Status { get; set; }
        public DateTime SubmitTime { get; set; }
        public DateTime? ApproveTime { get; set; }
        public string Remark { get; set; }
    }
}

