using FiiiPay.Business;
using FiiiPay.DTO;
using FiiiPay.DTO.Withdraw;
using FiiiPay.Framework;
using System.Web.Http;
using FiiiPay.API.Models;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.WithdrawController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Withdraw")]
    public class WithdrawController : ApiController
    {
        /// <summary>
        /// 提币详情
        /// </summary>
        /// <param name="im">im.Id = OrderId</param>
        /// <returns></returns>
        [HttpPost, Route("Detail")]
        public ServiceResult<WithdrawDetailOM> Detail(GetDetailByIdIM<long> im)
        {
            return new ServiceResult<WithdrawDetailOM>
            {
                Data = new UserWithdrawComponent().Detail(this.GetUser(), im.Id, this.IsZH())
            };
        }

        /// <summary>
        /// 准备提币
        /// </summary>
        /// <param name="im">im.Id = CoinId</param>
        /// <returns></returns>
        [HttpPost, Route("PreWithdraw")]
        public ServiceResult<PreWithdrawOM> PreWithdraw(GetDetailByIdIM<int> im)
        {
            return new ServiceResult<PreWithdrawOM>
            {
                Data = new UserWithdrawComponent().PreWithdraw(this.GetUser(), im.Id)
            };
        }

        /// <summary>
        /// 获取提币地址列表
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("AddressList")]
        public ServiceResult<AddressListOM> AddressList(GetDetailByIdIM<int> im)
        {
            return new ServiceResult<AddressListOM>
            {
                Data = new UserWithdrawComponent().AddressList(this.GetUser(), im.Id)
            };
        }

        /// <summary>
        /// 提币时，验证PIN码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("VerifyWithdrawPIN")]
        public ServiceResult<bool> VerifyWithdrawPIN(VerifyPinModel model)
        {
            new UserWithdrawComponent().VerifyWithdrawPIN(this.GetUser().Id, model.PIN);

            return new ServiceResult<bool>(){Data = true};
        }

        /// <summary>
        /// 提币时，综合验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns>token</returns>
        [HttpPost, Route("VerifyWithdrawIMCombine")]
        public ServiceResult<bool> VerifyWithdrawIMCombine(WithdrawCombineVerifyModel model)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();

            var accountId = this.GetUser().Id;
            new UserWithdrawComponent().VerifyWithdrawCombine(accountId, model.SMSCode, model.GoogleCode, model.DivisionCode);
            result.Data = true;
            return result;
        }

        /// <summary>
        /// 提币
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Withdraw")]
        public ServiceResult<WithdrawOM> Withdraw(WithdrawIM im)
        {
            return new ServiceResult<WithdrawOM>
            {
                Data = new UserWithdrawComponent().Withdraw(this.GetUser(), im, this.GetClientIPAddress())
            };
        }

        /// <summary>
        /// 获取手续费率以及手续费
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("TransactionFeeRate")]
        public ServiceResult<TransactionFeeRateOM> TransactionFeeRate(TransactionFeeRateIM im)
        {
            return new ServiceResult<TransactionFeeRateOM>
            {
                Data = new UserWithdrawComponent().TransactionFeeRate(im.CoinId, im.TargetAddress, im.TargetTag)
            };
        }

        /// <summary>
        /// 地址管理，列出所有币种以及地址数量
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("ListForManageWithdrawalAddress")]
        public ServiceResult<ListForManageWithdrawalAddressOM> ListForManageWithdrawalAddress()
        {
            return new ServiceResult<ListForManageWithdrawalAddressOM>
            {
                Data = new ListForManageWithdrawalAddressOM
                {
                    List = new UserWithdrawComponent().ListForManageWithdrawalAddress(this.GetUser())
                }
            };
        }

        /// <summary>
        /// 添加提币地址
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("AddAddress")]
        public ServiceResult<AddAddressOM> AddAddress(AddAddressIM im)
        {
            return new ServiceResult<AddAddressOM>
            {
                Data = new UserWithdrawComponent().AddAddress(this.GetUser(), im.Address, im.Tag, im.Alias, im.CoinId)
            };
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("DeleteAddress")]
        public ServiceResult<bool> DeleteAddress(GetDetailByIdIM<long> im)
        {
            new UserWithdrawComponent().DeleteAddress(this.GetUser(), im.Id);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        ///// <summary>
        ///// Manals the withdraw.
        ///// </summary>
        ///// <param name="id">The identifier.</param>
        ///// <returns></returns>
        //[HttpGet, Route("ManalWithdraw"), AllowAnonymous]
        //public ServiceResult<bool> ManalWithdraw(int id)
        //{
        //    try
        //    {
        //        return new ServiceResult<bool>
        //        {
        //            Code = 0,
        //            Data = new UserWithdrawComponent().ManalWithdraw(id)
        //        };
        //    }
        //    catch //(Exception exception)
        //    {
        //        //_log.ErrorFormat("Manal withdraw " + exception.Message);
        //    }
        //    return new ServiceResult<bool>();
        //}
    }
}
