using System.Collections.Generic;
using FiiiPay.Business;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using FiiiPOS.API.Extensions;
using FiiiPOS.DTO;
using System.Web.Http;
using FiiiPay.Entities;
using FiiiPay.Framework.Component;
using FiiiPOS.API.Models;
using Newtonsoft.Json;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// FiiiEX
    /// </summary>
    public class FiiiEXController : ApiController
    {
        /// <summary>
        /// FiiiPos扫码登录FiiiEX
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Login")]
        public ServiceResult<bool> Login(FiiiEXLoginIM im)
        {
            var codeEntity = QRCode.Deserialize(im.Code);
            bool result = false;
            if (codeEntity.SystemPlatform == SystemPlatform.FiiiEX)
            {
                if (codeEntity.QRCodeBusiness == QRCodeBusiness.FiiiPosLogin)
                    result = new FiiiEXLoginComponent().FiiiPosLogin(im.Code.Replace('-', ':'), this.GetMerchantAccountId());
            }
            if (!result)
                throw new FiiiPay.Framework.Exceptions.CommonException(ReasonCode.INVALID_QRCODE, FiiiPay.Framework.Properties.R.无效二维码);
            return new ServiceResult<bool>
            {
                Data = result
            };
        }


        /// <summary>
        /// 是否存在 Ex 帐号
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("HasExAccount")]
        public ServiceResult<bool> HasExAccount()
        {
            var result = new ServiceResult<bool>();

            result.Data = new FiiiEXLoginComponent().HasExAccount(FiiiType.FiiiPOS, this.GetMerchantAccountId());
            result.Successful();
            return result;
        }

        /// <summary>
        /// 获取FiiiEx划转条件
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("TransferFiiiExCondition")]
        public ServiceResult<TransferFiiiExConditionDTO> GetTransferFiiiExCondition(int cryptoId)
        {
            var result = new ServiceResult<TransferFiiiExConditionDTO>();

            result.Data = new FiiiEXTransferComponent().FiiiPOSTransferFiiiExCondition(this.GetMerchantAccountId(), cryptoId);

            result.Successful();
            return result;
        }

        /// <summary>
        /// 划转到 Ex
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("TransferToEx")]
        public ServiceResult<FiiiEXTransferComponent.TransferResult> TransferToEx(TransferToExModel model)
        {
            var result = new ServiceResult<FiiiEXTransferComponent.TransferResult>();

            result.Data = new FiiiEXTransferComponent().FiiiPOSTransferToEx(this.GetMerchantAccountId(), model.CryptoId, model.Amount, model.PINToken);

            result.Successful();
            return result;
        }

        /// <summary>
        /// 从 Ex 划转
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("TransferFromEx")]
        public ServiceResult<FiiiEXTransferComponent.TransferResult> TransferFromEx(TransferFromExModel model)
        {
            var result = new ServiceResult<FiiiEXTransferComponent.TransferResult>();

            result.Data = new FiiiEXTransferComponent().FiiiPOSTransferFromEx(this.GetMerchantAccountId(), model.CryptoId, model.Amount, model.PINToken);

            result.Successful();
            return result;
        }

        /// <summary>
        /// 划转列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet, Route("List")]
        public ServiceResult<List<MerchantExTransferOrderDTO>> List(int pageIndex = 1, int pageSize = 20)
        {
            var result = new ServiceResult<List<MerchantExTransferOrderDTO>>();
            result.Data = new FiiiEXTransferComponent().FiiiPOSTransferList(this.GetMerchantAccountId(), pageIndex, pageSize);
            result.SuccessfulWithExtension(JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, PageCount = result.Data.Count }));
            return result;
        }


        /// <summary>
        /// 划转详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("Detail")]
        public ServiceResult<MerchantExTransferOrderDTO> Detail(long id)
        {
            var result = new ServiceResult<MerchantExTransferOrderDTO>();

            result.Data = new FiiiEXTransferComponent().FiiiPOSTransferDetail(this.GetMerchantAccountId(), id);
            result.Successful();
            return result;
        }
    }
}
