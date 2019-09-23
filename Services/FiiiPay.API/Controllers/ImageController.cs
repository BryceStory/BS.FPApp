using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using FiiiPay.Business;
using FiiiPay.DTO.Image;
using FiiiPay.Framework;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.ImageController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Image")]
    public class ImageController : ApiController
    {
        /// <summary>
        /// 下载用户相关的图片，比如用户头像，护照照片
        /// </summary>
        /// <param name="id">图片guid</param>
        /// <returns></returns>
        [Route("Download"), HttpGet]
        public HttpResponseMessage Download(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id == Guid.Empty.ToString()) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            
            var bytes = new ImageComponent().DownloadWithRegion(id, this.GetUser().CountryId);
            if(bytes == null || bytes.Length == 0) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return resp;
        }

        ///// <summary>
        ///// Downloads the image.
        ///// </summary>
        ///// <param name="id">The identifier.</param>
        ///// <returns></returns>
        //[Route("DownloadImage"), HttpGet]
        //[AllowAnonymous]
        //public HttpResponseMessage DownloadImage(Guid id)
        //{
        //    if (id == Guid.Empty) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        //    //var bytes = new FileComponent().GetFileById(id);
        //    var bytes = new MasterImageAgent().Download(id.ToString());
        //    if (bytes == null || bytes.Length == 0) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

        //    var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        //    {
        //        Content = new ByteArrayContent(bytes)
        //    };
        //    resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        //    return resp;
        //}

        ///// <summary>
        ///// Downloads the crypto image.
        ///// </summary>
        ///// <param name="code">The code.</param>
        ///// <returns></returns>
        //[Route("DownloadCryptoImage"), HttpGet]
        //[AllowAnonymous]
        //public HttpResponseMessage DownloadCryptoImage(string code)
        //{
        //    if (string.IsNullOrWhiteSpace(code)) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

        //    var dac = new Foundation.Business.CryptoComponent().GetByCode(code);
        //    if(dac == null) return new HttpResponseMessage(System.Net.HttpStatusCode.NotImplemented);

        //    var bytes = new MasterImageAgent().Download(dac.IconURL?.ToString());
        //    if (bytes == null || bytes.Length == 0) return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);

        //    var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        //    {
        //        Content = new ByteArrayContent(bytes)
        //    };
        //    resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        //    return resp;
        //}

        /// <summary>
        /// 上传图片到kyc服务器
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Upload")]
        public ServiceResult<string> Upload(UploadImageIM model)
        {
            return new ServiceResult<string>
            {
                Data = new ImageComponent().UploadWithRegion(model.FileName, model.Base64Content, this.GetUser().CountryId)
            };
        }

        /// <summary>
        /// 上传图片到图片服务器, 不会生成图片的缩略图
        /// </summary>
        /// <param name="model"></param>
        /// <returns>图片ID</returns>
        [HttpPost, Route("UploadToMaster")]
        public async Task<ServiceResult<string>> UploadToMaster(UploadImageIM model)
        {
            return new ServiceResult<string>
            {
                Data = await new ImageComponent().UploadToMasterAsync(model.FileName, model.Base64Content)
            };
        }

        /// <summary>
        /// 上传图片到图片服务器
        /// 会自动生成图片的缩略图
        /// </summary>
        /// <param name="model"></param>
        /// <returns>图片ID和缩略图ID</returns>
        [HttpPost, Route("UploadWithThumbnailToMaster")]
        public async Task<ServiceResult<Guid[]>> UploadWithThumbnailToMaster(UploadImageIM model)
        {
            return new ServiceResult<Guid[]>
            {
                Data = await new ImageComponent().UploadWithThumbnailToMasterAsync(model.FileName, model.Base64Content)
            };
        }
    }
}
