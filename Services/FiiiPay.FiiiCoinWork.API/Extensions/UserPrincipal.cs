using System.Security.Principal;
#pragma warning disable 1591

namespace FiiiPay.FiiiCoinWork.API.Extensions
{
    public class UserPrincipal: IPrincipal
    {
        public UserPrincipal(UserIdentity identity)
        {
            this._identity = identity;
        }
        private IIdentity _identity;
        public bool IsInRole(string role)
        {
            return false;
        }

        public IIdentity Identity { get { return _identity; } }
    }
}