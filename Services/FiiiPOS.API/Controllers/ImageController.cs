using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using FiiiPOS.API.Models;
using FiiiPOS.Business;
using FiiiPOS.Business.FiiiPOS;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 图片相关
    /// </summary>
    [RoutePrefix("Image")]
    public class ImageController : ApiController
    {
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="id">图片guid</param>
        /// <returns></returns>
        [Route("Download"), HttpGet]
        public HttpResponseMessage Download(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id == "null") return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            var account = new MerchantAccountComponent().GetById(this.GetMerchantAccountId());
            var bytes = new ImageComponent().DownloadWithRegion(id, account.CountryId);

            if (bytes == null || bytes.Length == 0) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return resp;
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="id">图片guid</param>
        /// <returns></returns>
        [Route("DownloadImage"), HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DownloadImage(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id == "null") return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var bytes = new ImageComponent().DownloadWithMaster(id);
            if (bytes == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return resp;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Upload")]
        public ServiceResult<string> Upload(UploadImageModel model)
        {
            var account = new MerchantAccountComponent().GetById(this.GetMerchantAccountId());
            return new ServiceResult<string>
            {
                Data = new ImageComponent().UploadWithRegion(model.FileName, model.Base64Content, account.CountryId)
            };
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UploadFile")]
        public ServiceResult<string> UploadFile(UploadFileWithByteArray model)
        {
            return new ServiceResult<string>
            {
                Data = new ImageComponent().UploadWithRegion(model.FileName, model.Stream)
            };

        }
    }
}
