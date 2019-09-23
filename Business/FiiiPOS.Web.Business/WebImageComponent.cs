using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using log4net;
using Newtonsoft.Json;

namespace FiiiPOS.Web.Business
{
    public class WebImageComponent
    {
        private readonly string FUDATION_URL = ConfigurationManager.AppSettings["Foundation_URL"];
        private readonly string CLIENT_KEY = ConfigurationManager.AppSettings["Foundation_ClientKey"];
        private readonly string SECRET_KEY = ConfigurationManager.AppSettings["Foundation_SecretKey"];

        private readonly ILog _logger = LogManager.GetLogger("");
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public string Download(string id, int countryId)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.NO_IMAGE, "获取不到图片");
            }

            var bytes = new RegionImageAgent(countryId).Download(id);
            if (bytes == null)
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.NO_IMAGE, "获取不到图片");
            }
            string imageData = Convert.ToBase64String(bytes);
            if (string.IsNullOrEmpty(imageData))
                throw new CommonException(ReasonCode.FiiiPosReasonCode.NO_IMAGE, "获取不到图片");
            return imageData;
        }

        /// <summary>
        ///上传图片
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="base64Content"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public string Upload(string fileName, string base64Content, int countryId)
        {
            _logger.Error("--------------------------------------");
            try
            {
                var bytes = Convert.FromBase64String(base64Content);
                var ms = new MemoryStream(bytes);
                Image.FromStream(ms);
                return new RegionImageAgent(countryId).Upload(fileName ?? Guid.NewGuid().ToString(), bytes);
            }
            catch
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.IMAGE_FORMAT_ERROR, "图片格式不正确");
            }
        }

        public string UploadFundation(string fileName, string base64Content)
        {
            _logger.Error("--------------------------------------");
            try
            {
                var bytes = Convert.FromBase64String(base64Content);
                return new MasterImageAgent().Upload(fileName, bytes);
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
                throw new CommonException(ReasonCode.FiiiPosReasonCode.IMAGE_FORMAT_ERROR, "图片格式不正确");
            }
        }

        public string DownloadFundation(string id)
        {
            _logger.Error("--------------------------------------");
            try
            {
                return Convert.ToBase64String(new MasterImageAgent().Download(id));
            }
            catch
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.IMAGE_FORMAT_ERROR, "图片格式不正确");
            }
        }
    }
}
