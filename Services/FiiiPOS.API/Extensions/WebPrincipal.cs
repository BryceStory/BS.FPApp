using System.Security.Principal;

#pragma warning disable 1591

namespace FiiiPOS.API.Extensions
{
    public class WebPrincipal : IPrincipal
    {
        public WebPrincipal(WebIdentity identity)
        {
            this.Identity = identity;
        }

        public bool IsInRole(string role)
        {
            return true;
        }

        public IIdentity Identity { get; }
    }
}