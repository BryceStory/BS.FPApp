using FiiiPay.Framework;
using FiiiPOS.Web.API.Base;
using FiiiPOS.Web.API.Models.Input;
using FiiiPOS.Web.Business;
using System;
using System.Web.Http;

namespace FiiiPOS.Web.API.Controllers
{
    /// <summary>
    /// 上传图片
    /// </summary>
    [RoutePrefix("api/Image")]
    public class ImageController : BaseApiController
    {
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="id">图片guid</param>
        /// <returns></returns>
        [Route("Download"), HttpGet]
        public ServiceResult<string> Download(string id)
        {
            ServiceResult<string> result = new ServiceResult<string>();
            int countryId = WorkContext.CountryId;

            var bytes = new WebImageComponent().Download(id, countryId);

            result.Data = bytes;
            return result;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Upload")]
        public ServiceResult<string> Upload(UploadImageInModel model)
        {
            int countryId = WorkContext.CountryId;

            string imgId = new WebImageComponent().Upload(model.FileName, model.Base64Content, countryId);
            return Result_OK(imgId);

        }


        /// <summary>
        /// 上传Fundation图片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UploadFundation")]
        public ServiceResult<string> UploadFundation(UploadImageInModel model)
        {
            int countryId = WorkContext.CountryId;

            string imgId = new WebImageComponent().UploadFundation(model.FileName, model.Base64Content);
            return Result_OK(imgId);
        }

        /// <summary>
        /// 下载Fundation图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("DownloadFundation"), HttpGet]
        public ServiceResult<string> DownloadFundation(string id)
        {
            int countryId = WorkContext.CountryId;

            string imgId = new WebImageComponent().DownloadFundation(id);
            return Result_OK(imgId);
        }
    }
}
