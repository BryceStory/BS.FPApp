using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using FiiiPay.Business.Properties;
using FiiiPay.Data;
using FiiiPay.DTO;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Exceptions;
using MongoDB.Bson.IO;

namespace FiiiPay.Business.FiiiPay
{
    public class MerchantComponent : BaseComponent
    {
        public List<GetMerchantInfoOM> GetMerchantInfoList(GetMerchantInfoListIM im)
        {
            if (im.PageSize > 20)
                im.PageSize = 20;
            List<MerchantBriefInformation> list;
            if (im.Location == null)
            {
                list = new MerchantInformationDAC().QueryMerchantListByCountryId(im.Filter.CountryId, im.PageIndex, im.PageSize, im.Filter.KeyWord,
                    im.Filter.Category);
            }
            else
            {
                list = new MerchantInformationDAC().QueryNearbyMerchantList(im.Location.Latitude, im.Location.Longitude, im.Filter.CountryId, im.PageSize, im.PageIndex, im.Filter.KeyWord, im.Filter.Category);

            }

            var mwDAC = new MerchantWalletDAC();
            var mscDAC = new MerchantSupportCryptoDAC();
            var cryptoList = new CryptoComponent().GetList();

            return list.Select(item =>
            {
                List<int> supportCryptoIdList = new List<int>();
                if (item.AccountType == (byte)AccountType.Merchant)
                    supportCryptoIdList = mwDAC.SupportReceiptList(item.MerchantAccountId).Select(t => t.CryptoId).ToList();
                else
                    supportCryptoIdList = mscDAC.GetList(item.Id).Select(t => t.CryptoId).ToList();

                return new GetMerchantInfoOM()
                {
                    AccountType = item.AccountType,
                    IsAllowExpense = item.IsAllowExpense,
                    Address = item.Address,
                    AvailableCryptoIconList = cryptoList.Where(c => supportCryptoIdList.Contains(c.Id)).Select(t => t.IconURL.ToString()).ToList(),
                    Distance = item.Distance.ToSpecificDecimal(2).ToString(CultureInfo.InvariantCulture),
                    OriginIconId = item.FileId,
                    CompressIconId = item.ThumbnailId ?? Guid.Empty,
                    Id = item.MerchantInformationId,
                    Location = new Location() { Longitude = item.Lng, Latitude = item.Lat },
                    Tags = item.Tags.Split(',').ToList(),
                    Title = item.MerchantName
                };
            }).ToList();
        }

        public List<GetMerchantInfoOM> GetMerchantInfoList(GetMerchantInfoListByMapIM im)
        {
            var list = new MerchantInformationDAC().QueryNearbyMerchantList(im.LeftTop.Longitude, im.LeftTop.Latitude, im.RightDown.Longitude, im.RightDown.Latitude);

            var mwDAC = new MerchantWalletDAC();
            var mscDAC = new MerchantSupportCryptoDAC();
            var cryptoList = new CryptoComponent().GetList();

            return list.Select(item =>
            {
                List<int> supportCryptoIdList = new List<int>();
                if (item.AccountType == (byte)AccountType.Merchant)
                    supportCryptoIdList = mwDAC.SupportReceiptList(item.MerchantAccountId).Select(t => t.CryptoId).ToList();
                else
                    supportCryptoIdList = mscDAC.GetList(item.Id).Select(t => t.CryptoId).ToList();

                return new GetMerchantInfoOM()
                {
                    Address = item.Address,
                    IsAllowExpense = item.IsAllowExpense,
                    AvailableCryptoIconList = cryptoList.Where(c => supportCryptoIdList.Contains(c.Id)).Select(t => t.IconURL.ToString()).ToList(),
                    Distance = item.Distance.ToSpecificDecimal(2).ToString(CultureInfo.InvariantCulture),
                    OriginIconId = item.FileId,
                    CompressIconId = item.ThumbnailId ?? Guid.Empty,
                    Id = item.MerchantInformationId,
                    Location = new Location() { Longitude = item.Lng, Latitude = item.Lat },
                    Tags = item.Tags.Split(',').ToList(),
                    Title = item.MerchantName
                };
            }).ToList();
        }

