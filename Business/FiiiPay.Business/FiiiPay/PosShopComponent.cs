using FiiiPay.Business.Properties;
using FiiiPay.DTO;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Enums;
using FiiiPay.Framework.Exceptions;
using System;

namespace FiiiPay.Business.FiiiPay
{
    public class PosShopComponent
    {
        public bool ScanLogin(string code,Guid accountId)
        {
            var codeEntity = QRCode.Deserialize(code);

            if (codeEntity.SystemPlatform != SystemPlatform.FiiiShop || codeEntity.QrCodeEnum != QRCodeEnum.FiiiPayLogin)
                throw new CommonException(ReasonCode.INVALID_QRCODE, MessageResources.InvalidQRCode);

            string cKey = "FiiiShop:Login:" + codeEntity.QRCodeKey;
            FiiiShopLoginDTO dto = new FiiiShopLoginDTO
            {
                AccountId = accountId,
                Status = (byte)LoginStatus.Logined
            };
            RedisHelper.Set(Constant.REDIS_TOKEN_DBINDEX, cKey, dto, TimeSpan.FromMinutes(3));
            return true;
        }
    }
}
