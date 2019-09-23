namespace FiiiPay.Framework.Component
{
    /// <summary>
    /// 安全验证类型，一个类型独立缓存数据，独立计算错误次数
    /// </summary>
    public enum SecurityMethod
    {
        CellphoneCode,
        Password,
        LoginPhoneCode,
        RegisterPhoneCode,
        Pin,
        GoogleAuthencator,
        LoginGoogleAuthencator,
        LoginBySMSGoogleAuthencator,
        SecurityValidate,
        TempToken,
        /// <summary>
        /// 修改密码的旧密码验证
        /// </summary>
        OldPassword
    }
}