        public List<GetMerchantInfoOM> GetMerchantInfoList(GetMerchantInfoListByDistanceIM im)
        {
            var list = new MerchantInformationDAC().QueryMerchantListByCountryId(im.CurrentPlace.Latitude, im.CurrentPlace.Longitude, im.Distance);

            var mwDAC = new MerchantWalletDAC();
            var mscDAC = new MerchantSupportCryptoDAC();
            var cryptoList = new CryptoComponent().GetList();

            return list.Select(item =>
            {
                List<int> supportCryptoIdList = new List<int>();
                if (item.AccountType == (byte)AccountType.Merchant)
                    supportCryptoIdList = mwDAC.SupportReceiptList(item.MerchantAccountId).Select(t => t.CryptoId).ToList();
                else
                    supportCryptoIdList = mscDAC.GetList(item.Id).Select(t => t.CryptoId).ToList();

                return new GetMerchantInfoOM()
                {
                    Address = item.Address,
                    AccountType = item.AccountType,
                    IsAllowExpense = item.IsAllowExpense,
                    AvailableCryptoIconList = cryptoList.Where(c => supportCryptoIdList.Contains(c.Id)).Select(t => t.IconURL.ToString()).ToList(),
                    Distance = item.Distance.ToSpecificDecimal(2).ToString(CultureInfo.InvariantCulture),
                    OriginIconId = item.FileId,
                    CompressIconId = item.ThumbnailId ?? Guid.Empty,
                    Id = item.MerchantInformationId,
                    Location = new Location() { Longitude = item.Lng, Latitude = item.Lat },
                    Tags = item.Tags.Split(',').ToList(),
                    Title = item.MerchantName
                };
            }).ToList();
        }

        public GetMerchantDetailOM GetMerchantDetail(GetMerchantDetailIM im)
        {
            var merchantDetail = new MerchantInformationDAC().GetMerchantDetailById(im.Id);
            if (merchantDetail == null)
            {
                Error($"{nameof(GetMerchantDetail)} 查询商家信息出错 id----{Newtonsoft.Json.JsonConvert.SerializeObject(im)}");
                throw new Exception("系统查询错误");
            }

            if (merchantDetail.Status != Status.Enabled || merchantDetail.IsPublic != Status.Enabled)
            {
                throw new CommonException(ReasonCode.MERCHANT_NOT_PUBLIC, MessageResources.MerchantNotPublic);
            }

            if (merchantDetail.AccountType == Entities.Enums.AccountType.User)
                return GetFiiiPayMerchantDetail(merchantDetail);
            return GetFiiiPosMerchantDetail(merchantDetail);
        }

        private GetMerchantDetailOM GetFiiiPayMerchantDetail(MerchantInformation merchantDetail)
        {
            var weekArray = Enum.GetValues(typeof(Week));

            var weekList = new List<int>();

            foreach (var item in weekArray)
            {
                if (merchantDetail.Week.HasFlag((Enum)item))
                {
                    weekList.Add((int)Math.Log((int)item, 2) + 1);
                }
            }

            var supportCurrencyList = new MerchantSupportCryptoDAC().GetList(merchantDetail.Id);
            var supportCryptoIdList = supportCurrencyList.Select(t => t.CryptoId).ToList();
            var cryptoList = new CryptocurrencyDAC().GetAll();
            var imageList = new MerchantOwnersFigureDAC().GetOwnersFiguresById(merchantDetail.Id);
            var recommendList = new MerchantRecommendDAC().GetRecommendsById(merchantDetail.Id);

            var result = new GetMerchantDetailOM()
            {
                AccountType = (int)AccountType.User,
                IsAllowExpense = merchantDetail.IsAllowExpense,
                AvailableCryptoCodeList = supportCurrencyList.Select(item => item.CryptoCode).ToList(),
                AvailableCryptoIconList = cryptoList.Where(c => supportCryptoIdList.Contains(c.Id)).Select(t => t.IconURL.ToString()).ToList(),
                Address = merchantDetail.Address,
                BussinessHour = merchantDetail.WeekTxt,
                ImageUrls = imageList.Select(item => item.FileId).ToList(),
                Location = new Location() { Longitude = merchantDetail.Lng, Latitude = merchantDetail.Lat },
                Introduce = merchantDetail.Introduce,
                RecommendGoods = recommendList.Select(item => new Goods() { Content = item.RecommendContent, Image = new ImageWithCompressImage() { Compress = item.ThumbnailId, Origin = item.RecommendPicture } }).ToList(),
                Tags = merchantDetail.Tags.Split(','),
                Title = merchantDetail.MerchantName,
                Phone = merchantDetail.Phone,
                PhoneCode = merchantDetail.PhoneCode
            };
            return result;
        }

