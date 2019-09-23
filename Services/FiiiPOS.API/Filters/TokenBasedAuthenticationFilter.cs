using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using FiiiPay.Framework;
using FiiiPay.Framework.Constants;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Exceptions;
using FiiiPOS.API.Extensions;
using FiiiPOS.Business;
using FiiiPOS.Business.FiiiPOS;

#pragma warning disable 1591

namespace FiiiPOS.API.Filters
{
    public class TokenBasedAuthenticationFilter : IAuthenticationFilter, IFilter
    {
        private readonly string _authenticationType;

        /// <summary>Gets the authentication type of the OWIN middleware to use.</summary>
        /// <returns>The authentication type of the OWIN middleware to use.</returns>
        public string AuthenticationType
        {
            get
            {
                return this._authenticationType;
            }
        }

        /// <summary>Gets a value indicating whether the filter allows multiple authentication.</summary>
        /// <returns>true if the filter allows multiple authentication; otherwise, false.</returns>
        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

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
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            HttpRequestMessage request = context.Request;
            if (request == null)
                throw new InvalidOperationException("Request must not be null");

            if (context.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0)
                return Task.FromResult(0);

            if (context.Request.Headers.Authorization == null)
                return Unauthorized(context);

            try
            {
                var authHeader = request.Headers.Authorization.Parameter;
                var accessToken = AccessTokenGenerator.DecryptToken<MerchantAccessToken>(authHeader);
                if (string.IsNullOrWhiteSpace(accessToken.Identity))
                {
                    var merchant = new MerchantAccountComponent().GetMerchantAccountBySN(accessToken.POSSN);
                    if(merchant == null) return Unauthorized(context);
                    accessToken.Identity = merchant.Username;
                }
                var cacheKey = $"{RedisKeys.FiiiPOS_APP_MerchantId}:{accessToken.Identity}";

                var cacheToken = RedisHelper.StringGet(Constant.REDIS_TOKEN_DBINDEX, cacheKey);

                if (authHeader != cacheToken)
                    return Unauthorized(context);

                var bc = new MerchantAccountComponent();
                var account = bc.GetByPosSn(accessToken.POSSN, accessToken.Identity);
                if (account == null)
                    return Unauthorized(context);

                bc.ChangeLanguage(account.Id, context.Request.Headers.AcceptLanguage.FirstOrDefault()?.Value);

                var webContext = new WebContext
                {
                    Id = account.Id,
                    CountrtId = account.CountryId,
                    Name = account.MerchantName
                };

                context.Principal = new WebPrincipal(new WebIdentity(webContext));

                return Task.FromResult(0);
            }
            catch (CommonException ex)
            {
                return CommonErrorResult(context, ex);
            }
            catch (Exception)
            {
                return Unauthorized(context);
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

        private static Task Unauthorized(HttpAuthenticationContext context)
        {
            context.ErrorResult = new ErrorResult
            {
                Code = ReasonCode.UNAUTHORIZED,
                Message = "Unauthorized"
            };

            return Task.FromResult(0);
        }

        private static Task CommonErrorResult(HttpAuthenticationContext context, CommonException ex)
        {
            context.ErrorResult = new ErrorResult
            {
                Code = ex.ReasonCode,
                Message = ex.Message
            };

            return Task.FromResult(0);
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