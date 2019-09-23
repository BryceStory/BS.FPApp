using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using FiiiPay.Business;
using FiiiPay.Entities;
using FiiiPay.FiiiCoinWork.API.Extensions;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Constants;
using FiiiPay.Framework.Enums;
using Newtonsoft.Json;

#pragma warning disable 1591

namespace FiiiPay.FiiiCoinWork.API.Filters
{
    public class TokenBasedAuthenticationFilter : IAuthenticationFilter, IFilter
    {
        //private static readonly Lazy<ConnectionMultiplexer> lazyConnection =
        //    new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisConnectionString"]));

        //private static ConnectionMultiplexer Connection => lazyConnection.Value;

        private readonly string _authenticationType;

        /// <summary>Gets the authentication type of the OWIN middleware to use.</summary>
        /// <returns>The authentication type of the OWIN middleware to use.</returns>
        public string AuthenticationType => this._authenticationType;

        /// <summary>Gets a value indicating whether the filter allows multiple authentication.</summary>
        /// <returns>true if the filter allows multiple authentication; otherwise, false.</returns>
        public bool AllowMultiple => false;

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Http.BaseAuthenticationFilter" /> class.</summary>
        /// <param name="authenticationType">The authentication type of the OWIN middleware to use.</param>
        public TokenBasedAuthenticationFilter()
        {
            this._authenticationType = "Bearer";
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Http.BaseAuthenticationFilter" /> class.</summary>
        /// <param name="authenticationType">The authentication type of the OWIN middleware to use.</param>
        public TokenBasedAuthenticationFilter(string authenticationType)
        {
            if (authenticationType == null)
            {
                throw new ArgumentNullException(nameof(authenticationType));
            }
            this._authenticationType = authenticationType;
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
            HttpRequestMessage request = context.Request;
            if (request == null)
            {
                throw new InvalidOperationException("Request must not be null");
            }

            if (context.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0)
            {
                return;
            }

            if (context.Request.Headers.Authorization == null)
            {
                Unauthorized(context);
                return;
            }

            try
            {
                var authHeader = request.Headers.Authorization.Parameter;
                var accessToken = JsonConvert.DeserializeObject<AccessToken>(AccessTokenGenerator.DecryptToken(authHeader));

                string cacheToken = RedisHelper.StringGet($"{SystemPlatform.FiiiCoinWork}:Investor:{accessToken.Username}");

                if (authHeader == cacheToken)
                {
                    var bc = new InvestorAccountComponent();
                    InvestorAccount account = bc.GetByUsername(accessToken.Username);

                    if (account.Status == AccountStatus.Locked)
                    {
                        AccountLocked(context);
                        return;
                    }

                    string lan = context.Request.Headers.AcceptLanguage.FirstOrDefault()?.Value?? "en";
                    RedisHelper.StringSet($"{SystemPlatform.FiiiCoinWork}:Language:{account.Username}", lan);

                    SetSecurityPrincipal(ref context, account);
                }
                else
                {
                    Unauthorized(context);
                }
            }
            catch (Exception)
            {
                Unauthorized(context);
            }
        }

        private void SetSecurityPrincipal(ref HttpAuthenticationContext context, InvestorAccount account)
        {
            UserIdentity identity = new UserIdentity(account);

            context.Principal = new UserPrincipal(identity);
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

        private static void Unauthorized(HttpAuthenticationContext context)
        {
            context.ErrorResult = new ErrorResult
            {
                Code = ReasonCode.UNAUTHORIZED,
                Message = "Unauthorized"
            };
        }

        private static void AccountLocked(HttpAuthenticationContext context)
        {
            context.ErrorResult = new ErrorResult
            {
                Code = ReasonCode.ACCOUNT_LOCKED,
                Message = "Account is locked"
            };
        }

        class ErrorResult : ServiceResult<string>, IHttpActionResult
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