        private GetMerchantDetailOM GetFiiiPosMerchantDetail(MerchantInformation merchantDetail)
        {
            var weekArray = Enum.GetValues(typeof(Week));

            var weekList = new List<int>();

            foreach (var item in weekArray)
            {
                if (merchantDetail.Week.HasFlag((Enum)item))
                {
                    weekList.Add((int)Math.Log((int)item, 2) + 1);
                }
            }

            var currencyList = new MerchantWalletDAC().SupportReceiptList(merchantDetail.MerchantAccountId);
            var cryptoDict = new CryptocurrencyDAC().GetAll().ToDictionary(item => item.Code);
            var imageList = new MerchantOwnersFigureDAC().GetOwnersFiguresById(merchantDetail.Id);
            var recommendList = new MerchantRecommendDAC().GetRecommendsById(merchantDetail.Id);

            var result = new GetMerchantDetailOM()
            {
                AccountType = (int)Entities.Enums.AccountType.Merchant,
                IsAllowExpense = merchantDetail.IsAllowExpense,
                AvailableCryptoCodeList = currencyList.Select(item => item.CryptoCode).ToList(),
                AvailableCryptoIconList = currencyList.Select(item =>
                {
                    if (cryptoDict.ContainsKey(item.CryptoCode))
                    {
                        return cryptoDict[item.CryptoCode].IconURL.ToString();
                    }
                    return string.Empty;
                }).Where(item => !string.IsNullOrEmpty(item)).ToList(),
                Address = merchantDetail.Address,
                EndDate = merchantDetail.EndTime,
                StartDate = merchantDetail.StartTime,
                BussinessHour = merchantDetail.WeekTxt,
                ImageUrls = imageList.Select(item => item.FileId).ToList(),
                Location = new Location() { Longitude = merchantDetail.Lng, Latitude = merchantDetail.Lat },
                Introduce = merchantDetail.Introduce,
                RecommendGoods = recommendList.Select(item => new Goods() { Content = item.RecommendContent, Image = new ImageWithCompressImage() { Compress = item.ThumbnailId, Origin = item.RecommendPicture } }).ToList(),
                Tags = merchantDetail.Tags.Split(','),
                Title = merchantDetail.MerchantName,
                Phone = merchantDetail.Phone,
                PhoneCode = merchantDetail.PhoneCode,
                Weeks = weekList
            };
            return result;
        }

        public List<MerchantStoreTypeOM> GetStoreTypeList()
        {
            return new StoreTypeDAC().GetList()
                .Select(item => new MerchantStoreTypeOM() { Id = item.Id, Name_CN = item.Name_CN, Name_EN = item.Name_EN }).ToList();
        }

        public async Task<string> GetMerchantInviteCode(Guid accountId)
        {
            var existInviteRecord = await new InviteRecordDAC().GetDetailByAccountIdAsync(accountId, InviteType.FiiipayMerchant);
            return existInviteRecord?.InviterCode;
        }

