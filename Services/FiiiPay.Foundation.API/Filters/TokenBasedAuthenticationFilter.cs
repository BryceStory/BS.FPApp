using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Foundation.API.Filters
{
    public class TokenBasedAuthenticationFilter : IAuthenticationFilter
    {
        //private static readonly Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisConnectionString"]));

        /// <summary>Gets the authentication type of the OWIN middleware to use.</summary>
        /// <returns>The authentication type of the OWIN middleware to use.</returns>
        public string AuthenticationType { get; }

        /// <summary>Gets a value indicating whether the filter allows multiple authentication.</summary>
        /// <returns>true if the filter allows multiple authentication; otherwise, false.</returns>
        public bool AllowMultiple => false;

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Http.BaseAuthenticationFilter" /> class.</summary>
        /// <param name="authenticationType">The authentication type of the OWIN middleware to use.</param>
        public TokenBasedAuthenticationFilter(string authenticationType)
        {
            AuthenticationType = authenticationType ?? throw new ArgumentNullException(nameof(authenticationType));
        }

        /// <summary>Asynchronously authenticates the request.</summary>
        /// <returns>The task that completes the authentication.</returns>
        /// <param name="context">The authentication context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var request = context.Request;
            if (request == null)
            {
                throw new InvalidOperationException("Request must not be null");
            }

            if (context.Request.Headers.Authorization == null)
            {
                return;
            }

            try
            {
                var authHeader = request.Headers.Authorization.Parameter;
                var clientKey = ConfigurationManager.AppSettings["ClientKey"];
                var secretKey = ConfigurationManager.AppSettings["SecretKey"];

                //var FiiiPayIsMaintain = RedisHelper.StringGet($"FiiiPay:API:IsMaintain");
                //if (FiiiPayIsMaintain.Equals("True"))
                //{
                //    throw new CommonException(10300, "Maintenance");
                //}

                try
                {
                    var decryptedPIN = AES128.Decrypt(authHeader, secretKey);

                    if (decryptedPIN.Length > 14)
                    {
                        var date = decryptedPIN.Substring(0, 14);
                        var timestamp = DateTime.ParseExact(date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                        //Payment PIN submission must be within accepted time.
                        //This is to prevent hacker copy the encrypted string and use it to authorize victim payment PIN without the need
                        //to know the exact PIN
                        if (timestamp.AddMinutes(5) < DateTime.UtcNow || timestamp.AddMinutes(-5) > DateTime.UtcNow)
                        {
                            //DANGER: Need to write log here
                        }
                        else
                        {
                            var actualPIN = decryptedPIN.Substring(14);

                            if (actualPIN == clientKey)
                            {
                                SetSecurityPrincipal(ref context, "1", "FiiiPay", "");
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //DANGER: Need to write log here
                }

            }
            catch (Exception)
            {
                //DANGER: Need to write log here
            }
        }

        private static void SetSecurityPrincipal(ref HttpAuthenticationContext context, string identifier, string name, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identifier),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new FiiiPayIdentity(claims, "Bearer");

            context.Principal = new ClaimsPrincipal(identity);
        }

        /// <summary>Asynchronously challenges an authentication.</summary>
        /// <returns>The task that completes the challenge.</returns>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var request = context.Request;
            if (request == null)
            {
                throw new InvalidOperationException("Request must not be null");
            }

            return Task.FromResult(0);
        }
    }

    public class FiiiPayIdentity : ClaimsIdentity
    {
        public long Id { get; set; }

        public FiiiPayIdentity(IEnumerable<Claim> claims, string authenticationType) : base(claims, authenticationType)
        {
            foreach (var claim in claims)
            {
                if (claim.Type == ClaimTypes.NameIdentifier)
                {
                    this.Id = Convert.ToInt64(claim.Value);
                }
            }
        }
    }
}