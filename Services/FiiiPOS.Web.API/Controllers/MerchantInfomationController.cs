using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework;
using FiiiPOS.Web.API.Base;
using FiiiPOS.Web.API.Models.Models.Input;
using FiiiPOS.Web.API.Models.Models.Output;
using FiiiPOS.Web.Business;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FiiiPOS.Web.API.Controllers
{
    /// <summary>
    /// 门店管理
    /// </summary>
    [RoutePrefix("api/MerchantInfomation")]
    public class MerchantInfomationController : BaseApiController
    {
        /// <summary>
        /// 增加门店信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("MerchantInfomationSubmit")]
        public ServiceResult<int> MerchantInfomationSubmit(MerchantInfomationsModel model)
        {
            var _logger = LogManager.GetLogger(typeof(MerchantInfomationController));

            _logger.Info(Json(model));

            var result = new ServiceResult<int>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;
                return result;
            }

            var accountId = this.WorkContext.MerchantId; //商家ID
            var countryId = this.WorkContext.CountryId;  //国家ID

            int tempWeek = 0;
            model.Week.ForEach(item => tempWeek += (int)Math.Pow(2, item - 1));

            //门店管理信息
            var information = new MerchantInformation 
            {
                Id = Guid.NewGuid(),
                MerchantName = model.MerchantName,
                Week = (Week)tempWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Tags = string.Join(",", model.Tags),
                Introduce = model.Introduce,
                Address = model.Address,
                Lng = model.Lng,
                Lat = model.Lat,
                Phone = model.Phone,
                Status = Status.Enabled,
                VerifyStatus = VerifyStatus.UnderApproval,
                MerchantAccountId = accountId,
                IsPublic =Status.Enabled
            };

            //商家主图
            var ownersFigures = model.OwnersFigures.Select(item => new MerchantOwnersFigure()
            {
                FileId = item,
                MerchantInformationId = information.Id,
                Sort = model.OwnersFigures.IndexOf(item)//图片排序顺序
            }).ToList();

            //商家推荐图
            var recommends = model.Recommends.Select(item => new MerchantRecommend()
            {
                Id = Guid.NewGuid(),
                MerchantInformationId = information.Id,
                RecommendContent = item.Content,
                RecommendPicture = item.Picture,
                Sort = model.Recommends.IndexOf(item),//图片排序顺序
            }).ToList();

            //商家类别  
            var categorys = model.Categorys.Select(item => new MerchantCategory()
            {
                MerchantInformationId = information.Id,
                Category = item,
            }).ToList();

            new MerchantInformationComponent().InsertMerchantInformation(information, ownersFigures, recommends, categorys);
            result.Data = countryId;
            return result;
        }

        /// <summary>
        /// 查询门店信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetMerchantInformation")]
        public ServiceResult<MerchantInfomationsOutModel> GetMerchantInformation()
        {
            MerchantInfomationsOutModel infomationsOutModel = null;
            MerchantInformationES informationES = new MerchantInformationComponent().SelectMerchantInformation(WorkContext.MerchantId,WorkContext.CountryId);
            var weekArray = Enum.GetValues(typeof(Week));

            var weekList = new List<int>();

            foreach (var item in weekArray)
            {
                if (informationES.Week.HasFlag((Enum)item))
                {
                    weekList.Add((int)Math.Log((int)item, 2) + 1);
                }
            }
            infomationsOutModel = new MerchantInfomationsOutModel()
            {
                MerchantName = informationES.MerchantName,
                Categorys = informationES.Categorys,
                Week = weekList,
                StartTime = informationES.StartTime,
                EndTime = informationES.EndTime,
                Tags = informationES.Tags.Split(',').ToList(),
                Phone = informationES.Phone,
                Introduce = informationES.Introduce,
                Address = informationES.Address,
                Lng = informationES.Lng,
                Lat = informationES.Lat,
                OwnersFigures = informationES.OwnersFigures,
                VerifyStatus = informationES.VerifyStatus,
                Recommends = informationES.Recommends,
                Countrys =informationES.Countrys,
                IsPublic = informationES.IsPublic
            };
            return Result_OK(infomationsOutModel);
        }

        /// <summary>
        /// 修改门店信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("MerchantInfomationUpdate")]
        public ServiceResult<int> MerchantInfomationUpdate(MerchantInfomationsModel model)
        {
            var _logger = LogManager.GetLogger(typeof(MerchantInfomationController));

            _logger.Info(Json(model));

            var result = new ServiceResult<int>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;
                return result;
            }
            var accountId = this.WorkContext.MerchantId; //商家ID
            var countryId = this.WorkContext.CountryId;  //国家ID

            int tempWeek = 0;
            model.Week.ForEach(item => tempWeek += (int)Math.Pow(2, item - 1)); //营业周期转换
            //门店管理信息
            var information = new MerchantInformation
            {
                Id = new MerchantInformationComponent().GetInformationIdByAccount(accountId),
                MerchantName = model.MerchantName,
                Week = (Week)tempWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Tags = string.Join(",", model.Tags),
                Introduce = model.Introduce,
                Address = model.Address,
                Lng = model.Lng,
                Lat = model.Lat,
                Phone = model.Phone,
                VerifyStatus = VerifyStatus.UnderApproval,
                MerchantAccountId = accountId
            };

            //商家主图
            var ownersFigures = model.OwnersFigures.Select(item => new MerchantOwnersFigure()
            {
                FileId = item,
                MerchantInformationId = information.Id,
                Sort = model.OwnersFigures.IndexOf(item)//图片排序顺序
            }).ToList();

            //商家推荐图
            var recommends = model.Recommends.Select(item => new MerchantRecommend()
            {
                Id = Guid.NewGuid(),
                MerchantInformationId = information.Id,
                RecommendContent = item.Content,
                RecommendPicture = item.Picture,
                Sort = model.Recommends.IndexOf(item)//图片排序顺序
            }).ToList();

            //商家类别  
            var categorys = model.Categorys.Select(item => new MerchantCategory()
            {
                MerchantInformationId = information.Id,
                Category = item,
            }).ToList();

            new MerchantInformationComponent().UpdateMerchantInformation(information, ownersFigures, recommends, categorys, model.Phone);
            result.Data = countryId;
            return result;

        }

        /// <summary>
        /// 查询商家类别
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("StoreTypeQuerAll")]
        public ServiceResult<List<StoreTypeOutModel>> StoreTypeQuerAll()
        {
            return Result_OK(new MerchantInformationComponent().StoreTypeQuerAll().Select(item => new StoreTypeOutModel() { Id = item.Id, Name_CN = item.Name_CN, Name_EN = item.Name_EN }).ToList());
        }

        /// <summary>
        /// 查询商家门店状态是否停用
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetStatusId")]
        public ServiceResult<bool> GetStatusId()
        {
            var result = new ServiceResult<bool>();
            var accountId = this.WorkContext.MerchantId;
            return Result_OK(new MerchantInformationComponent().GetStatusId(accountId)?.IsPublic == Status.Enabled);
        }

        /// <summary>
        /// 修改商家门店按钮是否停用
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateStatus")]
        public ServiceResult<bool> UpdateStatus()
        {
            var accountId = this.WorkContext.MerchantId;
            new MerchantInformationComponent().UpdateStatus(accountId);
            return Result_OK<bool>(true);
        }

        /// <summary>
        /// 查询单商家门店审核失败原因
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("CauseFailure")]
        public ServiceResult CauseFailure()
        {
            var accountId = this.WorkContext.MerchantId;
            return Result_OK<string>(new MerchantInformationComponent().CauseFailure(accountId));
        }

    }
}