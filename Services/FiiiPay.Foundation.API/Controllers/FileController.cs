using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using FiiiPay.Foundation.API.Models;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using log4net;

namespace FiiiPay.Foundation.API.Controllers
{
    [RoutePrefix("File")]
    [Authorize]
    public class FileController : ApiController
    {
        public const int MISSING_REQUIRED_FIELDS = 10001;
        public const int RECORD_NOT_EXIST = 10002;

        private readonly ILog _log = LogManager.GetLogger(typeof(FileController));

        /// <summary>
        /// 生成已上传图片的缩略图
        /// </summary>
        /// <param name="fileId">图片ID</param>
        /// <returns></returns>
        [HttpGet,Route("UploadWithCompress")]
        public ServiceResult<Guid> UploadWithCompress(Guid fileId)
        {
            var resultData = new ServiceResult<Guid>();

            try
            {
                if (!ModelState.IsValid)
                {
                    resultData.Code = MISSING_REQUIRED_FIELDS;
                    foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                        resultData.Message += error + Environment.NewLine;

                    return new ServiceResult<Guid>() { Data = Guid.Empty};
                }

                resultData.Data =  new FileComponent().CreateWithCompress(fileId);
                
                resultData.Success();
            }
            catch (CommonException exception)
            {
                resultData.SystemError(exception);
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex);
            }

            return resultData;
        }

        /// <summary>
        /// 上传图片，同时生成缩略图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UploadWithThumbnail")]
        public ServiceResult<Guid[]> UploadWithThumbnail(FileUploadWithThumbnailModel model)
        {
            var resultData = new ServiceResult<Guid[]>();

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
                
                resultData.Data = new FileComponent().CreateWithThumbnail(file,model.File);
                resultData.Success();
            }
            catch (CommonException exception)
            {
                resultData.SystemError(exception);
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex);
            }

            return resultData;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="model"></param>
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
                _log.Error(exception);
                resultData.SystemError(exception);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                resultData.SystemError(ex);
            }

            return resultData;
        }

        [HttpPost]
        [Route("Delete")]
        public ServiceResult Delete(FileDeleteModel model)
        {
            var resultData = new ServiceResult();
            try
            {
                if (!ModelState.IsValid)
                {
                    resultData.Code = MISSING_REQUIRED_FIELDS;
                    foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                        resultData.Message += error + Environment.NewLine;

                    return resultData;
                }

                new FileComponent().Delete(model.Id);

                resultData.Success();
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex);
            }

            return resultData;
        }

        [HttpPost]
        [Route("GetFileByMd5")]
        public ServiceResult<File> GetFileByMd5(GetFileByMd5Model model)
        {
            var resultData = new ServiceResult<File>();
            try
            {
                if (!ModelState.IsValid)
                {
                    resultData.Code = MISSING_REQUIRED_FIELDS;
                    foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                        resultData.Message += error + Environment.NewLine;

                    return resultData;
                }

                new FileComponent().GetByMd5(model.Md5);

                resultData.Success();
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex);
            }

            return resultData;
        }

        [HttpGet]
        [Route("GetFileName")]
        public ServiceResult<string> GetFileName(Guid id)
        {
            var resultData = new ServiceResult<string>();
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
                    resultData.Data = file.FileName;
                    resultData.Success();
                }
                else
                {
                    resultData.Failer(RECORD_NOT_EXIST, "File is not found");
                }
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex);
            }

            return resultData;
        }

        [HttpGet]
        [Route("CheckFileExists")]
        public ServiceResult<bool> CheckFileExists(Guid id)
        {
            var resultData = new ServiceResult<bool>();

            try
            {
                if (!ModelState.IsValid)
                {
                    resultData.Code = MISSING_REQUIRED_FIELDS;
                    foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                        resultData.Message += error + Environment.NewLine;

                    return resultData;
                }

                var exists = new FileComponent().CheckFileExists(id);

                resultData.Data = exists;

                resultData.Success();
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex);
            }

            return resultData;
        }

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

                var file = new FileComponent().GetFileById(id);
                if (file != null)
                {
                    resultData.Data = file;
                    resultData.Success();
                }
                else
                {
                    resultData.Failer(RECORD_NOT_EXIST, "File is not found");
                }
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex);
                resultData.Message = "Inner error";
            }

            return resultData;
        }

        [Route("DownloadImage"), HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DownloadImage(Guid id)
        {
            if (id == Guid.Empty) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var bytes = new FileComponent().GetFileById(id);
            if (bytes == null || bytes.Length == 0) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return resp;
        }

        [Route("DownloadCryptoImage"), HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DownloadCryptoImage(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            var crypto = new CryptoComponent().GetByCode(code);
            if (crypto == null) return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);

            var bytes = new FileComponent().GetFileById(crypto.IconURL ?? Guid.Empty);

            if (bytes == null || bytes.Length == 0) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return resp;
        }

        [HttpPost]
        [Route("Copy")]
        public ServiceResult<File> Copy(FileCopyModel model)
        {
            var resultData = new ServiceResult<File>();

            try
            {
                if (!ModelState.IsValid)
                {
                    resultData.Code = MISSING_REQUIRED_FIELDS;
                    foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                        resultData.Message += error + Environment.NewLine;

                    return resultData;
                }

                var file = new FileComponent().CopyInFolder(model.Id);

                resultData.Data = file;
                resultData.Success();
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex);
            }
            return resultData;
        }
        
        [HttpPost]
        [Route("Replace")]
        public ServiceResult Replace(FileReplaceModel model)
        {
            var resultData = new ServiceResult();
            try
            {
                if (!ModelState.IsValid)
                {
                    resultData.Code = MISSING_REQUIRED_FIELDS;
                    foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                        resultData.Message += error + Environment.NewLine;

                    return resultData;
                }

                new FileComponent().ReplaceById(model.SourceId, model.TargetId);

                resultData.Success();
            }
            catch (Exception ex)
            {
                resultData.SystemError(ex);
            }
            return resultData;
        }
    }
}
