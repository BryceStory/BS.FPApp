using System;
using System.Configuration;
using System.Web.Http;
using FiiiPay.Framework;

namespace FiiiPay.Foundation.API.Controllers
{
    public class LoginController : ApiController
    {
        [HttpPost]
        [Route("Generatekey")]
        [AllowAnonymous]
        public ServiceResult<string> Generatekey(GenerateKeyObject entity)
        {
            var resultData = new ServiceResult<string>();
            try
            {
                string PWD = "WiYf1uyXenUdIM6Bf1OSy5HOPVO6ufQi";
                if (entity.Password == PWD)
                {
                    resultData.Data = GenerateToken();
                    resultData.Success();
                }
            }
            catch
            {
                resultData.Failer(10001, "Password error");
            }

            return resultData;
        }

        private string GenerateToken()
        {
            string clientKey = ConfigurationManager.AppSettings["ClientKey"];
            string secretKey = ConfigurationManager.AppSettings["SecretKey"];
            string password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + clientKey;
            string token = AES128.Encrypt(password, secretKey);
            return token;
        }
    }
    
    public class GenerateKeyObject
    {
        public string Password { get; set; }
    }
}