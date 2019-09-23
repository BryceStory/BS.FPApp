namespace FiiiPay.Framework.Component
{
    public enum SMSBusiness
    {
        /// <summary>
        /// 独立的电话号码安全验证
        /// </summary>
        PhoneCode,
        SecurityValidate,
        Login,
        Register,
        ForgotPassword,
        UpdatePin,
        FindPinBack,
        UpdatePassword,
        UpdateCellphoneOld,
        UpdateCellphoneNew,

        /// <summary>
        /// fiiipos 绑定账号
        /// </summary>
        BindingAccount
    }
}
