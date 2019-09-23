using FiiiPay.BackOffice.Common;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Entities;
using System;

namespace FiiiPay.BackOffice.BLL
{
    public class BlobBLL:BaseBLL
    {

        public byte[] Download(string id)
        {
            return new MasterImageAgent().Download(id);
        }

        public byte[] DeleteImage(string id)
        {
            return new MasterImageAgent().Delete(id);
        }

        public string UploadImage(FileUploadModel model)
        {
            return new MasterImageAgent().Upload(model.FileName, model.File);
        }

        public Guid UploadWithCompress(Guid imageId)
        {
            return new MasterImageAgent().UploadWithCompress(imageId);
        }

        public byte[] Download(string id, int countryId)
        {
            return new RegionImageAgent(countryId).Download(id);
        }

        public BlobRouter GetRouter(int countryId)
        {
            var br = FiiiPayDB.DB.Ado.SqlQuerySingle<BlobRouter>("SELECT * FROM BlobRouters WHERE CountryId=@CountryId", new { CountryId = countryId });
            return br;
        }
    }
}