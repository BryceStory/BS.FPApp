using System;

namespace FiiiPay.Framework.Enums
{
    [Flags]
    public enum ValidationFlag
    {
        Cellphone = 1,
        Email = 2,
        GooogleAuthenticator = 4,
        IDNumber = 8
    }
}