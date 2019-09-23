using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("AgentMenu")]
    public class AgentController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.CountryList = GetCountrySelectList();
            return View();
        }

        [HttpPost]
        public JsonResult LoadData(string companyName, int? countryId, int? stateId, int? cityId, GridPager pager)
        {
            List<AgentViewModel> agentList = new AgentBLL().GetAgentPersonPager(companyName, countryId, stateId, cityId, ref pager);
            if (agentList == null || agentList.Count <= 0)
                return Json(new List<AgentViewModel>().ToGridJson(pager));

            var areaBll = new AreaBLL();
            var countryIds = agentList.Select(t => t.CountryId).ToList();
            var countryList = areaBll.GetCountrysByIds(countryIds);
            var stateList = areaBll.GetStatesByCountryIds(countryIds);
            var stateIds = stateList.Select(t => t.Id).ToList();
            var cityList = areaBll.GetCitysByStateIds(stateIds);

            var obj = agentList.ToGridJson(pager, r =>
                    new
                    {
                        id = r.Id,
                        cell = new
                        {
                            Id = r.Id,
                            AgentCode = r.AgentCode,
                            CompanyName = r.CompanyName,
                            CountryName = countryList.Where(t=>t.Id==r.CountryId).Select(t=>t.Name).FirstOrDefault(),
                            StateName = stateList.Where(t=>t.Id==r.StateId).Select(t=>t.Name).FirstOrDefault(),
                            CityName = cityList.Where(t => t.Id == r.CityId).Select(t => t.Name).FirstOrDefault(),
                            ContactName = r.ContactName,
                            ContactWay = r.ContactWay,
                            SaleName= r.SaleName,
                            CreateTime = r.CreateTime.ToString("yyyy-MM-dd")
                        }
                    });
            return Json(obj);
        }

        public JsonResult GetStateList(int countryId)
        {
            var data = new AreaBLL().GetStatesByCountryId(countryId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCityList(int stateId)
        {
            var data = new AreaBLL().GetCitysByStateId(stateId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            ViewBag.SellerList = GetSellerSelectList();
            ViewBag.CountryList = GetCountrySelectList();
            ViewBag.StateList = GetStateSelectList(0);
            ViewBag.CityList = GetCitySelectList(0);
            return View("Edit", new Agent());
        }
        public ActionResult Edit(int id)
        {
            Agent agent = BoDB.AgentDb.GetById(id);
            ViewBag.SellerList = GetSellerSelectList();
            ViewBag.CountryList = GetCountrySelectList();
            ViewBag.StateList = GetStateSelectList(agent.CountryId);
            ViewBag.CityList = GetCitySelectList(agent.StateId);
            return View(agent);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public JsonResult Save(Agent agent)
        {
            SaveResult sr;
            if (agent.Id > 0)
            {
                agent.ModifyBy = UserId;
                sr = SaveEdit(agent);
            }
            else
            {
                agent.CreateBy = UserId;
                sr = SaveCreate(agent);
            }
            return Json(sr.toJson());
        }
        [BOAccess("AgentCreate")]
        private SaveResult SaveCreate(Agent agent)
        {
            return new AgentBLL().SaveCreate(agent, UserId, UserName);
        }
        [BOAccess("AgentUpdate")]
        private SaveResult SaveEdit(Agent agent)
        {
            return new AgentBLL().SaveEdit(agent, UserId, UserName);
        }
        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("AgentDelete")]
        public JsonResult Delete(int id)
        {
            var sr = new AgentBLL().Delete(id, UserId, UserName);
            return Json(sr.toJson());
        }

        private List<SelectListItem> GetCountrySelectList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            oList.Add(new SelectListItem() { Text = "Select country" });
            var countryList = FoundationDB.CountryDb.GetList();
            if (countryList != null&&countryList.Count>0)
            {
                oList.AddRange(countryList.Select(t => new SelectListItem() { Text = t.Name, Value = t.Id.ToString() }));
            }
            return oList;
        }

        public List<SelectListItem> GetStateSelectList(int countryId=0)
        {
            List<States> data;
            if (countryId > 0)
                data = new AreaBLL().GetStatesByCountryId(countryId);
            else
                data = null;
            List<SelectListItem> oList = new List<SelectListItem>();
            oList.Add(new SelectListItem() { Text = "Select state" });
            if (data != null && data.Count > 0)
            {
                oList.AddRange(data.Select(t => new SelectListItem() { Text = t.Name, Value = t.Id.ToString() }));
            }
            return oList;
        }

        public List<SelectListItem> GetCitySelectList(int stateId=0)
        {
            List<Cities> data;
            if (stateId > 0)
                data = new AreaBLL().GetCitysByStateId(stateId);
            else
                data = null;
            List<SelectListItem> oList = new List<SelectListItem>();
            oList.Add(new SelectListItem() { Text = "Select city" });
            if (data != null && data.Count > 0)
            {
                oList.AddRange(data.Select(t => new SelectListItem() { Text = t.Name, Value = t.Id.ToString() }));
            }
            return oList;
        }

        private List<SelectListItem> GetSellerSelectList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            oList.Add(new SelectListItem() { Text = "Select seller" });
            var sellerList = BoDB.SalespersonDb.GetList();
            if (sellerList != null && sellerList.Count > 0)
            {
                oList.AddRange(sellerList.Select(t => new SelectListItem() { Text = t.SaleName, Value = t.Id.ToString() }));
            }
            return oList;
        }
    }
}