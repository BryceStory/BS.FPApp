using FiiiPay.BackOffice.Models;

namespace FiiiPay.BackOffice.BLL
{
    public class ActionLogBLL :BaseBLL
    {
        public void Create(ActionLog log) {
            BoDB.ActionLogDb.Insert(log);
        }
    }
}