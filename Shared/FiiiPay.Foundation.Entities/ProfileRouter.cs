using System.Text;

namespace FiiiPay.Foundation.Entities
{
    public class ProfileRouter
    {
        public int Country { get; set; }
        public string ServerAddress { get; set; }
        public string ClientKey { get; set; }
        public string SecretKey { get; set; }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("Country:" + Country);
            str.Append(",ServerAddress:" + ServerAddress);
            str.Append(",ClientKey:" + ClientKey);
            str.Append(",SecretKey:" + SecretKey);
            return str.ToString();
        }
    }
}
