using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using FiiiPOS.DTO.Messages;
using System.Web.Http;
using System;
using FiiiPOS.Business;
using log4net;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 消息
    /// </summary>
    [RoutePrefix("Messages")]
    public class MessagesController : ApiController
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(MessagesController));

        /// <summary>
        /// 读取消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("ReadMessage")]
        public ServiceResult<bool> ReadMessage(ReadMessageIM model)
        {
            bool result = new MessageComponent().ReadMessage(model.Id, this.GetMerchantAccountId());

            return new ServiceResult<bool>
            {
                Data = result
            };
        }

        /// <summary>
        /// 一键已读
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("OnekeyReadMessage")]
        public ServiceResult<bool> OnekeyReadMessage()
        {
            bool result = false;
            try
            {
                new ArticleComponent().OnekeyRead(this.GetMerchantAccountId());
                result = new MessageComponent().ReadMessage(this.GetMerchantAccountId());
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("OnekeyReadMessage:{0}", ex));
            }
            return new ServiceResult<bool>
            {
                Data = result
            };
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("DeleteMessage")]
        public ServiceResult<bool> DeleteMessage(DeleteMessageIM model)
        {
            bool result = new MessageComponent().DeleteMessage(model.Id, this.GetMerchantAccountId());

            return new ServiceResult<bool>
            {
                Data = result
            };
        }

        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMessageList")]
        public ServiceResult<MessageListOM> GetMessageList(MessageListIM model)
        {
            var isZH = this.IsZH();

            return new ServiceResult<MessageListOM>
            {
                Data = new MessageListOM
                {
                    List = new MessageComponent().GetListByPage(this.GetMerchantAccountId(), model.PageIndex, model.PageSize, isZH)
                }
            };
        }


        /// <summary>
        /// 获取未读消息总数
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("TotalUnreadMessage")]
        public ServiceResult<long> GetTotalUnreadMessage()
        {

            ServiceResult<long> result = new ServiceResult<long>
            {
                Data = new MessageComponent().GetTotalUnreadMessage(this.GetMerchantAccountId())
            };

            return result;
        }
    }
}
