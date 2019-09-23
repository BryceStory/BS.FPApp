using FiiiPay.BackOffice.Common;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.BackOffice.BLL
{
    public class FiiiFinanceBLL
    {
        public SaveResult GetStatus(int CountryId, long RequestId)
        {
            try
            {
                var status = new FiiiFinanceAgent().GetStatus(RequestId);
                return new SaveResult(true, status.TotalConfirmation + " / " + status.MinRequiredConfirmation);
            }
            catch (CommonException ex)
            {
                return new SaveResult(false, ex.Message);
            }
        }
    }
}