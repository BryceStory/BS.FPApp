using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using FiiiPOS.API.Models;
using FiiiPOS.Business.FiiiPOS;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 提币地址
    /// </summary>
    [RoutePrefix("api/CryptoAddress")]
    public class CryptoAddressController : ApiController
    {
        /// <summary>
        /// 提币地址管理
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("Index")]
        public ServiceResult<List<CryptoAddressIndexES>> Index()
        {
            var result = new ServiceResult<List<CryptoAddressIndexES>>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var cpt = new CryptoAddressComponent();

            result.Data = cpt.GetMerchantCryptoAddress(this.GetMerchantAccountId());

            
            return result;
        }

        /// <summary>
        /// 单个币种列表
        /// </summary>
        /// <param name="cryptoId">加密币ID</param>
        /// <returns></returns>
        [HttpGet, Route("SingleList")]
        public ServiceResult<List<CryptoAddress>> SingleList(int cryptoId)
        {
            var result = new ServiceResult<List<CryptoAddress>>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var cpt = new CryptoAddressComponent();

            var list = cpt.GetMerchantCryptoAddress(this.GetMerchantAccountId(), cryptoId);

            result.Data = list;

            
            return result;
        }

        /// <summary>
        /// 添加地址 
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Add")]
        public ServiceResult Add(AddAddressModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var cpt = new CryptoAddressComponent();

            cpt.AddAddress(this.GetMerchantAccountId(), model.CryptoId, model.Address,model.Tag, model.Remark);

            

            return result;
        }

        ///// <summary>
        ///// 修改地址
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost, Route("Edit")]
        //public ServiceResult Edit(EditAddressModel model)
        //{
        //    var result = new ServiceResult();
        //    if (!ModelState.IsValid)
        //    {
        //        result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
        //        foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
        //            result.Message += error + Environment.NewLine;

        //        return result;
        //    }

        //    var cpt = new CryptoAddressComponent();

        //    cpt.EditAddress(this.GetMerchantAccountId(), model.Id, model.Address, model.Remark);

        //    

        //    return result;
        //}


        /// <summary>
        /// 删除地址
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Delete")]
        public ServiceResult Delete(DeleteAddressModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var cpt = new CryptoAddressComponent();

            cpt.DeleteAddress(this.GetMerchantAccountId(), model.Id);

            

            return result;
        }
    }
}
