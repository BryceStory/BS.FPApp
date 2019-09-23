using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Profile.API.Common;
using FiiiPay.Profile.API.Models;
using FiiiPay.Profile.Entities;
using log4net;

namespace FiiiPay.Profile.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.Profile.API.Controllers.FileController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("File")]
    [Authorize]
    public class FileController : ApiController
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(FileController));

        private const int MISSING_REQUIRED_FIELDS = 10000;

        /// <summary>
        /// Uploads the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload")]
        public ServiceResult<Guid> Upload(FileUploadModel model)
        {
            var resultData = new ServiceResult<Guid>();

            try
            {
                if (!ModelState.IsValid)
                {
                    resultData.Code = MISSING_REQUIRED_FIELDS;
                    foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                        resultData.Message += error + Environment.NewLine;

                    return resultData;
                }

                var file = new File
                {
                    FileName = model.FileName,
                    FileType = model.FileType,
                    Timestamp = DateTime.Now
                };

                new FileComponent().Create(file, model.File);

                resultData.Data = file.Id;
                resultData.Success();
            }
            catch (CommonException exception)
            {
                resultData.SystemError(exception);
                _log.Error(exception);
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex); _log.Error(ex);
            }

            return resultData;
        }

        /// <summary>
        /// Files the has exist.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("FileHasExist")]
        public ServiceResult<Guid> FileHasExist(FileHasExistModel model)
        {
            var resultData = new ServiceResult<Guid>();

            try
            {
                if (!ModelState.IsValid)
                {
                    resultData.Code = MISSING_REQUIRED_FIELDS;
                    foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                        resultData.Message += error + Environment.NewLine;

                    return resultData;
                }

                var file = new FileComponent().GetByMd5(model.Md5);
                if (file != null)
                {
                    resultData.Data = file.Id;
                }

                resultData.Success();
            }
            catch (CommonException exception)
            {
                resultData.SystemError(exception);
                _log.Error(exception);
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex); _log.Error(ex);
            }

            return resultData;
        }

        /// <summary>
        /// Downloads the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Download")]
        public ServiceResult<byte[]> Download(Guid id)
        {
            var resultData = new ServiceResult<byte[]>();

            try
            {
                if (!ModelState.IsValid)
                {
                    resultData.Code = MISSING_REQUIRED_FIELDS;
                    foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                        resultData.Message += error + Environment.NewLine;

                    return resultData;
                }

                var file = new FileComponent().GetById(id);

                if (file != null)
                {
                    var storageFolder = ConfigurationManager.AppSettings.Get("StorageFolder");
                    var fullPath = System.IO.Path.Combine(storageFolder, file.FilePath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        byte[] bytes;
                        using (var stream = System.IO.File.Open(fullPath, System.IO.FileMode.Open))
                        {
                            bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, bytes.Length);
                            stream.Close();
                        }

                        resultData.Data = bytes;

                        resultData.Success();
                    }
                    else
                    {
                        resultData.Failer(10001, "File is not found");
                    }
                }
                else
                {
                    resultData.Failer(10001, "File is not found");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                resultData.SystemError(ex);
            }

            return resultData;
        }
    }
}
