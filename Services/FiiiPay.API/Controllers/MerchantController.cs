using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using FiiiPay.API.Models;
using FiiiPay.Business;
using FiiiPay.Business.FiiiPay;
using FiiiPay.Business.Properties;
using FiiiPay.DTO;
using FiiiPay.DTO.Merchant;
using FiiiPay.Framework;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.MerchantController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Merchant")]
    public class MerchantController : ApiController
    {
        /// <summary>
        /// 蓝牙扫描附近的FiiiPos商家列表
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("GetListByCodeList")]
        public ServiceResult<GetListByCodeListOM> GetListByCodeList(GetListByCodeListIM im)
        {
            if (im.CodeList.Count == 0)
            {
                return new ServiceResult<GetListByCodeListOM>
                {
                    Data = new GetListByCodeListOM
                    {
                        List = new List<GetListByCodeListOMItem>()
                    }
                };
            }
            return new ServiceResult<GetListByCodeListOM>
            {
                Data = new UserAccountComponent().GetListByCodeList(string.Join(",", im.CodeList))
            };
        }

        /// <summary>
        /// 地图查找附近门店
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMerchantInfoList"), AllowAnonymous]
        public ServiceResult<List<GetMerchantInfoOM>> GetMerchantInfoList(GetMerchantInfoListIM im)
        {
            return new ServiceResult<List<GetMerchantInfoOM>>
            {
                Data = new MerchantComponent().GetMerchantInfoList(im)
            };
        }
        /// <summary>
        /// 地图查找附近门店
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMerchantInfoListByDistance"), AllowAnonymous]
        public ServiceResult<List<GetMerchantInfoOM>> GetMerchantInfoListByDistance(GetMerchantInfoListByDistanceIM im)
        {
            return new ServiceResult<List<GetMerchantInfoOM>>
            {
                Data = new MerchantComponent().GetMerchantInfoList(im)
            };
        }
        /// <summary>
        /// 地图查找附近商家
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMerchantInfoListByMap"), AllowAnonymous]
        public ServiceResult<List<GetMerchantInfoOM>> GetMerchantInfoListByMap(GetMerchantInfoListByMapIM im)
        {
            return new ServiceResult<List<GetMerchantInfoOM>>
            {
                Data = new MerchantComponent().GetMerchantInfoList(im)
            };
        }
        /// <summary>
        /// 获取商家详情
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMerchantDetail"), AllowAnonymous]
        public ServiceResult<GetMerchantDetailOM> GetMerchantDetail(GetMerchantDetailIM im)
        {
            return new ServiceResult<GetMerchantDetailOM>
            {
                Data = new MerchantComponent().GetMerchantDetail(im)
            };
        }
        /// <summary>
        /// 获取商家类别列表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetStoreTypeList"), AllowAnonymous]
        public ServiceResult<List<MerchantStoreTypeOM>> GetStoreTypeList()
        {
            return new ServiceResult<List<MerchantStoreTypeOM>>()
            {
                Data = new MerchantComponent().GetStoreTypeList()
            };
        }

        /// <summary>
        /// 获取用户开设门店的邀请码
        /// </summary>
        /// <returns>返回邀请码，如果用户未被邀请开设门店，则返回空</returns>
        [HttpPost, Route("GetMerchantInviteCode")]
        public async Task<ServiceResult<string>> GetMerchantInviteCode()
        {
            return new ServiceResult<string>()
            {
                Data = await new MerchantComponent().GetMerchantInviteCode(this.GetUser().Id)
            };
        }

        /// <summary>
        /// FiiiPay门店入驻
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost,Route("FiiipayMerchantCreate")]
        public async Task<ServiceResult<bool>> FiiipayMerchantCreate(MerchantInfoCreateModel model)
        {
            var result = new ServiceResult<bool>();
            if (model.MerchantCategorys.Length > 3 || model.TagList.Length > 7)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                result.Message = MessageResources.InvalidFormat;
                result.Data = false;
                return result;
            }

            result.Data = await new MerchantComponent().FiiipayMerchantCreateAsync(this.GetUser(), new FiiiPayMerchantInfoCreateIM
            {
                InviteCode = model.InviteCode,
                MerchantName = model.MerchantName,
                WeekTxt = model.WeekTxt,
                TagList = model.TagList,
                MerchantCategorys = model.MerchantCategorys,
                SupportCoins = model.SupportCoins,
                Introduce = model.Introduce,
                CountryId = model.CountryId,
                StateId = model.StateId,
                CityId = model.CityId,
                Address = model.Address,
                Lng = model.Lng,
                Lat = model.Lat,
                ApplicantName = model.ApplicantName,
                Phone = model.Phone,
                StorefrontImg = model.StorefrontImg,
                FigureImgList = model.FigureImgList,
                LicenseNo = model.LicenseNo,
                BusinessLicenseImage = model.BusinessLicenseImage,
                UseFiiiDeduction = model.UseFiiiDeduction
            });

            return result;
        }
    }
}
