using FiiiPay.BackOffice.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Utils
{
    public class FileUploader
    {
        string state = "SUCCESS";

        string URL = null;
        string currentType = null;
        string uploadpath = null;
        string filename = null;
        string originalName = null;
        string FullPath = null;
        string fileId = null;
        HttpPostedFile uploadFile = null;
        
        public Hashtable UpFileToLocal(HttpContext cxt, string pathbase, string[] filetype, int size)
        {
            pathbase = pathbase + DateTime.UtcNow.ToString("yyyy-MM-dd") + "/";
            uploadpath = cxt.Server.MapPath(pathbase);//获取文件上传路径

            try
            {
                uploadFile = cxt.Request.Files[0];
                originalName = uploadFile.FileName;

                //目录创建
                CreateFolder();

                //格式验证
                if (CheckType(filetype))
                {
                    state = "File type error";
                }
                //大小验证
                if (CheckSize(size))
                {
                    state = "File size overstep the setting";
                }
                //保存图片
                if (state == "SUCCESS")
                {
                    fileId = Guid.NewGuid().ToString();
                    filename = fileId + GetFileExt();
                    uploadFile.SaveAs(uploadpath + filename);
                    URL = pathbase + filename;
                }
            }
            catch (Exception e)
            {
                state = "Unknown error";
                URL = "";
            }
            return GetUploadInfo();
        }

        public Hashtable UpFileToCDN(HttpContext cxt, string[] filetype, int size)
        {
            try
            {
                uploadFile = cxt.Request.Files[0];
                originalName = uploadFile.FileName;
                //格式验证
                if (CheckType(filetype))
                {
                    state = "File type error";
                }
                //大小验证
                if (CheckSize(size))
                {
                    state = "File size overstep the setting";
                }
                
                //保存图片
                if (state == "SUCCESS")
                {
                    FileUploadModel model = new FileUploadModel();
                    model.FileName = uploadFile.FileName;
                    model.FileType = uploadFile.ContentType;
                    byte[] buffer = new byte[uploadFile.ContentLength];
                    uploadFile.InputStream.Read(buffer, 0, uploadFile.ContentLength);
                    model.File = buffer;

                    fileId = new BLL.BlobBLL().UploadImage(model);
                    URL = System.Configuration.ConfigurationManager.AppSettings["ArticleImageHandler"] + "?id=" + fileId;
                }
            }
            catch (Exception e)
            {
                state = "Unknown error";
                URL = "";
            }
            return GetCNDUploadInfo();
        }

        public string UpImageToCDN(HttpPostedFileBase uploadImage)
        {
            try
            {
                FileUploadModel model = new FileUploadModel();
                model.FileName = uploadImage.FileName;
                model.FileType = uploadImage.ContentType;
                byte[] buffer = new byte[uploadImage.ContentLength];
                uploadImage.InputStream.Read(buffer, 0, uploadImage.ContentLength);
                model.File = buffer;

                fileId = new BLL.BlobBLL().UploadImage(model);
                return fileId;
            }
            catch(Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(FileUploader));
                log.Error(ex);
                return null;
            }
        }

        public Hashtable UpScrawl(HttpContext cxt, string pathbase, string tmppath, string base64Data)
        {
            pathbase = pathbase + DateTime.UtcNow.ToString("yyyy-MM-dd") + "/";
            uploadpath = cxt.Server.MapPath(pathbase);//获取文件上传路径
            FileStream fs = null;
            try
            {
                //创建目录
                CreateFolder();
                //生成图片
                filename = System.Guid.NewGuid() + ".png";
                fs = File.Create(uploadpath + filename);
                byte[] bytes = Convert.FromBase64String(base64Data);
                fs.Write(bytes, 0, bytes.Length);

                URL = pathbase + filename;
            }
            catch (Exception e)
            {
                state = "Unknown error";
                URL = "";
            }
            finally
            {
                fs.Close();
                DeleteFolder(cxt.Server.MapPath(tmppath));
            }
            return GetUploadInfo();
        }
        
        public string GetOtherInfo(HttpContext cxt, string field)
        {
            string info = null;
            if (cxt.Request.Form[field] != null && !String.IsNullOrEmpty(cxt.Request.Form[field]))
            {
                info = field == "fileName" ? cxt.Request.Form[field].Split(',')[1] : cxt.Request.Form[field];
            }
            return info;
        }
        
        private Hashtable GetUploadInfo()
        {
            Hashtable infoList = new Hashtable();

            infoList.Add("fileId", fileId);
            infoList.Add("state", state);
            infoList.Add("url", URL);
            infoList.Add("originalName", originalName);
            infoList.Add("name", Path.GetFileName(URL));
            infoList.Add("size", uploadFile.ContentLength);
            infoList.Add("type", Path.GetExtension(originalName));
            infoList.Add("fullpath", uploadpath + Path.GetFileName(URL));

            return infoList;
        }

        private Hashtable GetCNDUploadInfo()
        {
            Hashtable infoList = new Hashtable();

            infoList.Add("fileId", fileId);
            infoList.Add("state", state);
            infoList.Add("url", URL);
            infoList.Add("originalName", originalName);
            infoList.Add("name", originalName);
            infoList.Add("size", uploadFile.ContentLength);
            infoList.Add("type", Path.GetExtension(originalName));

            return infoList;
        }
        
        private bool CheckType(string[] filetype)
        {
            if (filetype == null)
                return false;
            currentType = GetFileExt();
            return Array.IndexOf(filetype, currentType) == -1;
        }
        
        private bool CheckSize(int size)
        {
            if (size == 0)
                return false;
            return uploadFile.ContentLength >= (size * 1024 * 1024);
        }
        
        private string GetFileExt()
        {
            string[] temp = uploadFile.FileName.Split('.');
            return "." + temp[temp.Length - 1].ToLower();
        }
        
        private void CreateFolder()
        {
            if (!Directory.Exists(uploadpath))
            {
                Directory.CreateDirectory(uploadpath);
            }
        }
        
        public void DeleteFolder(string path)
        {
            //if (Directory.Exists(path))
            //{
            //    Directory.Delete(path, true);
            //}
        }
    }
}