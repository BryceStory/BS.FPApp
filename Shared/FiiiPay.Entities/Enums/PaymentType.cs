namespace FiiiPay.Entities.Enums
{
    public enum PaymentType : byte
    {
        ScanCode = 1,
        NFC,
        Bluetooth,
        QRCode,
        /// <summary>
        /// 商家固态二维码
        /// </summary>
        MerchantStaticQRCode,
        Gateway
    }
}