using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("POSSNManageMenu")]
    public class POSSNManageController : BaseController
    {
        // GET: POSSNManage
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.PageId = id;
            return View();
        }

        public JsonResult LoadData(GridPager pager, string SN, DateTime? startDate, DateTime? endDate)
        {
            List<POSs> data = FiiiPayDB.DB.Queryable<POSs>()
                .WhereIF(!string.IsNullOrEmpty(SN), t => t.Sn.Contains(SN))
                .WhereIF(startDate.HasValue, r => r.Timestamp > startDate.Value.AddDays(-1))
                .WhereIF(endDate.HasValue, r => r.Timestamp < endDate.Value.AddDays(1))
                .ToList();

            var obj = data.ToGridJson(ref pager, r =>
            new
            {
                id = r.Id,
                cell = new string[]{
                        r.Id.ToString(),
                        r.Sn,
                        r.Status?"Used":"Not Used",
                        r.IsWhiteLabel.ToString(),
                        r.WhiteLabel,
                        r.FirstCrypto,
                        r.IsMiningEnabled ?"Enable":"Disable",
                        r.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                    }
            });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            return PartialView();
        }

        public ActionResult SetWhiteLabel()
        {
            var list = FoundationDB.CryptocurrencyDb.GetList();
            List<SelectListItem> oList = new List<SelectListItem>();
            oList.Add(new SelectListItem() { Text = "", Value = "" });
            foreach (var item in list)
            {
                oList.Add(new SelectListItem() { Text = item.Name, Value = item.Code.ToString() });
            }
            ViewBag.CURList = oList;

            return PartialView();
        }

        public ActionResult Import(int pageId)
        {
            ViewBag.PageId = pageId;
            return PartialView();
        }

        public ActionResult ImportWhiteLabel(int pageId)
        {
            var list = FoundationDB.CryptocurrencyDb.GetList();
            List<SelectListItem> oList = new List<SelectListItem>();
            oList.Add(new SelectListItem() { Text = "", Value = "" });
            foreach (var item in list)
            {
                oList.Add(new SelectListItem() { Text = item.Name, Value = item.Code.ToString() });
            }
            ViewBag.CURList = oList;
            ViewBag.PageId = pageId;
            return PartialView();
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("POSCreate")]
        public ActionResult Save(POSs oPos)
        {
            POSBLL ab = new POSBLL();
            SaveResult result = new SaveResult();
            result = ab.Create(oPos, UserId, UserName);
            return Json(result.toJson());
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("POSDelete")]
        public ActionResult Delete(int id)
        {
            POSBLL ab = new POSBLL();
            return Json(ab.DeleteById(id, UserId, UserName).toJson());
        }

        [HttpPost]
        public void Download()
        {
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet("Sheet1");
            IRow row1 = sheet.CreateRow(0);
            row1.CreateCell(0, CellType.String).SetCellValue("POS SN");
            row1.CreateCell(1, CellType.String).SetCellValue("Whether to Open Mining");
            row1.CreateCell(2, CellType.String).SetCellValue("Please note: This is the headline, the column header is POS SN, cannot be modified and deleted, please enter SN  number in POS SN column");
            ICellStyle cellStyle = book.CreateCellStyle();
            //row1.GetCell(1).CellStyle=
            Response.Charset = System.Text.Encoding.UTF8.BodyName;
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"POS SN List.xls\"");
            book.Write(Response.OutputStream);
            Response.Flush();
            Response.End();
        }

        [HttpPost]
        [BOAccess("BatchUpdate")]
        [AjaxAntiForgeryToken]
        public ActionResult BatchUpdate(List<long> ids, bool isEnable)
        {
            POSBLL bll = new POSBLL();
            var result = bll.BatchUpdate(ids, isEnable, UserId, UserName);
            return Json(result.toJson());
        }

        [HttpPost]
        [BOAccess("BatchUpdate")]
        [AjaxAntiForgeryToken]
        public ActionResult MarkWhiteLabel(List<long> ids, string whiteLabel, string firstCrypto)
        {
            POSBLL bll = new POSBLL();
            var result = bll.MarkWhiteLabel(ids, whiteLabel, firstCrypto, UserId, UserName);
            return Json(result.toJson());
        }

        [HttpPost]
        [BOAccess("BatchUpdate")]
        [AjaxAntiForgeryToken]
        public ActionResult UnMarkWhiteLabel(List<long> ids)
        {
            POSBLL bll = new POSBLL();
            var result = bll.UnMarkWhiteLabel(ids, UserId, UserName);
            return Json(result.toJson());
        }

        [HttpPost, ValidateAntiForgeryToken]
        [BOAccess("ImportPOSSN")]
        public ActionResult ImportPOSSN()
        {
            POSBLL ab = new POSBLL();
            //导入文件
            HttpPostedFileBase file = Request.Files["importSN"];
            DataTable dt = ExcelToDataTable(file);
            List<string> snList = new List<string>();
            List<POSs> list = new List<POSs>();
            foreach (DataRow item in dt.Rows)
            {
                if (!item[0].ToString().StartsWith("N3000"))
                {
                    continue;
                }
                POSs pos = new POSs();
                pos.Sn = item[0].ToString();
                pos.Status = false;
                pos.Timestamp = DateTime.UtcNow;
                if (item[0] != null && item[1].ToString().Equals("Open"))
                {
                    pos.IsMiningEnabled = true;
                }
                list.Add(pos);
                snList.Add(item[0].ToString());
            }
            List<POSs> listRepeat = new List<POSs>();
            if (list.Count > 0)
            {
                listRepeat = ab.Import(list, snList, UserId, UserName);
                if (listRepeat.Count > 0)
                {
                    HSSFWorkbook book = new HSSFWorkbook();
                    ISheet sheet = book.CreateSheet("Sheet1");
                    IRow row1 = sheet.CreateRow(0);
                    row1.CreateCell(0, CellType.String).SetCellValue("POS SN");
                    row1.CreateCell(1, CellType.String).SetCellValue("Imported file exist duplicate POS SN numbers, please try again after delete the duplicate SN numbers");

                    for (int i = 0; i < listRepeat.Count; i++)
                    {
                        IRow row = sheet.CreateRow(i + 1);
                        row.CreateCell(0, CellType.String).SetCellValue(listRepeat[i].Sn);
                    }

                    ICellStyle cellStyle = book.CreateCellStyle();
                    Response.Charset = System.Text.Encoding.UTF8.BodyName;
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=\"POS SN List.xls\"");
                    book.Write(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            return RedirectToAction("Index", new { id = Request["PageId"] });
        }
        [HttpPost, ValidateAntiForgeryToken]
        [BOAccess("ImportPOSSN")]
        public ActionResult ImportWhiteLabelPOSSN()
        {
            POSBLL ab = new POSBLL();
            //导入文件
            HttpPostedFileBase file = Request.Files["importSN"];
            DataTable dt = ExcelToDataTable(file);
            List<string> snList = new List<string>();
            List<POSs> list = new List<POSs>();
            foreach (DataRow item in dt.Rows)
            {
                if (!item[0].ToString().StartsWith("N3000"))
                {
                    continue;
                }
                POSs pos = new POSs();
                pos.Sn = item[0].ToString();
                pos.Status = false;
                pos.Timestamp = DateTime.UtcNow;
                pos.WhiteLabel = Request.Form["WhiteLabel"];
                pos.IsWhiteLabel = true;
                pos.FirstCrypto = Request.Form["FirstCrypto"];
                if (item[1].ToString().Equals("Open"))
                {
                    pos.IsMiningEnabled = true;
                }
                list.Add(pos);

                snList.Add(item[0].ToString());
            }
            List<POSs> listRepeat = new List<POSs>();
            if (list.Count > 0)
            {
                listRepeat = ab.Import(list, snList, UserId, UserName);
                if (listRepeat.Count > 0)
                {
                    HSSFWorkbook book = new HSSFWorkbook();
                    ISheet sheet = book.CreateSheet("Sheet1");
                    IRow row1 = sheet.CreateRow(0);
                    row1.CreateCell(0, CellType.String).SetCellValue("POS SN");
                    row1.CreateCell(1, CellType.String).SetCellValue("Imported file exist duplicate POS SN numbers, please try again after delete the duplicate SN numbers");

                    for (int i = 0; i < listRepeat.Count; i++)
                    {
                        IRow row = sheet.CreateRow(i + 1);
                        row.CreateCell(0, CellType.String).SetCellValue(listRepeat[i].Sn);
                    }

                    ICellStyle cellStyle = book.CreateCellStyle();
                    Response.Charset = System.Text.Encoding.UTF8.BodyName;
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=\"POS SN List.xls\"");
                    book.Write(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            return RedirectToAction("Index", new { id = Request["PageId"] });
        }

        /// <summary>读取excel
        /// 默认第一行为表头
        /// </summary>
        /// <param name="strFileName">excel文档绝对路径</param>
        /// <returns></returns>
        public DataTable ExcelToDataTable(HttpPostedFileBase file)
        {
            System.IO.Stream MyStream;
            int FileLen;
            FileLen = file.ContentLength;
            // 读取文件的 byte[]   
            //byte[] bytes = new byte[FileLen];
            MyStream = file.InputStream;
            //MyStream.Read(bytes, 0, FileLen);
            DataTable dt = new DataTable();
            HSSFWorkbook hssfworkbook;

            if (file.FileName.IndexOf(".xlsx") > 0)
            {
                hssfworkbook = new HSSFWorkbook(file.InputStream);
            }
            else if (file.FileName.IndexOf(".xls") > 0)
            {
                hssfworkbook = new HSSFWorkbook(file.InputStream);
            }
            else
            {
                throw new Exception("Excel格式错误");
            }

            ISheet sheet = hssfworkbook.GetSheetAt(0);

            IRow headRow = sheet.GetRow(0);
            int colCount = headRow.LastCellNum;
            List<POSs> posList = new List<POSs>();
            if (headRow != null)
            {

                for (int i = 1; i <= colCount; i++)
                {
                    ICell cell = headRow.GetCell(i);
                    if (cell != null)
                    {
                        if (dt.Columns.Contains(cell.ToString()))
                        {

                            throw new Exception("Excel存在重复列，列名为：" + cell.ToString());
                        }
                        dt.Columns.Add(cell.ToString());
                    }
                    else
                    {
                        dt.Columns.Add("column" + i);
                    }
                }
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                bool emptyRow = true;
                object[] itemArray = null;

                if (row != null)
                {
                    itemArray = new object[colCount];
                    if (row.FirstCellNum != -1)
                    {
                        for (int j = row.FirstCellNum; j < colCount; j++)
                        {
                            if (row.GetCell(j) != null)
                            {

                                switch (row.GetCell(j).CellType)
                                {
                                    case CellType.Numeric:
                                        if (HSSFDateUtil.IsCellDateFormatted(row.GetCell(j)))//日期类型
                                        {
                                            itemArray[j] = row.GetCell(j).DateCellValue.ToString("yyyy-MM-dd");
                                        }
                                        else//其他数字类型
                                        {
                                            itemArray[j] = row.GetCell(j).NumericCellValue;
                                        }
                                        break;
                                    case CellType.Blank:
                                        itemArray[j] = string.Empty;
                                        break;
                                    case CellType.Formula:
                                        if (Path.GetExtension(file.FileName).ToLower().Trim() == ".xlsx")
                                        {
                                            XSSFFormulaEvaluator eva = new XSSFFormulaEvaluator(hssfworkbook);
                                            if (eva.Evaluate(row.GetCell(j)).CellType == CellType.Numeric)
                                            {
                                                if (HSSFDateUtil.IsCellDateFormatted(row.GetCell(j)))//日期类型
                                                {
                                                    itemArray[j] = row.GetCell(j).DateCellValue.ToString("yyyy-MM-dd");
                                                }
                                                else//其他数字类型
                                                {
                                                    itemArray[j] = row.GetCell(j).NumericCellValue;
                                                }
                                            }
                                            else
                                            {
                                                itemArray[j] = eva.Evaluate(row.GetCell(j)).StringValue;
                                            }
                                        }
                                        else
                                        {
                                            HSSFFormulaEvaluator eva = new HSSFFormulaEvaluator(hssfworkbook);
                                            if (eva.Evaluate(row.GetCell(j)).CellType == CellType.Numeric)
                                            {
                                                if (HSSFDateUtil.IsCellDateFormatted(row.GetCell(j)))//日期类型
                                                {
                                                    itemArray[j] = row.GetCell(j).DateCellValue.ToString("yyyy-MM-dd");
                                                }
                                                else//其他数字类型
                                                {
                                                    itemArray[j] = row.GetCell(j).NumericCellValue;
                                                }
                                            }
                                            else
                                            {
                                                itemArray[j] = eva.Evaluate(row.GetCell(j)).StringValue;
                                            }
                                        }
                                        break;
                                    default:
                                        itemArray[j] = row.GetCell(j).StringCellValue;
                                        break;

                                }

                                if (itemArray[j] != null && !string.IsNullOrEmpty(itemArray[j].ToString().Trim()))
                                {
                                    emptyRow = false;
                                }
                            }
                        }
                    }


                }

                //非空数据行数据添加到DataTable
                if (!emptyRow)
                {
                    dt.Rows.Add(itemArray);
                }
            }
            return dt;
        }
    }
}