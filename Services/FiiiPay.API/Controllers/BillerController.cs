using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FiiiPay.API.Filters;
using FiiiPay.Business.FiiiPay;
using FiiiPay.DTO;
using FiiiPay.Framework;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 生活缴费
    /// </summary>
    [RoutePrefix("Biller"),BillerForbidden]
    public class BillerController : ApiController
    {
        /// <summary>
        /// 付款
        /// </summary>
        /// <param name="im"></param>
        /// 10401: 超过单笔 10402: 超过当日 10403: 超过当月 10404: 金额有误 10405: 国家信息不对
        /// <returns></returns>
        [HttpPost, Route("Pay")]
        public ServiceResult<BillerPayOM> Pay(BillerPayIM im)
        {
            int code = 0;
            var data = new BillerComponent().Pay(this.GetUser(), im, ref code);
            
            return new ServiceResult<BillerPayOM>()
            {
                Data = data,
                Code = code
            };
        }
        
        /// <summary>
        /// 缴费详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("Detail")]
        public ServiceResult<BillerDetailOM> Detail(BillerDetailIM im)
        {
            return new ServiceResult<BillerDetailOM>()
            {
                Data = new BillerComponent().Detail(this.GetUser(), im.Id)
            };
        }
        /// <summary>
        /// 添加地址 如果返回码为0则为添加成功 10406: 已经达到最大值500条无法添加
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("AddAddress")]
        public ServiceResult<bool> AddAddress(BillerAddAddressIM im)
        {
            new BillerComponent().AddAddress(this.GetUser(), im);
            return new ServiceResult<bool>()
            {
                Data = true,
                Code = 0
            };
        }

        /// <summary>
        /// 添加地址 如果返回码为0则为删除成功
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("DeleteAddress")]
        public ServiceResult<bool> DeleteAddress(BillerDeleteAddressIM im)
        {
            new BillerComponent().DeleteAddress(this.GetUser().Id, im.Id);
            return new ServiceResult<bool>()
            {
                Data = true,
                Code = 0
            };
        }

        /// <summary>
        /// 添加地址 如果返回码为0则为修改成功
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("EditAddress")]
        public ServiceResult<bool> EditAddress(BillerEditAddressIM im)
        {
            new BillerComponent().EditAddress(this.GetUser(), im);
            return new ServiceResult<bool>()
            {
                Data = true,
                Code = 0
            };
        }

        /// <summary>
        /// 返回地址列表
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAddresses")]
        public ServiceResult<List<BillerAddressOM>> GetAddresses(BillerGetAddressIM im)
        {
            return new ServiceResult<List<BillerAddressOM>>()
            {
                Data = new BillerComponent().GetAddresses(this.GetUser(), im)
            };
        }
        /// <summary>
        /// 获取缴费币种状态信息
        /// </summary>
        /// countryid 为当前国家的id
        /// <returns></returns>
        [HttpPost, Route("GetBillerCryptoCurrency")]
        public ServiceResult<BillerCryptoOM> GetBillerCryptoCurrency(BillerCryptoIM im)
        {
            return new ServiceResult<BillerCryptoOM>()
            {
                Data = new BillerCryptoOM()
                {
                    List = new BillerComponent().GetBillerCryptoCurrency(this.GetUser(), im.FiatCurrency).ToList(),
                    FaitCurrency = im.FiatCurrency
                } 
            };
        }
        
        /// <summary>
        /// 消息失败
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("MessageFail")]
        public ServiceResult<BillerMessageFailOM> MessageFail(Guid id)
        {
            return new ServiceResult<BillerMessageFailOM>()
            {
                Data = new BillerComponent().MessageFail(id)
            };
        }

    }
}