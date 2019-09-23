namespace FiiiPay.Framework.Component.Verification
{
    public class UserSecrets
    {
        /// <summary>
        /// ValidationFlag
        /// </summary>
        public byte? ValidationFlag { get; set; }
        public string IdentityNo { get; set; }
        public string GoogleAuthSecretKey { get; set; }
        public string Pin { get; set; }
        public string Password { get; set; }
    }
}
