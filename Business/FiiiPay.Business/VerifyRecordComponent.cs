using System;
using System.Threading.Tasks;
using FiiiPay.Business.Properties;
using FiiiPay.Data;
using FiiiPay.DTO.Messages;
using FiiiPay.Entities;
using FiiiPay.Framework;

namespace FiiiPay.Business
{
    public class VerifyRecordComponent
    {
        public VerifyRecord GetById(long id)
        {
            return new VerifyRecordDAC().GetById(id);
        }

        public GetNewsOM GetRecord(long id)
        {
            var model = new VerifyRecordDAC().GetById(id);
            if (model == null)
            {
                return null;
            }

            string title;
            string body;
            switch (model.Type)
            {
                case VerifyRecordType.UserLv1Verified:
                    title = Resources.ResourceManager.GetString("KYC_LV1_VERIFIEDTitle");
                    body = model.Body;
                    if (string.IsNullOrWhiteSpace(model.Body))
                    {
                        body = Resources.ResourceManager.GetString("User_KYC_LV1_DefaultBody");
                    }
                    break;
                case VerifyRecordType.UserLv1Reject:
                    title = Resources.ResourceManager.GetString("KYC_LV1_REJECTTitle");
                    body = model.Body;
                    break;
                case VerifyRecordType.UserLv2Verified:
                    title = Resources.ResourceManager.GetString("KYC_LV2_VERIFIEDTitle");
                    body = model.Body;
                    if (string.IsNullOrWhiteSpace(model.Body))
                    {
                        body = Resources.ResourceManager.GetString("User_KYC_LV2_DefaultBody");
                    }
                    break;
                case VerifyRecordType.UserLv2Reject:
                    title = Resources.ResourceManager.GetString("KYC_LV2_REJECTTitle");
                    body = model.Body;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new GetNewsOM
            {
                Title = title,
                Timestamp = model.CreateTime.ToUnixTime(),
                Content = body
            };
        }

        public async Task<VerifyMessageOM> GetFiiipayMerchantMessageAsync(long id)
        {
            var verifyRecord = await new FiiipayMerchantVerifyRecordDAC().GetByIdAsync(id);
            if (verifyRecord.VerifyStatus == Entities.Enums.VerifyStatus.Certified)
            {
                return new VerifyMessageOM
                {
                    Title = Resources.ResourceManager.GetString("FiiipayMerchantCertifiedTitle"),
                    Timestamp = verifyRecord.CreateTime.ToUnixTime(),
                    Content = Resources.ResourceManager.GetString("FiiipayMerchantVerifyDetail"),
                    MerchantInfoId = verifyRecord.MerchantInfoId
                };
            }
            else if (verifyRecord.VerifyStatus == Entities.Enums.VerifyStatus.Disapproval)
            {
                return new VerifyMessageOM
                {
                    Title = Resources.ResourceManager.GetString("FiiipayMerchantDisapprovalTitle"),
                    Timestamp = verifyRecord.CreateTime.ToUnixTime(),
                    Content = string.Format(Resources.ResourceManager.GetString("FiiipayMerchantDisapprovalDetail"), verifyRecord.Message),
                    MerchantInfoId = verifyRecord.MerchantInfoId
                };
            }
            else
                return null;
        }
    }
}
