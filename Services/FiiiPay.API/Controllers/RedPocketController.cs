using System;
using System.Collections.Generic;
using FiiiPay.API.Models;
using FiiiPay.Business.FiiiPay;
using FiiiPay.DTO;
using FiiiPay.Framework;
using System.Web.Http;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.RedPocketController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("RedPocket")]
    public class RedPocketController : ApiController
    {
        private static readonly object _lock = new object();
        private static readonly object _pocketLock = new object();

        private static RedPocketComponent _component;

        public static RedPocketComponent Component
        {
            get
            {
                lock (_lock)
                {
                    if (_component == null)
                        _component = new RedPocketComponent();

                    return _component;
                }
            }
        }

        /// <summary>
        /// 发送红包
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Push")]
        //[AllowAnonymous]
        public ServiceResult<RedPocketPushOM> Push(PushRedPocketModel model)
        {
            var user = this.GetUser();
            //var user = new UserAccountComponent().GetById(Guid.Parse("D59228E9-52F8-452A-9AFA-AC649C618C02"));
            var result = new RedPocketComponent().Push(user, user.Id, model.CryptoId, model.Amount, model.Count, user.Pin, model.PIN, model.Message);
            return new ServiceResult<RedPocketPushOM>
            {
                Data = new RedPocketPushOM { ExpirationDate = result.ExpirationDate.ToUtcTimeTicks().ToString(), PassCode = result.PassCode.ToUpper() }
            };
        }

        /// <summary>
        /// 领取红包
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Receive")]
        //[AllowAnonymous]
        public ServiceResult<RedPocketDetailOM> Receive(ReceiveModel model)
        {
            lock (_pocketLock)
            {
                //var data = Component.Receive(new UserAccountComponent().GetById(Guid.Parse("D59228E9-52F8-452A-9AFA-AC649C618C02")), model.PassCode);
                var data = Component.Receive(this.GetUser(), model.PassCode, this.IsZH());
                //Guid.Parse("D59228E9-52F8-452A-9AFA-AC649C618C02")
                return new ServiceResult<RedPocketDetailOM>
                {
                    Data = data
                };
            }
        }

        /// <summary>
        /// 红包详情
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Detail")]
        public ServiceResult<RedPocketDetailOM> Detail(GetDetailByIdIM<long> model)
        {
            return new ServiceResult<RedPocketDetailOM>
            {
                Data = new RedPocketComponent().Detail(this.GetUser().Id, model.Id)
            };
        }

        /// <summary>
        /// 重新发送
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("RePush")]
        public ServiceResult<RedPocketPushOM> RePush(GetDetailByIdIM<long> model)
        {
            var reslut = new RedPocketComponent().RePush(this.GetUser().Id, model.Id);

            return new ServiceResult<RedPocketPushOM>
            {
                Data = new RedPocketPushOM
                { ExpirationDate = reslut.ExpirationDate.ToUtcTimeTicks().ToString(), PassCode = reslut.PassCode.ToUpper() }
            };
        }

        /// <summary>
        /// 红包详情列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("DetailList")]
        public ServiceResult<List<RedPocketListDetailOM>> DetailList(RedPocketDetailListModel model)
        {
            return new ServiceResult<List<RedPocketListDetailOM>>
            {
                Data = new RedPocketComponent().DetailList(model.PocketId, model.PageIndex, model.PageSize)
            };
        }

        /// <summary>
        /// 帐单详情
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("StatementDetail")]
        public ServiceResult<StatementDetailOM> StatementDetail(StatementDetailDataModel model)
        {
            return new ServiceResult<StatementDetailOM>
            {
                Data = new RedPocketComponent().StatementDetail(this.GetUser().Id, model.Type, model.Id)
            };
        }

        /// <summary>
        /// 获取币种对应美元价格及最大红包数
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetUSDPrice")]
        public ServiceResult<USDPriceModel> GetUSDPrice(GetUSDPriceModel model)
        {
            return new ServiceResult<USDPriceModel>
            {
                Data = new USDPriceModel
                {
                    MaxCount = RedPocketComponent.MaxCount,
                    MaxAmount = Convert.ToInt32(RedPocketComponent.MaxAmount),
                    USDPrice = new Foundation.Business.PriceInfoComponent().GetPrice("USD", model.Currency).ToString()
                }
            };
        }

        /// <summary>
        /// 已发送红包列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("PushList")]
        public ServiceResult<PushRedPocketListOM> PushList(RedPocketPageModel model)
        {
            var user = this.GetUser();
            return new ServiceResult<PushRedPocketListOM>
            {
                Data = new RedPocketComponent().PushList(user.Id,user.FiatCurrency, model.PageIndex, model.PageSize)
            };
        }

        /// <summary>
        /// 领取红包列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("ReceiveList")]
        //[AllowAnonymous]
        public ServiceResult<RedPocketReceiveListOM> ReceiveList(RedPocketPageModel model)
        {
            var user = this.GetUser();
            //var user = new UserAccountComponent().GetById(Guid.Parse("D59228E9-52F8-452A-9AFA-AC649C618C02"));
            return new ServiceResult<RedPocketReceiveListOM>
            {
                Data = new RedPocketComponent().ReceiveList(user.Id, user.FiatCurrency, model.PageIndex, model.PageSize)
            };
        }
    }
}
