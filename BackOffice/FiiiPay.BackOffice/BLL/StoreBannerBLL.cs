using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class StoreBannerBLL:BaseBLL
    {
        public SaveResult<long> SaveAdd(StoreBanners banner)
        {
            banner.Sort = 1;
            banner.Timestamp = DateTime.UtcNow;

            var id = FiiiPayDB.StoreBannerDb.InsertReturnIdentity(banner);

            return new SaveResult<long>(true, id);
        }

        public SaveResult SaveEdit(StoreBanners banner)
        {
            var originalBanner = FiiiPayDB.StoreBannerDb.GetById(banner.Id);
            if (originalBanner == null)
                return new SaveResult(false);
            originalBanner.Title = banner.Title;
            originalBanner.OpenByAPP = banner.OpenByAPP;
            originalBanner.LinkUrl = banner.LinkUrl;
            originalBanner.Status = banner.Status;
            originalBanner.StartTime = banner.StartTime;
            originalBanner.EndTime = banner.EndTime;
            originalBanner.CountryId = banner.CountryId;
            originalBanner.PictureId = banner.PictureId;
            originalBanner.ViewPermission = banner.ViewPermission;

            FiiiPayDB.StoreBannerDb.Update(originalBanner);
            return new SaveResult(true);
        }

        public SaveResult SetTop(long id)
        {
            FiiiPayDB.DB.Updateable<StoreBanners>().SetColumns(t => new StoreBanners { Sort = 0 })
                .Where(t => t.Id == id).ExecuteCommand();
            return new SaveResult(true);
        }

        public SaveResult Delete(long id)
        {
            var sr = FiiiPayDB.StoreBannerDb.DeleteById(id);
            return new SaveResult(sr);
        }
    }
}