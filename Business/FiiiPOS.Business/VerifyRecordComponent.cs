using System;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPOS.Business.Properties;
using FiiiPOS.DTO;

namespace FiiiPOS.Business
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
                
                case VerifyRecordType.MerchantLv1Verified:
                    title = Resources.ResourceManager.GetString("MerchantLv1Verified");
                    body = model.Body;
                    if (string.IsNullOrWhiteSpace(model.Body))
                    {
                        body = Resources.ResourceManager.GetString("Merchant_KYC_LV1_DefaultBody");
                    }
                    break;
                case VerifyRecordType.MerchantLv1Reject:
                    title = Resources.ResourceManager.GetString("MerchantLv1Reject");
                    body = model.Body;
                    break;
                case VerifyRecordType.MerchantLv2Verified:
                    title = Resources.ResourceManager.GetString("MerchantLv2Verified");
                    body = model.Body;
                    if (string.IsNullOrWhiteSpace(model.Body))
                    {
                        body = Resources.ResourceManager.GetString("Merchant_KYC_LV2_DefaultBody");
                    }
                    break;
                case VerifyRecordType.MerchantLv2Reject:
                    title = Resources.ResourceManager.GetString("MerchantLv2Reject");
                    body = model.Body;
                    break;
                case VerifyRecordType.StoreVerified:
                    title = Resources.ResourceManager.GetString("StoreVerified");
                    body = model.Body;
                    if (string.IsNullOrWhiteSpace(model.Body))
                    {
                        body = Resources.ResourceManager.GetString("Store_DefaultBody");
                    }
                    break;
                case VerifyRecordType.StoreReject:
                    title = Resources.ResourceManager.GetString("StoreReject");
                    body = model.Body;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new GetNewsOM
            {
                Title = title,
                PublishTime = model.CreateTime.ToUnixTime(),
                Content = body
            };
        }
    }
}
