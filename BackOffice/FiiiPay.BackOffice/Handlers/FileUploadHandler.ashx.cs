using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FiiiPay.Framework.Cache;

namespace FiiiPay.BackOffice.Handlers
{
    /// <summary>
    /// FileUploadHandler 的摘要说明
    /// </summary>
    public class FileUploadHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string token = "loginuser";
            var loginId = context.Request.Cookies["LoginUser"];
            var cookieValue = Encrypts.GetDecryptString(loginId.Value);
            var cookieValues = cookieValue.Split(new char[] { '_' });
            if (cookieValues == null || cookieValues.Length != 2)
                return;
            var userId = int.Parse(cookieValues[0]);
            var ticks = long.Parse(cookieValues[1]);
            DateTime dtNow = DateTime.UtcNow;
            DateTime dtToken = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(ticks);

            var totalMinutes = (dtNow - dtToken).TotalMinutes;
            if ((dtNow - dtToken).TotalMinutes > 35)//长时间没访问
                return;

            token += userId;

            var loginSession = RedisHelper.Get<LoginUser>(token);
            if (loginSession == null)
            {
                return;
            }

            string saveType = context.Request["savetype"];
            string callback = context.Request["callback"];
            string json;
            if (saveType!=null && saveType.ToLower() == "local")
            {
                json = UploadToLocal(context);
            }
            else
            {
                json = UploadToCDN(context);
            }

            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.ContentType = "text/html";
            if (callback != null)
            {
                context.Response.Write(String.Format("<script>{0}(JSON.parse(\"{1}\"));</script>", callback, json));
            }
            else
            {
                context.Response.Write(json);
            }
        }

        private string UploadToLocal(HttpContext context)
        {
            //上传配置
            string pathbase = System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"];//保存路径
            if (!pathbase.StartsWith("/"))
                pathbase = "/" + pathbase;
            if (!pathbase.EndsWith("/"))
                pathbase += "/";
            pathbase += "UMeditorUpload/";
            int size = 10;//文件大小限制,单位mb 
            string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };//文件允许格式
            
            //上传图片
            Hashtable info;
            FileUploader up = new FileUploader();
            info = up.UpFileToLocal(context, pathbase, filetype, size); //获取上传状态
            return BuildJson(info);
        }

        private string UploadToCDN(HttpContext context)
        {
            int size = 10;//文件大小限制,单位mb 
            string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };//文件允许格式
            Hashtable info = new FileUploader().UpFileToCDN(context, filetype, size);
            return BuildJson(info);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string BuildJson(Hashtable info)
        {
            List<string> fields = new List<string>();
            string[] keys = new string[] {"fileId", "originalName", "name", "url", "size", "state", "type" };
            for (int i = 0; i < keys.Length; i++)
            {
                fields.Add(String.Format("\"{0}\": \"{1}\"", keys[i], info[keys[i]]));
            }
            return "{" + String.Join(",", fields) + "}";
        }
    }
}