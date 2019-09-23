using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Handlers
{
    /// <summary>
    /// GetQRCodeHandler 的摘要说明
    /// </summary>
    public class GetQRCodeHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string merchantId = context.Request.QueryString["id"];
            MerchantAccount merchant = new MerchantBLL().GetById(new Guid(merchantId));

            string strCode = FiiiPay.Framework.Component.QRCode.Generate(SystemPlatform.FiiiPOS, QRCodeEnum.MerchantScanPay, merchant.Id.ToString());

            QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qrcode = new QRCoder.QRCode(qrCodeData);
            Bitmap qrCodeImage = qrcode.GetGraphic(17, Color.Black, Color.White, null, 15, 6, false);
            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Jpeg);

            string bgImagePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + "/images/bg-en.jpg");
            List<ImageMergeEntity> imgs = new List<ImageMergeEntity>();
            var bm = new Bitmap(bgImagePath);
            imgs.Add(new ImageMergeEntity
            {
                ContentType = ImageContentType.Image,
                Image = new Bitmap(bgImagePath),
                Index = 0
            });
            imgs.Add(new ImageMergeEntity
            {
                ContentType = ImageContentType.Image,
                Image = new Bitmap(ms),
                Index = 0,
                X = 300,
                Y = 620,
                AreaHeight = 650,
                AreaWidth = 650
            });
            imgs.Add(new ImageMergeEntity
            {
                ContentType = ImageContentType.Text,
                Text = merchant.MerchantName.Length > 10 ? merchant.MerchantName.Substring(0, 8) + "..." : merchant.MerchantName,
                FontSize = 48,
                FontColor = Color.Black,
                Index = 4,
                X = 220,
                Y = 1310,
                AreaHeight = 80,
                AreaWidth = 785
            });
            var imgByte = ImageHelper.ImageMerge(imgs.ToArray());
            context.Response.ContentType = "image/jpeg";
            context.Response.BinaryWrite(imgByte);
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}