using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Component.Verification
{
    /// <summary>
    /// 业务验证情况
    /// </summary>
    public interface IVerified
    {
        long ExpireTime { get; set; }
    }
}
