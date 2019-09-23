namespace FiiiPay.DTO.Account
{
    public class LoginOM
    {
        public string AccessToken { get; set; }
        public string ExpiresTime { get; set; }
        public SimpleUserInfoOM UserInfo { get; set; }

        public bool IsNewDevice { get; set; }

        public bool IsNeedGoogleVerify { get; set; }
    }

   
    
}
