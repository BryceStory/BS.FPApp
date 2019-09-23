using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using FiiiPay.Framework;
using FiiiPOS.Web.Business;
using FiiiPay.Entities;
using System.Net;
using System.Net.Http.Formatting;

namespace FiiiPOS.Web.API.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenAuthenticationFilter : FilterAttribute, IAuthenticationFilter
    {
        /// <summary>
        /// 默认为 Bearer
        /// </summary>
        private readonly string _authenticationType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationType"></param>
        public TokenAuthenticationFilter(string authenticationType)
        {
            _authenticationType = authenticationType;
        }

        /// <summary>Asynchronously authenticates the request.</summary>
        /// <returns>The task that completes the authentication.</returns>
        /// <param name="context">The authentication context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
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
                Unauthorized(context);
                return;
            }

            var authHeader = request.Headers.Authorization.Parameter;
            MerchantAccount account = new MerchantComponent().GetMerchantAccountByToken(authHeader);
            if (account == null)
            {
                Unauthorized(context);
            }
            else
            {
                SetSecurityPrincipal(ref context, account.Id.ToString(), account.Username, account.CountryId);
            }

        }

        /// <summary>Asynchronously challenges an authentication.</summary>
        /// <returns>The task that completes the challenge.</returns>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            HttpRequestMessage request = context.Request;
            if (request == null)
            {
                throw new InvalidOperationException("Request must not be null");
            }

            return Task.FromResult(0);
        }

        private void SetSecurityPrincipal(ref HttpAuthenticationContext context, string identifier, string name, int countryId)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, identifier));
            claims.Add(new Claim(ClaimTypes.Name, name));
            claims.Add(new Claim(ClaimTypes.Country, countryId.ToString())); //国家Id
            WebIdentity identity = new WebIdentity(claims, "Bearer");

            context.Principal = new ClaimsPrincipal(identity);
        }

        private static void Unauthorized(HttpAuthenticationContext context)
        {
            context.ErrorResult = new ErrorResult
            {
                Code = 401,
                Message = "Unauthorized"
            };
        }

        class ErrorResult : ServiceResult, IHttpActionResult
        {
            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ErrorResult>(this, new JsonMediaTypeFormatter())
                };

                return Task.FromResult(response);
            }
        }


    }
}