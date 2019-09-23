using FiiiPay.Business;
using FiiiPay.DTO.Invite;
using FiiiPay.Framework;
using System.Collections.Generic;
using System.Web.Http;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 邀请好友注册fiiipay或者购买fiiipos
    /// </summary>
    [RoutePrefix("Invite")]
    public class InviteController : ApiController
    {
        /// <summary>
        /// 获取邀请fiiipay用户的粗略详情
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("FiiiPayPreProfit")]
        public ServiceResult<PreFiiiPayProfitOM> FiiiPayPreProfit()
        {
            return new ServiceResult<PreFiiiPayProfitOM>()
            {
                Data = new InviteComponent().PreFiiiPayProfit(this.GetUser())
            };
        }

        /// <summary>
        /// 获取邀请fiiipos用户的粗略详情
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("FiiiPosPreProfit")]
        public ServiceResult<PreFiiiPosProfitOM> FiiiPosPreProfit()
        {
            return new ServiceResult<PreFiiiPosProfitOM>()
            {
                Data = new InviteComponent().PreFiiiPosProfit(this.GetUser())
            };
        }
        /// <summary>
        /// 获取榜单详情
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("PreRankingDetail")]
        public ServiceResult<List<InviteRankOM>> PreRankingDetail(InviteRankIM im)
        {
            return new ServiceResult<List<InviteRankOM>>()
            {
                Data = new InviteComponent().GetInviteRankList(im)
            };
        }
        /// <summary>
        /// 邀请fiiipay用户的收益记录
        /// </summary>
        /// <param name="pageIndex">当前页码 1(首页)：20 其余增加10</param>
        /// <returns></returns>
        [HttpPost, Route("FiiiPayProfitRecord")]
        public ServiceResult<List<ProfitDetailOM>> FiiiPayProfitRecord(int pageIndex)
        {
            return new ServiceResult<List<ProfitDetailOM>>()
            {
                Data = new InviteComponent().FiiipayDetail(this.GetUser(), pageIndex)
            };
        }
        /// <summary>
        /// 邀请fiiipos用户的收益粗略记录
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("FiiiPosProfitRecord")]
        public ServiceResult<List<FiiiposBonusRecordOM>> FiiiPosProfitRecord()
        {
            return new ServiceResult<List<FiiiposBonusRecordOM>>()
            {
                Data = new InviteComponent().FiiiposDetail(this.GetUser())
            };
        }
        /// <summary>
        /// 邀请fiiipos用户的收益详细记录
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("FiiiPosProfitDetailRecord")]
        public ServiceResult<FiiiposProfitDetailRecordOM> FiiiPosProfitDetailRecord(FiiiposBonusRecordIM im)
        {
            return new ServiceResult<FiiiposProfitDetailRecordOM>()
            {
                Data = new InviteComponent().FiiiposMoreDetail(im, this.GetUser())
            };
        }

        /// <summary>
        /// 获取当前排名
        /// 如果还没有收益时 不让用户展示排名 会返回-1作为一个标志
        /// </summary>
        /// <param name="im">只支持fiiipay和fiiipos的奖励排行</param>
        /// <returns></returns>
        // TODO 针对平台进行排名
        [HttpPost, Route("CurrentRank")]
        public ServiceResult<int> CurrentRank(SystemPlatform im)
        {
            return new ServiceResult<int>()
            {
                Data = new InviteComponent().GetCurrentRank(im, this.GetUser())
            };
        }
        /// <summary>
        /// 单个奖励详情
        /// </summary>
        /// <param name="im">收益详情订单主键</param>
        /// <returns></returns>
        [HttpPost, Route("SingleBonusDetail")]
        public ServiceResult<SingleBonusDetailOM> SingleBonusDetail(SingleBonusDetailIM im)
        {
            return new ServiceResult<SingleBonusDetailOM>()
            {
                Data = new InviteComponent().GetSingleBonusDetail(im.Id)
            };
        }
    }
}