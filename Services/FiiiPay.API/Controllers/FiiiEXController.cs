using FiiiPay.Business;
using FiiiPay.DTO.FiiiEXLogin;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using System.Web.Http;
using FiiiPay.DTO;
using FiiiPay.DTO.FiiiExTransfer;
using FiiiPay.Entities;
using FiiiPay.Framework.Component;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// FiiiEX
    /// </summary>
    [RoutePrefix("FiiiEX")]
    public class FiiiEXController : ApiController
    {
        /// <summary>
        /// FiiiPay扫码登录FiiiEX
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
                if (codeEntity.QRCodeBusiness == QRCodeBusiness.FiiiPayLogin)
                {
                    result = new FiiiEXLoginComponent().FiiiPayLogin(im.Code.Replace('-', ':'), this.GetUser());
                }
            }
            if (!result)
                throw new Framework.Exceptions.CommonException(ReasonCode.INVALID_QRCODE, Framework.Properties.R.无效二维码);
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
            var accountId = this.GetUser().Id;

            return new ServiceResult<bool>
            {
                Data = new FiiiEXLoginComponent().HasExAccount(FiiiType.FiiiPay, accountId)
            };
        }

        /// <summary>
        /// 获取FiiiEx划转条件
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("TransferFiiiExCondition")]
        public ServiceResult<TransferFiiiExConditionOM> GetTransferFiiiExCondition(FiiiExBalanceIM im)
        {
            var result = new ServiceResult<TransferFiiiExConditionOM>();

            result.Data = new FiiiEXTransferComponent().FiiiPayTransferFiiiExCondition(this.GetUser(), im.CyptoId);

            result.Successful();
            return result;
        }

        /// <summary>
        /// 划转到 Ex
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("TransferToEx")]
        public ServiceResult<FiiiEXTransferComponent.TransferResult> TransferToEx(TransferToExIM model)
        {
            var result = new ServiceResult<FiiiEXTransferComponent.TransferResult>();

            result.Data = new FiiiEXTransferComponent().FiiiPayTransferToEx(this.GetUser(), model.CryptoId, model.Amount, model.PIN);

            result.Successful();
            return result;
        }

        /// <summary>
        /// 从 Ex 划转
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("TransferFromEx")]
        public ServiceResult<FiiiEXTransferComponent.TransferResult> TransferFromEx(TransferFromExIM model)
        {
            var result = new ServiceResult<FiiiEXTransferComponent.TransferResult>();

            result.Data = new FiiiEXTransferComponent().FiiiPayTransferFromEx(this.GetUser(), model.CryptoId, model.Amount, model.PIN);

            result.Successful();
            return result;
        }


        /// <summary>
        /// 划转详情详情
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Detail")]
        public ServiceResult<UserExTransferOrderOM> Detail(GetDetailByIdIM<long> im)
        {
            var result = new ServiceResult<UserExTransferOrderOM>();

            result.Data = new FiiiEXTransferComponent().FiiiPayTransferDetail(this.GetUser(), im.Id);
            result.Successful();
            return result;
        }
    }
}
