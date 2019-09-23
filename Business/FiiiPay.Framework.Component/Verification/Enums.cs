namespace FiiiPay.Framework.Verification
{
    public enum FiiipayBusiness
    {
        Common = 0,
        CombinedVerification,
        Register,
        Login,
        ForgotPassword,
        UpdatePin,
        FindPinBack,
        UpdatePassword,
        UpdateCellphoneOld,
        UpdateCellphoneNew,
        /// <summary>
        /// fiiipos 绑定账号
        /// </summary>
        BindingAccount,
        /// <summary>
        /// FiiiShop支付
        /// </summary>
        FiiiShowPay,
        ResetPin
    }

    /// <summary>
    /// 验证码生成类型
    /// </summary>
    public enum CodeGenerateMethod
    {
        /// <summary>
        /// 不确定的
        /// </summary>
        Indefinite = 0,
        /// <summary>
        /// 固定的，无法将验证码失效
        /// </summary>
        Fixed,
        /// <summary>
        /// 可计算的
        /// </summary>
        Computable,
        /// <summary>
        /// 随机的
        /// </summary>
        Random
    }
}
