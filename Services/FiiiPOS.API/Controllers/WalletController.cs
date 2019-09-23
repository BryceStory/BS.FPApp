using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using FiiiPOS.API.Models;
using FiiiPOS.Business.FiiiPOS;
using FiiiPOS.DTO;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 钱包
    /// </summary>
    [RoutePrefix("api/Wallet")]
    public class WalletController : ApiController
    {
        /// <summary>
        /// 币种列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("List")]
        public ServiceResult<List<CryptocurrencyDTO>> List()
        {
            var result = new ServiceResult<List<CryptocurrencyDTO>>();

            result.Data = new MerchantWalletComponent().GetMerchentCryptoList(this.GetMerchantAccountId()).ToList();

            result.Success();
            return result;
        }

        /// <summary>
        /// 设置钱包
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Setting")]
        public ServiceResult Setting(SettingCrytocurrencyModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var component = new MerchantWalletComponent();

            var accountId = this.GetMerchantAccountId();

            component.SettingCrytocurrencies(accountId, model.CryptoIds);

            result.Success();
            return result;
        }

        /// <summary>
        /// 商户总资产
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("MerchantTotalAssets")]
        public ServiceResult<MerchantTotalAssetsDTO> MerchantTotalAssets()
        {
            var result = new ServiceResult<MerchantTotalAssetsDTO>();

            var accountId = this.GetMerchantAccountId();
            result.Data = new MerchantWalletComponent().GetMerchantTotalAssets(accountId);
            result.Success();
            return result;
        }

        /// <summary>
        /// 获取商家币种列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("MerchantWalletList")]
        public ServiceResult<List<MerchantWalletDTO>> MerchantWalletList()
        {
            var result = new ServiceResult<List<MerchantWalletDTO>>();

            result.Data = new MerchantWalletComponent().GetMerchantWalletList(this.GetMerchantAccountId());
            result.Success();
            return result;
        }

        /// <summary>
        /// 根据币种Id获取商户钱包信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("MerchantWalletInfo")]
        public ServiceResult<MerchantWalletDTO> MerchantWalletInfo(int cryptoId)
        {
            var result = new ServiceResult<MerchantWalletDTO>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();

            result.Data = new MerchantWalletComponent().GetWalletInfoById(accountId, cryptoId);

            result.Success();
            return result;
        }

        /// <summary>
        /// 获取钱包充币地址
        /// </summary>
        /// <param name="cryptoId">商户钱包币种ID</param>
        /// <returns></returns>
        [HttpGet, Route("GetDepositAddress")]
        public ServiceResult<DepositAddressInfo> GetDepositAddress(int cryptoId)
        {
            var result = new ServiceResult<DepositAddressInfo>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();

            result.Data = new MerchantWalletComponent().GetDepositAddressById(accountId, cryptoId);

            result.Success();
            return result;
        }

        /// <summary>
        /// 获取钱包提币限额，手续费等
        /// </summary>
        /// <param name="cryptoId">商户钱包币种ID</param>
        /// <returns></returns>
        [HttpGet, Route("GetWithdrawalInfo")]
        public ServiceResult<WithdrawalConditionInfo> GetWithdrawalInfo(int cryptoId)
        {
            var result = new ServiceResult<WithdrawalConditionInfo>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();

            result.Data = new MerchantWalletComponent().GetWithdrawalInfo(accountId, cryptoId);

            result.Success();
            return result;
        }

        /// <summary>
        /// 获取手续费率
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("TransactionFeeRate")]
        public ServiceResult<TransactionFeeDTO> TransactionFeeRate(TransactionFeeRateModel model)
        {
            var result = new ServiceResult<TransactionFeeDTO>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            result.Data = new MerchantWalletComponent().TransactionFeeRate(model.CoinId, model.TargetAddress, model.TargetTag);
            result.Success();
            return result;
        }

        /// <summary>
        /// 提币时，验证PIN码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("VerifyWithdrawPIN")]
        public ServiceResult<bool> VerifyWithdrawPIN(GetPINTokenModel model)
        {
            var result = new ServiceResult<bool>();
            new MerchantWalletComponent().VerifyWithdrawPIN(this.GetMerchantAccountId(), model.PIN);
            result.Data = true;
            return result;
        }

        /// <summary>
        /// 提币时，综合验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns>token</returns>
        [HttpPost, Route("VerifyWithdrawIMCombine")]
        public ServiceResult<bool> VerifyWithdrawIMCombine(WithdrawCombineVerifyModel model)
        {
            var result = new ServiceResult<bool>();

            new MerchantWalletComponent().VerifyWithdrawCombine(this.GetMerchantAccountId(), model.SMSCode, model.GoogleCode, model.DivisionCode);
            result.Data = true;
            return result;
        }

        /// <summary>
        /// 提币
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Withdrawal")]
        public ServiceResult<WithdrawalResult> Withdrawal(WithdrawalModel model)
        {
            var result = new ServiceResult<WithdrawalResult>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            var ip = this.GetClientIPAddress();

            var accountId = this.GetMerchantAccountId();

            var data = new MerchantWalletComponent().Withdrawal(accountId, model.Amount, model.CryptoId, model.Address, model.Tag, ip);
            result.Data = new WithdrawalResult
            {
                Id = data.Id,
                Timestamp = data.Timestamp.ToUnixTime(),
                OrderNo = data.OrderNo
            };

            result.Success();
            return result;
        }

        ///// <summary>
        ///// Manals the withdraw.
        ///// </summary>
        ///// <param name="id">The identifier.</param>
        ///// <returns></returns>
        //[HttpGet, Route("ManalWithdraw"), AllowAnonymous]
        //public ServiceResult<bool> ManalWithdraw(int id)
        //{
        //    return new ServiceResult<bool>
        //    {
        //        Code = 0,
        //        Data = new MerchantWalletComponent().ManalWithdraw(id)
        //    };
        //}
    }
}
