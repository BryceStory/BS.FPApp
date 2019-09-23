using System;
using System.Web.Http;
using System.Web.Http.Results;
using FiiiPay.Foundation.API.Models;
using FiiiPay.Foundation.Business;
using log4net;

namespace FiiiPay.Foundation.API.Controllers
{
    [RoutePrefix("MessageAuth")]
    public class MessageAuthController : ApiController
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(MessageAuthController));

        [HttpPost, Route("Auth")]
        public IHttpActionResult Auth(AuthModel model)
        {
            if (!Guid.TryParse(model.ClientId, out var clientId))
            {
                _logger.Info($"Client[{model.ClientId}] Request Login failed, \"ClientId error\". {{ Username: {model.Username}, Password: {model.Password}}}");
                return BadResult();
            }

            var result = false;
            try
            {
                var component = new PushMessageComponent();
                result = component.Auth(clientId, model.Username, model.Password);
            }
            catch { }

            _logger.Info($"Client[{model.ClientId}] Request Login {(result ? "successed" : "failed")}. {{ Username: {model.Username}, Password: {model.Password}}}");

            return result ? OkResult() : BadResult();
        }

        [HttpPost, Route("SuperUser")]
        public IHttpActionResult SuperUser(SuperUserModel model)
        {
            _logger.Info($"Client[{model.ClientId}] Request SuperUser failed.{{ Username: {model.Username}}}");
            return BadResult();
        }

        [HttpPost, Route("Acl")]
        public IHttpActionResult AccessControlList(AccessControlListModel model)
        {
            if (model.Access != AccessModel.Subscribe)
            {
                _logger.Info($"Client[{model.ClientId}] {model.Access} Topic[{model.Topic}] failed, \"Subscribe only\".{{ Username: {model.Username}, Ip: {model.Ipaddr}}}");
                return BadResult();
            }

            if (!Guid.TryParse(model.ClientId, out var clientId))
            {
                _logger.Info($"Client[{model.ClientId}] {model.Access} Topic[{model.Topic}] failed, \"ClientId error\".{{ Username: {model.Username}, Ip: {model.Ipaddr}}}");
                return BadResult();
            }

            var topic = model.Topic;
            var arr = topic.Split('/');
            if (arr.Length != 3)
            {
                _logger.Info($"Client[{model.ClientId}] {model.Access} Topic[{model.Topic}] failed, \"Topic error\".{{ Username: {model.Username}, Ip: {model.Ipaddr}}}");
                return BadResult();
            }

            var result = false;
            try
            {
                var component = new PushMessageComponent();
                result = component.AccessControlList(clientId, arr[0], arr[2]);
            }
            catch { }

            _logger.Info($"Client[{model.ClientId}] {model.Access} Topic[{model.Topic}] {(result ? "successed" : "failed")}.{{ Username: {model.Username}, Ip: {model.Ipaddr}}}");
            return result ? OkResult() : BadResult();
        }

        private IHttpActionResult BadResult()
        {
            return new BadRequestResult(this);
        }

        private IHttpActionResult OkResult()
        {
            return new OkResult(this);
        }
    }
}