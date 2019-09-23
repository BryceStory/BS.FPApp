using System;
using System.Web.Services;
using FiiiPay.Business;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using log4net;

namespace FiiiPay.FiiiExTransfer.WebService
{
    /// <summary>
    /// CoinTransfer 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class CoinTransfer : System.Web.Services.WebService
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CoinTransfer));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="pin"></param>
        /// <param name="coinCode"></param>
        /// <param name="amount"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [WebMethod(Description = "划入加密币")]
        public int TransferInto(Guid openId, string pin, string coinCode, decimal amount, out string orderNo)
        {
            try
            {
                _logger.Info($"Transfer Into: openId={openId}, pin={pin}, coinCode={coinCode}, amount={amount} ");
                orderNo = new FiiiEXTransferComponent().TransferInto(openId, pin, coinCode, amount);
                _logger.Info($"Result: orderNo={orderNo}");
                return 0;
            }
            catch (CommonException ex)
            {
                _logger.Warn(ex.Message, ex);
                orderNo = string.Empty;
                return ex.ReasonCode;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                orderNo = string.Empty;
                return ReasonCode.GENERAL_ERROR;
            }
        }
    }
}
