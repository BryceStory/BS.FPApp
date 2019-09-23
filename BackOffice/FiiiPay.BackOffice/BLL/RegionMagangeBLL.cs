using FiiiPay.BackOffice.Common;
using FiiiPay.Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class RegionMagangeBLL:BaseBLL
    {
        public List<Regions> GetRegionPager(int? countryId, ref GridPager pager)
        {
            int totalPage = 0, totalNumber = 0;

            var data = FoundationDB.DB.Queryable<Regions>()
                .Where(t => t.RegionLevel == RegionLevel.State)
                .WhereIF(countryId.HasValue, t => t.CountryId == countryId.Value)
                .OrderBy(t => t.Sort)
                .ToPageList(pager.Page, pager.Size, ref totalNumber, ref totalPage);
            pager.TotalPage = totalPage;
            pager.Count = totalNumber;
            return data;
        }

        public List<Regions> GetChildrenRegionPager(long parentId,int parentLevel, ref GridPager pager)
        {
            int totalPage = 0, totalNumber = 0;
            RegionLevel queryLevel = (RegionLevel)(parentLevel + 1);
            var data = FoundationDB.DB.Queryable<Regions>()
                .Where(t => t.RegionLevel == queryLevel && t.ParentId == parentId)
                .OrderBy(t => t.Sort)
                .ToPageList(pager.Page, pager.Size, ref totalNumber, ref totalPage);
            pager.TotalPage = totalPage;
            pager.Count = totalNumber;
            return data;
        }

        public SaveResult<Regions> SaveAdd(Regions r)
        {
            r.Status = RegionStatus.Enable;
            r.ParentId = r.CountryId;
            r.RegionLevel = RegionLevel.State;

            var id = FoundationDB.DB.Insertable(r).ExecuteReturnBigIdentity();
            if (id <= 0)
                return new SaveResult<Regions>(false);
            r.Id = id;
            return new SaveResult<Regions>(true, r);
        }

        public SaveResult<Regions> SaveEdit(Regions r)
        {
            var originRegions = FoundationDB.DB.Queryable<Regions>().First(t => t.Id == r.Id);
            originRegions.Code = r.Code;
            originRegions.Name = r.Name;
            originRegions.NameCN = r.NameCN;
            originRegions.Sort = r.Sort;
            
            FoundationDB.DB.Updateable(originRegions).ExecuteCommand();
            
            return new SaveResult<Regions>(true, originRegions);
        }

        public SaveResult<Regions> SaveChildAdd(Regions r)
        {
            var parent = FoundationDB.DB.Queryable<Regions>().First(t => t.Id == r.ParentId);
            r.Status = RegionStatus.Enable;
            r.CountryId = parent.CountryId;
            r.RegionLevel = parent.RegionLevel + 1;

            var id = FoundationDB.DB.Insertable(r).ExecuteReturnBigIdentity();
            if (id <= 0)
                return new SaveResult<Regions>(false);
            r.Id = id;
            return new SaveResult<Regions>(true, r);
        }
    }
}