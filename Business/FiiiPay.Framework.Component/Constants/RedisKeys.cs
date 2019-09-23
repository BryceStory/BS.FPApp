namespace FiiiPay.Framework.Constants
{
    public static class RedisKeys
    {
        public const string FiiiPOS_APP_MerchantId = "FiiiPOS:APP:MerchantId";
        public const string FiiiPOS_APP_Notice_MerchantId = "FiiiPOS:APP:Notice:MerchantId";
        public const string FiiiPOS_APP_Language_MerchantId = "FiiiPOS:APP:Language:MerchantId";
        public const string FiiiPOS_APP_EmailVerification = "FiiiPOS:APP:EmailVerification";
        public const string FiiiPOS_APP_OriginalEmailVerification = "FiiiPOS:APP:OriginalEmailVerification"; //验证原邮箱用
        public const string FiiiPOS_APP_BroadcastNo = "FiiiPOS:OTP:{0}";
        public const string FiiiPOS_WEB_EmailVerification = "FiiiPOS:WEB:EmailVerification";


        public const string FiiiPOS_APP_Signup_Binding_Account = "FiiiPOS:APP:Signup:Binding:Account";



        public const string FiiiPOS_APP_QRCodePrefix_Web = "wm";
        public const int FiiiPOS_APP_RedisDB_Web = 1; //fiiipos web redis数据库
        public const string FiiiPOS_APP_Redis_Key_QRCode_Web = "FiiiPOS:Web:QRCode:"; //fiiipos web redis QRCode key
    }
}