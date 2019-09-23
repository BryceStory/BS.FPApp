using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Component.Verification
{
    public class SecurityModel : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool CellphoneVerified { get; set; }
        public bool GoogleAuthVerified { get; set; }
        public bool CombinedVerified { get; set; }
    }

    public class ResetPasswordVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool CellphoneVerified { get; set; }
    }

    public class UpdatePasswordVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
    }
    public class UpdatePinVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool CombinedVerified { get; set; }
    }
    public class UpdateCellphoneVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool CombinedVerified { get; set; }
        public bool NewCellphoneVerified { get; set; }
    }
    public class ModifyCellphoneVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool NewCellphoneVerified { get; set; }
        public bool CombinedVerified { get; set; }
    }
    public class ResetPinVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool CombinedVerified { get; set; }
    }

    public class LoginBySMSVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool CellphoneVerified { get; set; }
        public bool GoogleAuthVerified { get; set; }
        public bool CombinedVerified { get; set; }
    }

    public class WithdrawVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool CombinedVerified { get; set; }
    }
    public class NewDeviceLogin : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool CombinedVerified { get; set; }
    }
    public class BindGoogleAuth : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool GoogleVerified { get; set; }
        public bool CombinedVerified { get; set; }
    }
    public class UnBindGoogleAuth : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool CombinedVerified { get; set; }
    }
    public class OpenGoogleAuth : IVerified
    {
        public long ExpireTime { get; set; }
        public bool GoogleVerified { get; set; }
    }
    public class CloseGoogleAuth : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool CombinedVerified { get; set; }
    }
    public class SetEmailVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool EmailVerified { get; set; }
    }
    public class UpdateEmailVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool OriginalEmailVerified { get; set; }
        public bool NewEmailVerified { get; set; }
    }
    public class FiiiPosSignUpVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool CellphoneVerified { get; set; }
    }
    public class BindAccountVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public string MerchantAccount { get; set; }
        public bool CellphoneVerified { get; set; }
        public bool PinVerified { get; set; }
        public bool GoogleVerified { get; set; }
    }
    public class UnBindAccountVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public bool PinVerified { get; set; }
        public bool CombinedVerified { get; set; }
    }
    public class FiiiPosSetEmailVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public string Email { get; set; }
        public bool PinVerified { get; set; }
        public bool EmailVerified { get; set; }
    }
    public class FiiiPosUpdateEmailVerify : IVerified
    {
        public long ExpireTime { get; set; }
        public string NewEmail { get; set; }
        public bool PinVerified { get; set; }
        public bool OriginalEmailVerified { get; set; }
        public bool NewEmailVerified { get; set; }
    }
}
