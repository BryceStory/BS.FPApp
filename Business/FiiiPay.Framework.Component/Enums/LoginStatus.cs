using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Component.Enums
{
    public enum LoginStatus
    {
        /// <summary>
        /// 未登录
        /// </summary>
        UnLogined=0,
        /// <summary>
        /// 登录确认中
        /// </summary>
        Logining=1,
        /// <summary>
        /// 登录成功
        /// </summary>
        Logined=2
    }
}
