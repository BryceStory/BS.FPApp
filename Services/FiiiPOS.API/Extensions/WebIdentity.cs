using System.Security.Principal;

#pragma warning disable 1591
namespace FiiiPOS.API.Extensions
{
    public class WebIdentity : IIdentity
    {
        public WebIdentity(WebContext context)
        {
            this.WebContext = context;
        }
        public string Name => WebContext.Name;
        public string AuthenticationType => "";
        public bool IsAuthenticated => true;

        public WebContext WebContext { get; set; }
    }
}