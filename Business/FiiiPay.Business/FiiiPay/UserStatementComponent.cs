using FiiiPay.Data;
using FiiiPay.DTO.Statement;
using FiiiPay.Entities;
using FiiiPay.Framework;
using System.Linq;
using FiiiPay.Business.Properties;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Enums;
using FiiiPay.DTO;
using System.Threading.Tasks;

namespace FiiiPay.Business
{
    public class UserStatementComponent : BaseComponent
    {
        public async Task<ListOM> ListAllType(UserAccount user, ListAllTypeIM im, bool isZH)
        {
            im.PageSize = im.PageSize ?? 20;
            if (im.PageSize > 20)
                im.PageSize = 20;

            var esList = await new UserTransactionDAC().GetListAsync(user.Id,null, im.CoinId, im.PageSize.Value, im.PageIndex ?? 0, im.Mounth, im.StartDate, im.EndDate, im.Keyword, im.MaxType);
            var coinList = await new CryptocurrencyDAC().GetAllAsync();

            return new ListOM
            {
                CurrentPageIndex = im.PageIndex ?? 0,
                List = esList.Select(a => new ListOMItem
                {
                    CryptoAmount = a.Amount.ToString(),
                    Code = a.CryptoCode,
                    //IconUrl = coin?.IconURL,
                    OrderId = a.DetailId,
                    StatusStr = GetStatusStr((int)a.Type, a.Status, isZH),
                    Status = a.Status,
                    RefundStatus = GetRefundStatus((int)a.Type, a.Status),
                    RefundStatusStr = GetRefundStatusStr((int)a.Type, a.Status, isZH),
                    Timestamp = a.Timestamp.ToUtcTimeTicks().ToString(),
                    Type = (int)a.Type
                }).ToList()
            };
        }

        public async Task<ListOM> ListSingleType(UserAccount user, ListSingleTypeIM im, bool isZH)
        {
            im.PageSize = im.PageSize ?? 20;
            if (im.PageSize > 20)
                im.PageSize = 20;

            var esList = await new UserTransactionDAC().GetListAsync(user.Id, im.Type, im.CoinId, im.PageSize.Value, im.PageIndex ?? 0, im.Mounth, im.StartDate, im.EndDate, im.Keyword, im.MaxType);
            var coinList = await new CryptocurrencyDAC().GetAllAsync();

            return new ListOM
            {
                CurrentPageIndex = im.PageIndex ?? 0,
                List = esList.Select(a => new ListOMItem
                {
                    CryptoAmount = a.Amount.ToString(),
                    Code = a.CryptoCode,
                    //IconUrl = coin?.IconURL,
                    OrderId = a.DetailId,
                    StatusStr = GetStatusStr((int)a.Type, a.Status, isZH),
                    Status = (a.Status > (int)RedPocketStatus.Complate ? (int)RedPocketStatus.Complate : a.Status),
                    RefundStatus = GetRefundStatus((int)a.Type, a.Status),
                    RefundStatusStr = GetRefundStatusStr((int)a.Type, a.Status, isZH),
                    Timestamp = a.Timestamp.ToUtcTimeTicks().ToString(),
                    Type = (int)a.Type
                }).ToList()
            };
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="type">0：充币，1：提币，2：订单，3：退款</param>
        /// <param name="status"></param>
        /// <param name="isZH"></param>
        /// <returns></returns>
        public string GetStatusStr(int type, int status, bool isZH)
        {
            var orderStatusList = new string[] { "", GetR("OrderPending", isZH), GetR("OrderCompleted", isZH), GetR("OrderRefunded", isZH), GetR("OrderClosed", isZH) };
            var depositStatusList = new string[] { GetR("DepositPending", isZH), GetR("DepositConfirmed", isZH), GetR("DepositPending", isZH), GetR("DepositCancelled", isZH) };
            var inviteStatusList = new string[] { "", GetR("DepositPending", isZH), GetR("DepositConfirmed", isZH), GetR("DepositCancelled", isZH) };
            var billerOrderStatusList = new[] {GetR("BillerOrderPending", isZH), GetR("BillerOrderFail", isZH), GetR("BillerOrderComplete", isZH)};
            switch (type)
            {
                case (int)BillType.Deposit:
                    return depositStatusList[status];
                case 1:
                    return depositStatusList[status];
                case 2:
                    return orderStatusList[status];
                case 3:
                    return GetR("OrderRefunded", isZH);
                case 4:
                case 5:
                //return GetR("OrderCompleted", isZH);
                case 6:
                case 7:
                    return GetR("OrderCompleted", isZH);
                case 8:
                    return inviteStatusList[status];
                case (int)BillType.GatewayOrder:
                    return orderStatusList[status];
                case (int)BillType.RefundGatewayOrder:
                    return GetR("OrderRefunded", isZH);
                case 11:
                    return billerOrderStatusList[status];
                case 12:
                case 13:
                    return orderStatusList[status];
                case 14:
                    var pRedPocketStatusList = GetR("RedPocketStatus", isZH).Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                    return status == (int)RedPocketStatus.Actived ? pRedPocketStatusList[0] : pRedPocketStatusList[1];
                case 15:
                    var rRedPocketStatusList = GetR("RedPocketStatus", isZH).Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                    return rRedPocketStatusList[1];
                default:
                    return "";
            }
        }

        private int GetRefundStatus(int type, int status)
        {
            if (type != 14)
                return 0;
            switch (status)
            {
                case (int)RedPocketStatus.Actived:
                case (int)RedPocketStatus.Complate:
                    return 0;
                case (int)RedPocketStatus.Refund:
                    return 1;
                case (int)RedPocketStatus.FullRefund:
                    return 2;
                default:
                    return 0;
            }
        }
        
        public string GetRefundStatusStr(int type, int status, bool isZH)
        {
            var redPocketRefundStatus = GetR("RedPocketStatus", isZH).Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            switch (type)
            {
                case 14:
                    if (status <= (int)RedPocketStatus.Complate)
                        return "";
                    return redPocketRefundStatus[status];
                default:
                    return "";
            }
        }

        string GetR(string key, bool isZH)
        {
            return Resources.ResourceManager.GetString(key, new System.Globalization.CultureInfo(isZH ? "zh" : "en"));
        }
    }
}
