using System;
using System.Web.Http;
using FiiiPay.CryptoCurrency.API.Models;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Wallet.Business;
using log4net;
using Newtonsoft.Json;

namespace FiiiPay.Wallet.API.Controllers
{
    /// <summary>
    /// 挖矿奖励分配
    /// </summary>
    [RoutePrefix("Mining")]
    [Authorize]
    public class MiningController : ApiController
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MiningController));
        private const int GENERAL_ERROR = 10000;

        /// <summary>
        /// 接收到挖矿成功通知
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Received")]
        [HttpPost]
        public ServiceResult Received(MiningReceivedModel model)
        {
            var result = new ServiceResult();
            //_logger.Info($"Mining-Received - Parameters - {JsonConvert.SerializeObject(model)}");

            try
            {
                var mc = new MiningComponent();

                mc.MiningReceived(model.RequestId, model.TransactionId, model.AccountType, model.AccountId, model.Amount);

                result.Success();
                return result;
            }
            catch (CommonException ex)
            {
                _logger.Error($"Mining-Received - ErrorMessage - {ex.Message}, Parameters - {JsonConvert.SerializeObject(model)}");
                result.Failer(ex.ReasonCode, ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"Mining-Received - ErrorMessage - {ex.Message}, Parameters - {JsonConvert.SerializeObject(model)}", ex);
                result.Failer(GENERAL_ERROR, "System error");
                return result;
            }
        }

        /// <summary>
        /// 接收到挖矿确认通知
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Confirmed")]
        [HttpPost]
        public ServiceResult Confirmed(MiningConfirmedModel model)
        {
            var result = new ServiceResult();
            /////////////////////////
            //receive接口已经直接加到了用户余额里，confirmed已不需要任何动作
            ////////////////////////
            result.Success();
            return result;

            //_logger.Info($"Mining-Confirmed - Parameters - {JsonConvert.SerializeObject(model)}");
            //_logger.Info($"Client IP - {this.GetClientIPAddress()}");

            //try
            //{
            //    if (model.MiningConfirmedList == null || model.MiningConfirmedList.Count <= 0)
            //    {
            //        result.Success();
            //        return result;
            //    }
                
            //    MiningComponent mc = new MiningComponent();
            //    foreach (var miningConfirmed in model.MiningConfirmedList)
            //    {
            //        try
            //        {
            //            mc.MiningConfirmed(miningConfirmed.AccountType, miningConfirmed.AccountId, miningConfirmed.Amount);
            //        }
            //        catch
            //        {
            //            _logger.Error($"Mining-Confirmed - Single Error - {JsonConvert.SerializeObject(miningConfirmed)}");
            //        }
            //    }

            //    result.Success();
            //    return result;
            //}
            //catch (CommonException ex)
            //{
            //    _logger.Error($"Mining-Confirmed - ErrorMessage - {ex.Message}", ex);
            //    result.Failer(ex.ReasonCode, ex.Message);
            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error($"Mining-Confirmed - ErrorMessage - {ex.Message}", ex);
            //    result.Failer(GENERAL_ERROR, "System error");
            //    return result;
            //}
        }
    }
}
