using FiiiPay.DTO.Messages;
using FiiiPay.Framework;
using System;
using System.Collections.Generic;
using System.Web.Http;
using FiiiPay.Framework.MongoDB;
using FiiiPay.Business;
using log4net;

namespace FiiiPay.API.Controllers
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
            bool result = false;
            try
            {
                result = MessagesComponent.ReadMessage(model.Id, this.GetUser().Id);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("ReadMessage:{0}", ex));
            }

            return new ServiceResult<bool>
            {
                Data = result
            };
        }

        /// <summary>
        /// Called when [read message].
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("OnekeyReadMessage")]
        public ServiceResult<bool> OnekeyReadMessage()
        {
            bool result = false;
            try
            {
                new ArticleComponent().OnekeyRead(this.GetUser().Id);
                result = MessagesComponent.OnekeyReadMessage(this.GetUser().Id);
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
            bool result = false;
            try
            {
                result = MessagesComponent.DeleteMessage(model.Id, this.GetUser().Id);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("DeleteMessage:{0}", ex));
            }

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
            model.MaxType = 27;//旧版本可显示27或之前的类型
            if (model.PageSize > 20)
                model.PageSize = 20;
            var outPutList = new List<MessageListOMItem>();
            try
            {
                var isZH = this.IsZH();

                var list = MessagesComponent.GetMessagesByPage(this.GetUser().Id, UserType.User, model.PageIndex, model.PageSize);

                if (list != null && list.Count > 0)
                {
                    list.RemoveAll(t => t.MsgType > model.MaxType);
                }

                foreach (var entity in list)
                {
                    var lang = isZH ? "zh" : "en";
                    if (entity.Title == "奖励FIII")
                    {
                        entity.Title = "BonusFIII";
                    }

                    if (entity.SubTitleKey == "奖励子标题")
                    {
                        entity.SubTitleKey = "BonusSubTitle";
                    }
                    string content = ResourceHelper.FiiiPay.GetFormatResource(entity.TitleKey, lang, entity.CoinCode);
                    string subTitle = ResourceHelper.FiiiPay.GetFormatResource(entity.SubTitleKey, lang, entity.CoinCode);

                    var outPutModel = new MessageListOMItem
                    {
                        NoticeId = entity._id.ToString(),
                        Type = entity.MsgType,
                        QueryId = entity.QueryId,
                        Title = content,
                        SubTitle = subTitle,
                        Timestamp = entity.CreateTime.ToUnixTime(),
                        Status = entity.Status
                    };
                    outPutList.Add(outPutModel);
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetMessageList:{0}", ex));
            }

            return new ServiceResult<MessageListOM>
            {
                Data = new MessageListOM
                {
                    List = outPutList
                }
            };
        }

        /// <summary>
        /// 新方式获取消息列表（包括网关支付消息）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("GetNewMessageList")]
        public ServiceResult<MessageListOM> GetNewMessageList(MessageListIM model)
        {
            if (!model.MaxType.HasValue)
                model.MaxType = 27;
            if (model.PageSize > 20)
                model.PageSize = 20;
            var outPutList = new List<MessageListOMItem>();
            try
            {
                var isZH = this.IsZH();

                var list = MessagesComponent.GetMessagesByPage(this.GetUser().Id, UserType.User, model.PageIndex, model.PageSize);
                if (list != null && list.Count > 0)
                {
                    list.RemoveAll(t => t.MsgType > model.MaxType.Value);
                }

                foreach (var entity in list)
                {
                    var lang = isZH ? "zh" : "en";
                    string content = ResourceHelper.FiiiPay.GetFormatResource(entity.TitleKey, lang, entity.CoinCode);
                    string subTitle = ResourceHelper.FiiiPay.GetFormatResource(entity.SubTitleKey, lang, entity.CoinCode);

                    var outPutModel = new MessageListOMItem
                    {
                        NoticeId = entity._id.ToString(),
                        Type = entity.MsgType,
                        QueryId = entity.QueryId,
                        Title = content,
                        SubTitle = subTitle,
                        Timestamp = entity.CreateTime.ToUnixTime(),
                        Status = entity.Status
                    };
                   
                    outPutList.Add(outPutModel);
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetMessageList:{0}", ex));
            }

            return new ServiceResult<MessageListOM>
            {
                Data = new MessageListOM
                {
                    List = outPutList
                }
            };
        }
    }
}
