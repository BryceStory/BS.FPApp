using System.Security.Principal;
using FiiiPay.Entities;

#pragma warning disable 1591

namespace FiiiPay.FiiiCoinWork.API.Extensions
{
    public class UserIdentity : IIdentity
    {
        public UserIdentity(InvestorAccount user)
        {
            User = user;
        }
        public string Name => User.Username;

        public string AuthenticationType => "";

        public bool IsAuthenticated => true;

        public InvestorAccount User { get; }
    }
}