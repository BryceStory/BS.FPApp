using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    public class FileManageController : BaseController
    {
        // GET: FileManage
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        public JsonResult LoadData(GridPager pager, string id)
        {
            List<Files> AccountList = FoundationDB.DB.Queryable<Files>().WhereIF(!string.IsNullOrEmpty(id), t => t.Id.ToString().Contains(id)).ToList();
            var obj = AccountList.ToGridJson(ref pager, r =>
                new
                {
                    id = r.Id,
                    cell = new string[]{
                        r.Id.ToString(),
                        r.FileName,
                        r.FileType,
                        r.MimeType,
                        r.FilePath,
                        r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                });
            //total 总页数, page 当前页, records 总记录数
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [BOAccess("FileDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            FileBLL fb = new FileBLL();
            return Json(fb.DeleteById(id, UserId, UserName).toJson());
        }


        [HttpPost]
        public void Download(string id)
        {
            var file = FoundationDB.FileDb.GetById(id);
            BlobBLL bb = new BlobBLL();
            var byteStr = bb.Download(id);
            if (byteStr != null)
            {
                Response.ContentType = "application/octet-stream";
                //通知浏览器下载文件而不是打开 
                Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(file.FileName, System.Text.Encoding.UTF8));
                Response.BinaryWrite(byteStr);
                Response.Flush();
                Response.End();
            }
        }
    }
}