        public async Task<bool> FiiipayMerchantCreateAsync(UserAccount account, FiiiPayMerchantInfoCreateIM model)
        {
            var uaDAC = new UserAccountDAC();
            if (account.L1VerifyStatus != VerifyStatus.Certified)
                throw new CommonException(ReasonCode.NOT_VERIFY_LV1, Resources.EMNeedLV1Verfied);

            var inviteAccount = uaDAC.GetByInvitationCode(model.InviteCode);
            bool needInviteRecord = true;
            if (!string.IsNullOrEmpty(model.InviteCode))
            {
                if (inviteAccount == null)
                    throw new CommonException(ReasonCode.INVITORCODE_NOT_EXISTS, MessageResources.InvalidInvitation);
                if (inviteAccount.InvitationCode == account.InvitationCode)
                    throw new CommonException(ReasonCode.INVALID_INVITECODE, MessageResources.InviteCodeCanotInputSelf);

                var existInviteRecord = await new InviteRecordDAC().GetDetailByAccountIdAsync(account.Id, InviteType.FiiipayMerchant);
                if (existInviteRecord != null)
                    needInviteRecord = false;
            }
            else
                needInviteRecord = false;


            var merchantCountry = new CountryDAC().GetById(model.CountryId);
            if (merchantCountry == null)
            {
                throw new CommonException(ReasonCode.RECORD_NOT_EXIST, MessageResources.InvalidParameters);
            }
            if (!merchantCountry.IsSupportStore)
                throw new CommonException(ReasonCode.RECORD_NOT_EXIST, MessageResources.InvalidParameters);

            Regions stateRegion;
            Regions cityRegion;

            if (model.StateId.HasValue)
            {
                stateRegion = await new RegionDAC().GetByIdAsync(model.StateId.Value);
                if (stateRegion == null)
                    throw new CommonException(ReasonCode.RECORD_NOT_EXIST, MessageResources.InvalidParameters);
                if (stateRegion.CountryId != model.CountryId)
                    throw new CommonException(ReasonCode.RECORD_NOT_EXIST, MessageResources.InvalidParameters);

                if (model.CityId.HasValue)
                {
                    cityRegion = await new RegionDAC().GetByIdAsync(model.CityId.Value);
                    if (cityRegion == null)
                        throw new CommonException(ReasonCode.RECORD_NOT_EXIST, MessageResources.InvalidParameters);
                    if (cityRegion.CountryId != model.CountryId || cityRegion.ParentId != stateRegion.Id)
                        throw new CommonException(ReasonCode.RECORD_NOT_EXIST, MessageResources.InvalidParameters);
                }
            }

            #region entities
            Guid merchantInfoId = Guid.NewGuid();
            DateTime dtNow = DateTime.UtcNow;
            
            MerchantInformation merchantInfo = new MerchantInformation
            {
                Id = merchantInfoId,
                CreateTime = dtNow,
                FromType = InputFromType.UserInput,
                MerchantName = model.MerchantName,
                WeekTxt = model.WeekTxt,
                Tags = model.TagList == null ? "" : string.Join(",", model.TagList),
                Introduce = model.Introduce,
                CountryId = merchantCountry.Id,
                StateId = model.StateId,
                CityId = model.CityId,
                PhoneCode = account.PhoneCode,
                Address = model.Address,
                Lng = model.Lng,
                Lat = model.Lat,
                Status = Status.Stop,
                VerifyStatus = VerifyStatus.UnderApproval,
                MerchantAccountId = account.Id,
                Phone = model.Phone,
                IsPublic = Status.Stop,
                FileId = model.StorefrontImg[0],
                ThumbnailId = model.StorefrontImg[1],
                AccountType = AccountType.User,
                Markup = 0,
                FeeRate = 0,
                IsAllowExpense = false,
                Week = Week.Monday,
                ApplicantName = model.ApplicantName,
                UseFiiiDeduct = model.UseFiiiDeduction
            };
            InviteRecord inviteRecord = new InviteRecord
            {
                AccountId = account.Id,
                InviterCode = model.InviteCode,
                Type = InviteType.FiiipayMerchant,
                InviterAccountId = inviteAccount?.Id ?? Guid.Empty,
                Timestamp = dtNow
            };
            FiiipayMerchantVerifyRecord record = new FiiipayMerchantVerifyRecord
            {
                CreateTime = dtNow,
                MerchantInfoId = merchantInfoId,
                BusinessLicenseImage = model.BusinessLicenseImage,
                LicenseNo = model.LicenseNo,
                VerifyStatus = VerifyStatus.UnderApproval
            };

            List<MerchantCategory> categorys = model.MerchantCategorys.Select(t => new MerchantCategory
            {
                MerchantInformationId = merchantInfoId,
                Category = t
            }).ToList();

            List<MerchantOwnersFigure> figuresList = new List<MerchantOwnersFigure>();
            if (model.FigureImgList != null)
            {
                for (int i = 0; i < model.FigureImgList.Length; i++)
                {
                    figuresList.Add(new MerchantOwnersFigure
                    {
                        MerchantInformationId = merchantInfoId,
                        FileId = model.FigureImgList[i][0],
                        Sort = i,
                        ThumbnailId = model.FigureImgList[i][1]
                    });
                }
            }

            var coinList = await new CryptocurrencyDAC().GetAllAsync();

            List<MerchantSupportCrypto> supportCryptoList = model.SupportCoins.Select(t =>
            {
                var c = coinList.Find(m => m.Id == t);
                if (c != null)
                {
                    return new MerchantSupportCrypto
                    {
                        MerchantInfoId = merchantInfoId,
                        CryptoId = t,
                        CryptoCode = c.Code
                    };
                }

                return null;
            }).Where(x=>x != null).ToList();

            #endregion

            var mInfoDAC = new MerchantInformationDAC();
            var fmVerfiyRecordDAC = new FiiipayMerchantVerifyRecordDAC();
            var mCategoryDAC = new MerchantCategoryDAC();
            var mFigureDAC = new MerchantOwnersFigureDAC();
            var mSupportCryptoDAC = new MerchantSupportCryptoDAC();
            var inviteDAC = new InviteRecordDAC();

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 0, 1, 30)))
            {
                mInfoDAC.Insert(merchantInfo);
                fmVerfiyRecordDAC.Insert(record);
                if (needInviteRecord)
                {
                    inviteDAC.Insert(inviteRecord);
                }

                if (categorys != null && categorys.Count > 0)
                {
                    foreach (var item in categorys)
                    {
                        mCategoryDAC.Insert(item);
                    }
                }
                if (figuresList != null && figuresList.Count > 0)
                {
                    foreach (var item in figuresList)
                    {
                        mFigureDAC.Insert(item);
                    }
                }
                if (supportCryptoList != null && supportCryptoList.Count > 0)
                {
                    foreach (var item in supportCryptoList)
                    {
                        mSupportCryptoDAC.Insert(item);
                    }
                }

                scope.Complete();
            }

            return true;
        }
    }
}
