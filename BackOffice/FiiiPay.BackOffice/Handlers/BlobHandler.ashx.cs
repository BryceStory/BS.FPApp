using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Utils;
using FiiiPay.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Handlers
{
    /// <summary>
    /// BlobHandler 的摘要说明
    /// </summary>
    public class BlobHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string fileId = context.Request.QueryString["id"];
            string cid = context.Request.QueryString["cid"];
            byte[] imgByte = null;
            try
            {
                if(string.IsNullOrEmpty(cid))
                    imgByte = new BlobBLL().Download(fileId);
                else
                {
                    int countryId = Convert.ToInt32(cid);
                    imgByte = new BlobBLL().Download(fileId, countryId);
                }
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(BlobHandler));
                log.Error(ex);
            }
            if(imgByte==null)
                imgByte = System.IO.File.ReadAllBytes(context.Server.MapPath("~/Content/Images/NoFileFound.png"));
            context.Response.ContentType = "image/jpeg";
            context.Response.BinaryWrite(imgByte);
            context.Response.End();
        }
        
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}