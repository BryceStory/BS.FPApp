namespace FiiiPOS.DTO
{
    /// <summary>
    /// GetStatusOfSecurity
    /// </summary>
    public class GetStatusOfSecurityOM
    {
        /// <summary>
        /// google authenticator status
        /// </summary>
        public SecurityStatus GoogleAuthenticator { get; set; }
    }
    /// <summary>
    /// SecurityStatus
    /// </summary>
    public class SecurityStatus
    {
        /// <summary>
        /// 是否已绑定
        /// </summary>
        public bool HasBinded { get; set; }
        /// <summary>
        /// 是否已打开
        /// </summary>
        public bool HasOpened { get; set; }
    }
}
