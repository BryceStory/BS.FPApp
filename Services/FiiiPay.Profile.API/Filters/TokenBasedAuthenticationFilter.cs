using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using FiiiPay.Framework;

namespace FiiiPay.Profile.API.Filters
{
    /// <summary>
    /// Class FiiiPay.Profile.API.Filters.TokenBasedAuthenticationFilter
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.IAuthenticationFilter" />
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
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
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
                return Task.FromResult(0);
            }

            try
            {
                var authHeader = request.Headers.Authorization.Parameter;
                var clientKey = ConfigurationManager.AppSettings["ClientKey"];
                var secretKey = ConfigurationManager.AppSettings["SecretKey"];

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
                        if (timestamp.AddMinutes(5) >= DateTime.UtcNow && timestamp.AddMinutes(-5) <= DateTime.UtcNow)
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
            return Task.FromResult(0);
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

    /// <summary>
    /// Class FiiiPay.Profile.API.Filters.FiiiPayIdentity
    /// </summary>
    /// <seealso cref="System.Security.Claims.ClaimsIdentity" />
    public class FiiiPayIdentity : ClaimsIdentity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FiiiPayIdentity"/> class.
        /// </summary>
        /// <param name="claims">The claims with which to populate the claims identity.</param>
        /// <param name="authenticationType">The type of authentication used.</param>
        public FiiiPayIdentity(IEnumerable<Claim> claims, string authenticationType) : base(claims, authenticationType)
        {
            foreach (var claim in claims)
            {
                if (claim.Type == ClaimTypes.NameIdentifier)
                {
                    Id = Convert.ToInt64(claim.Value);
                }
            }
        }
    }
}