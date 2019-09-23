namespace FiiiPOS.DTO
{
    /// <summary>
    /// 用户开启的密保方式
    /// </summary>
    public class GetOpenedSecuritiesOM
    {
        /// <summary>
        /// 是否开启了谷歌认证
        /// </summary>
        public bool IsOpenedAuthencator { get; set; }
        /// <summary>
        /// 用户手机号密文
        /// </summary>
        public string CellPhone { get; set; }

    }
}
