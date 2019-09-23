using System;
using System.Web.Http;
using FiiiPay.CryptoCurrency.API.Models;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Wallet.Business.Crypotcurrencies;
using log4net;
using Newtonsoft.Json;

namespace FiiiPay.Wallet.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.CryptoCurrencyController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("CryptoCurrency")]
    [Authorize]
    public class CryptoCurrencyController : ApiController
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CryptoCurrencyController));

        private const int GENERAL_ERROR = 10000;

        ///// <summary>
        ///// Deposits ("即将过期，请使用 DepositPending | DepositCompleted | DepositCancel 代替")
        ///// </summary>
        ///// <returns></returns>
        //[Route("Deposit")]
        //[HttpPost, Obsolete("即将过期，请使用 DepositPending | DepositCompleted | DepositCancel 代替")]
        //public ServiceResult Deposit(DepositModel model)
        //{
        //    var result = new ServiceResult();
        //    //_logger.Info($"Deposit - Parameters - {JsonConvert.SerializeObject(model)}");

        //    try
        //    {
        //        var cpt = new CryptocurrencyComponent(model.AccountType);
        //        cpt.Deposit(model.AccountId, model.TransactionId, model.Amount, model.CryptoName,
        //            model.Timestamp, model.Address, model.Tag);
        //        result.Success();
        //        return result;
        //    }
        //    catch (CommonException ex)
        //    {
        //        _logger.Error($"Deposit - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}", ex);
        //        result.Failer(ex.ReasonCode, ex.Message);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error($"Deposit - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}", ex);
        //        result.Failer(GENERAL_ERROR, "System error");
        //        return result;
        //    }
        //}

        /// <summary>
        /// DepositPending this instance.
        /// </summary>
        /// <returns></returns>
        [Route("DepositPending")]
        [HttpPost]
        public ServiceResult DepositPending(DepositPendingModel model)
        {
            var result = new ServiceResult();
            _logger.Info($"DepositPending - Parameters - {JsonConvert.SerializeObject(model)}");

            try
            {
                var cryptocurrencyComponent = new CryptocurrencyComponent(model.AccountType);

                cryptocurrencyComponent.DepositPending(model.AccountId, model.DepositRequestId, model.TransactionId, model.Amount, model.CryptoName, model.Address, model.Tag);
                result.Success();
                return result;
            }
            catch (CommonException ex)
            {
                _logger.Error($"DepositPending - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}");
                result.Failer(ex.ReasonCode, ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"DepositPending - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}", ex);
                result.Failer(GENERAL_ERROR, "System error");
                return result;
            }
        }

        /// <summary>
        /// DepositCompleted this instance.
        /// </summary>
        /// <returns></returns>
        [Route("DepositCompleted")]
        [HttpPost]
        public ServiceResult DepositCompleted(DepositCompletedModel model)
        {
            var result = new ServiceResult();
            _logger.Info($"DepositCompleted - Parameters - {JsonConvert.SerializeObject(model)}");

            try
            {
                var cryptocurrencyComponent = new CryptocurrencyComponent(model.AccountType);

                cryptocurrencyComponent.DepositCompleted(model.AccountId, model.DepositRequestId, model.CryptoName, model.TransactionId, model.Amount, model.Address, model.Tag);
                result.Success();
                return result;
            }
            catch (CommonException ex)
            {
                _logger.Error($"DepositCompleted - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}");
                result.Failer(ex.ReasonCode, ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"DepositCompleted - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}", ex);
                result.Failer(GENERAL_ERROR, "System error");
                return result;
            }
        }
        /// <summary>
        /// DepositReject this instance.
        /// </summary>
        /// <returns></returns>
        [Route("DepositCancel")]
        [HttpPost]
        public ServiceResult DepositCancel(DepositCancelModel model)
        {
            var result = new ServiceResult();
            _logger.Info($"DepositCancel - Parameters - {JsonConvert.SerializeObject(model)}");

            try
            {
                var cryptocurrencyComponent = new CryptocurrencyComponent(model.AccountType);

                cryptocurrencyComponent.DepositCancel(model.AccountId, model.DepositRequestId, model.CryptoName);
                result.Success();
                return result;
            }
            catch (CommonException ex)
            {
                _logger.Error($"DepositCancel - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}");
                result.Failer(ex.ReasonCode, ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"DepositCancel - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}", ex);
                result.Failer(GENERAL_ERROR, "System error");
                return result;
            }
        }

        /// <summary>
        /// Withdraws the approved.
        /// </summary>
        /// <returns>The model.</returns>
        [HttpPost, Route("WithdrawApproved")]
        public ServiceResult WithdrawApproved(WithdrawApprovedModel model)
        {
            var result = new ServiceResult();
            _logger.Info($"WithdrawApproved - Parameters - {JsonConvert.SerializeObject(model)}");

            try
            {
                var cryptocurrencyComponent = new CryptocurrencyComponent(model.AccountType);

                cryptocurrencyComponent.WithdrawApproved(model.AccountId, model.WithdrawRequestId, model.CryptoName, model.TransactionId);

                result.Success();
                return result;
            }
            catch (CommonException ex)
            {
                _logger.Error($"WithdrawApproved - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}");
                result.Failer(ex.ReasonCode, ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"WithdrawApproved - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}", ex);
                result.Failer(GENERAL_ERROR, "System error");
                return result;
            }
        }

        /// <summary>
        /// Withdraws the completed.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [Route("WithdrawCompleted")]
        [HttpPost]
        public ServiceResult WithdrawCompleted(WithdrawCompletedModel model)
        {
            var result = new ServiceResult();
            _logger.Info($"WithdrawCompleted - Parameters - {JsonConvert.SerializeObject(model)}");

            try
            {
                var cryptocurrencyComponent = new CryptocurrencyComponent(model.AccountType);

                cryptocurrencyComponent.WithdrawCompleted(model.AccountId, model.WithdrawRequestId, model.CryptoName, model.TransactionId);

                result.Success();
                return result;
            }
            catch (CommonException ex)
            {
                _logger.Error($"WithdrawCompleted - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}");
                result.Failer(ex.ReasonCode, ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"WithdrawCompleted - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}", ex);
                result.Failer(GENERAL_ERROR, "System error");
                return result;
            }
        }

        /// <summary>
        /// Withdraws the reject.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [Route("WithdrawReject")]
        [HttpPost]
        public ServiceResult WithdrawReject(WithdrawRejectModel model)
        {
            var result = new ServiceResult();
            _logger.Info($"WithdrawReject - Parameters - {JsonConvert.SerializeObject(model)}");

            try
            {
                var cryptocurrencyComponent = new CryptocurrencyComponent(model.AccountType);

                cryptocurrencyComponent.WithdrawReject(model.AccountId, model.WithdrawRequestId, model.CryptoName, model.ReasonMessage);

                result.Success();
                return result;
            }
            catch (CommonException ex)
            {
                _logger.Error($"WithdrawReject - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}");
                result.Failer(ex.ReasonCode, ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"WithdrawReject - ErrorMessage - {JsonConvert.SerializeObject(model)} - {ex.Message}", ex);
                result.Failer(GENERAL_ERROR, "System error");
                return result;
            }
        }
    }
}
