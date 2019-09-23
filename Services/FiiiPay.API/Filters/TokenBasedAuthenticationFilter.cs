using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using FiiiPay.Business;
using FiiiPay.Framework;
using System.Linq;
using FiiiPay.Business.Properties;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.API.Filters
{
    /// <summary>
    /// Class FiiiPay.API.Filters.TokenBasedAuthenticationFilter
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.FilterAttribute" />
    /// <seealso cref="System.Web.Http.Filters.IAuthenticationFilter" />
    public class TokenBasedAuthenticationFilter : FilterAttribute, IAuthenticationFilter
    {
        /// <summary>Gets the authentication type of the OWIN middleware to use.</summary>
        /// <returns>The authentication type of the OWIN middleware to use.</returns>
        public string AuthenticationType { get; }

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

            if (context.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0)
            {
                return;
            }

            HttpRequestMessage request = context.Request;
            if (request == null)
            {
                throw new InvalidOperationException("Request must not be null");
            }

            if (context.Request.Headers.Authorization == null)
            {
                throw new UnauthorizedException();
            }
            

            var authHeader = request.Headers.Authorization.Parameter;
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                throw new UnauthorizedException();
            }

            var lang = context.Request.Headers.AcceptLanguage.FirstOrDefault()?.Value;
            if (!string.IsNullOrWhiteSpace(lang) && !(lang.Contains("en") || lang.Contains("zh")))
            {
                throw new CommonException(ReasonCode.MISSING_REQUIRED_FIELDS, MessageResources.SystemError);
            }

            var authComponent = new AuthenticatorComponent();
            var account = authComponent.Token(authHeader, lang);

            context.Principal = new ClaimsPrincipal();
            context.Request.Properties.Add("User", account);
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
            HttpRequestMessage request = context.Request;
            if (request == null)
            {
                throw new InvalidOperationException("Request must not be null");
            }

            return Task.FromResult(0);
        }
    }
}