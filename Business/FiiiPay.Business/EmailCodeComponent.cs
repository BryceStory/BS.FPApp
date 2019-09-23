using FiiiPay.Data.Agents;
using System;
using System.Threading.Tasks;
using FiiiPay.Business.Properties;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;

namespace FiiiPay.Business
{
    public class EmailCodeComponent
    {
        /// <summary>
        /// 统一发送邮箱验证码
        /// </summary>
        /// <param name="business">要进行的业务，比如：UpdatePin</param>
        /// <param name="id">唯一id，比如：user.Id</param>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        public void Send(string business, object id, string email, string subject)
        {
            var keyByCode = $"FiiiPay:{business}:Code:{id}";

            var code = string.Empty;
            var codeInDb = RedisHelper.StringGet(keyByCode);
            if (!string.IsNullOrEmpty(codeInDb))
            {
                code = codeInDb;
            }
            else
            {
                code = new Random().Next(0, 1000000).ToString("000000");
#if DEBUG
                code = "123456";
#endif
                RedisHelper.StringSet(keyByCode, code, TimeSpan.FromMinutes(Constant.EMAIL_EXPIRED_TIME));
            }

            new EmailAgent().Send(email, subject, string.Format(Resources.VerificationCodoEmailTitle, code, Constant.EMAIL_EXPIRED_TIME), 5,"FiiiPay");
        }

        /// <summary>
        /// 统一发送邮箱验证码（异步）
        /// </summary>
        /// <param name="business">要进行的业务，比如：UpdatePin</param>
        /// <param name="id">唯一id，比如：user.Id</param>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        public async Task SendAsync(string business, object id, string email, string subject)
        {
            var keyByCode = $"FiiiPay:{business}:Code:{id}";

            var code = string.Empty;
            var codeInDb = RedisHelper.StringGet(keyByCode);
            if (!string.IsNullOrEmpty(codeInDb))
            {
                code = codeInDb;
            }
            else
            {
                code = new Random().Next(0, 1000000).ToString("000000");
#if DEBUG
                code = "123456";
#endif
                RedisHelper.StringSet(keyByCode, code, TimeSpan.FromMinutes(Constant.EMAIL_EXPIRED_TIME));
            }

            await new EmailAgent().SendAsync(email, subject, string.Format(Resources.VerificationCodoEmailTitle, code, Constant.EMAIL_EXPIRED_TIME), 5,"FiiiPay");
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="business">要进行的业务，比如：UpdatePin</param>
        /// <param name="id">唯一id，比如：user.Id</param>
        /// <param name="code">验证码</param>
        /// <param name="errorCount">最多可以尝试的次数</param>
        /// <param name="deleteCode">验证通过后是否删除</param>
        public void Verify(string business, object id, string code, int errorCount, bool deleteCode = true)
        {
            var keyByCode = $"FiiiPay:{business}:Code:{id}";
            var keyByCount = $"FiiiPay:{business}:Count:{id}";
            var keyByVerifyCount = $"FiiiPay:{business}:VerifyCount:{id}";
#if pushdev
            var codeInDb = "123456";
#else
            var codeInDb = RedisHelper.StringGet(keyByCode);
#endif
            if (codeInDb != null && codeInDb == code)
            {
                if (deleteCode)
                {
                    RedisHelper.KeyDelete(keyByCode);
                    RedisHelper.KeyDelete(keyByCount);
                    RedisHelper.KeyDelete(keyByVerifyCount);
                }
                else
                {
                    //如果验证通过，并且不删除这个验证码，表示以后还要用这个验证码验证，
                    //这时候把这个验证码的有效期改长一点，免得后面的业务花费太长时间，验证码过期
                    RedisHelper.KeyExpire(keyByCode, TimeSpan.FromHours(1));
                }

                return;
            }

            var countStr = RedisHelper.StringGet(keyByVerifyCount);
            int.TryParse(countStr, out int count);
            if (count >= errorCount)
            {
                RedisHelper.KeyDelete(keyByCode);
                RedisHelper.KeyDelete(keyByCount);
                RedisHelper.KeyDelete(keyByVerifyCount);
                throw new ApplicationException(MessageResources.VerificationMaxLimit);
            }

            RedisHelper.StringSet(keyByVerifyCount, (count + 1).ToString(), TimeSpan.FromMinutes(5));
            throw new ApplicationException(MessageResources.VerificationCodeError);
        }
    }
}
