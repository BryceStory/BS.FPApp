using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using FiiiPay.Business.Properties;
using FiiiPay.Foundation.Business.Agent;

namespace FiiiPay.Business
{
    public class ImageComponent
    {
        /// <summary>
        /// 从Region的KYC服务下载图片
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="countryId">The country identifier.</param>
        /// <returns></returns>
        public byte[] DownloadWithRegion(string id, int countryId)
        {
            return new RegionImageAgent(countryId).Download(id);
        }

        /// <summary>
        /// 从美国主服务上下载图片
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public byte[] DownloadWithMaster(string id)
        {
            return new MasterImageAgent().Download(id);
        }

        public async Task<string> UploadToMasterAsync(string fileName, string base64Content)
        {
            var bytes = Convert.FromBase64String(base64Content);

            try
            {
                var ms = new MemoryStream(bytes);
                Image.FromStream(ms);
            }
            catch
            {
                throw new ApplicationException(MessageResources.InvalidFormat);
            }

            var imgIds = await new MasterImageAgent().UploadAsync(fileName ?? Guid.NewGuid().ToString(), bytes);
            if (imgIds == null)
            {
                throw new ApplicationException(MessageResources.UploadFailed);
            }

            return imgIds;
        }

        public async Task<Guid[]> UploadWithThumbnailToMasterAsync(string fileName, string base64Content)
        {
            var bytes = Convert.FromBase64String(base64Content);
            
            try
            {
                var ms = new MemoryStream(bytes);
                Image.FromStream(ms);
            }
            catch
            {
                throw new ApplicationException(MessageResources.InvalidFormat);
            }

            var imgIds = await new MasterImageAgent().UploadWithThumbnailAsync(fileName ?? Guid.NewGuid().ToString(), bytes);
            if (imgIds == null)
            {
                throw new ApplicationException(MessageResources.UploadFailed);
            }

            return imgIds;
        }

        /// <summary>
        /// 图片上传到Region的服务器
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="base64Content">Content of the base64.</param>
        /// <param name="countryId">The country identifier.</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public string UploadWithRegion(string fileName, string base64Content, int countryId)
        {
            var bytes = Convert.FromBase64String(base64Content);

            try
            {
                var ms = new MemoryStream(bytes);
                Image.FromStream(ms);
            }
            catch
            {
                throw new ApplicationException(MessageResources.InvalidFormat);
            }

            var imgId = new RegionImageAgent(countryId).Upload(fileName ?? Guid.NewGuid().ToString(), bytes);
            if (string.IsNullOrEmpty(imgId))
            {
                throw new ApplicationException(MessageResources.UploadFailed);
            }

            return imgId;
        }
    }
}
