using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Utils;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Entities;
using FiiiPay.Framework.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class FiiiPayMerchantBLL : BaseBLL
    {
        public List<FiiipayMerchantInfoListModel> GetMerchantPager(string fiiipayAccount, string merchantName, int? countryId, byte? merchantStatus, byte? verifyStatus, ref GridPager pager)
        {
            string sql = "SELECT a.CreateTime, a.Id,b.Cellphone FiiiPayAccount,a.MerchantName,a.CountryId,a.VerifyStatus,a.Status,a.IsAllowExpense,FromType FROM dbo.MerchantInformations a LEFT JOIN dbo.UserAccounts b on a.MerchantAccountId=b.Id WHERE AccountType=" + (int)Entities.Enums.AccountType.User;
            List<SqlSugar.SugarParameter> paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(fiiipayAccount))
            {
                sql += " AND b.[Cellphone]=@Cellphone";
                paramList.Add(new SqlSugar.SugarParameter("@Cellphone", fiiipayAccount));
            }
            if (!string.IsNullOrEmpty(merchantName))
            {
                sql += " AND a.[MerchantName] LIKE @MerchantName";
                paramList.Add(new SqlSugar.SugarParameter("@MerchantName", "%" + merchantName + "%"));
            }
            if (countryId.HasValue)
            {
                sql += " AND a.[CountryId]=@CountryId";
                paramList.Add(new SqlSugar.SugarParameter("@CountryId", countryId.Value));
            }
            if (merchantStatus.HasValue)
            {
                sql += " AND a.[Status]=@Status";
                paramList.Add(new SqlSugar.SugarParameter("@Status", merchantStatus.Value));
            }
            if (verifyStatus.HasValue)
            {
                sql += " AND a.[VerifyStatus]=@VerifyStatus";
                paramList.Add(new SqlSugar.SugarParameter("@VerifyStatus", verifyStatus.Value));
            }
            var data = QueryPager.Query<FiiipayMerchantInfoListModel>(FiiiPayDB.DB, sql, ref pager, paramList);
            return data;
        }

        public SaveResult SaveSetting(Guid id, Status status, bool allowExpense,decimal feeRate,decimal markup)
        {
            FiiiPayDB.DB.Updateable<MerchantInformations>().SetColumns(t => new MerchantInformations
            {
                Status = status,
                IsAllowExpense = allowExpense,
                FeeRate = feeRate,
                Markup = markup
            }).Where(m => m.Id == id).ExecuteCommand();
            return new SaveResult(true);
        }

        public SaveResult SaveAdd(string boUsername, MerchantEditInfoModel model,string inviteCode)
        {
            var fiiipayAccount = FiiiPayDB.UserAccountDb.GetSingle(t => t.CountryId == model.CountryId && t.Cellphone == model.FiiiPayAccount);
            if (fiiipayAccount == null)
                return new SaveResult(false, "FiiiPay用户不存在");
            if (fiiipayAccount.Status != (byte)Entities.Enums.AccountStatus.Active)
                return new SaveResult(false, "FiiiPay用户异常");
            if (fiiipayAccount.L1VerifyStatus!= Entities.Enums.VerifyStatus.Certified)
                return new SaveResult(false, "该账号未通过KYC LV1认证");

            UserAccounts inviteAccount = null;
            bool needInsertInviteRecord = false;
            if (!string.IsNullOrEmpty(inviteCode))
            {
                inviteAccount = FiiiPayDB.UserAccountDb.GetSingle(t => t.InvitationCode == inviteCode);
                if (inviteAccount == null)
                    return new SaveResult(false, "无效的邀请码");
                if (inviteAccount.InvitationCode == fiiipayAccount.InvitationCode)
                    return new SaveResult(false, "不能填写商家自己的邀请码");
                needInsertInviteRecord = true;
            }

            var existRecord = FiiiPayDB.DB.Queryable<InviteRecords>().First(t => t.AccountId == fiiipayAccount.Id && t.Type == InviteType.FiiipayMerchant);
            if (existRecord == null)
            {
                needInsertInviteRecord = needInsertInviteRecord && true;
            }
            else
            {
                if(existRecord.InviterCode != inviteCode)
                {
                    return new SaveResult(false, "已经设置了其他账号为邀请人");
                }
                needInsertInviteRecord = false;
            }

            var country = FoundationDB.CountryDb.GetSingle(t => t.Code == model.CountryCode);
            if (country == null)
                return new SaveResult(false, "无效的国家");

            #region entities
            Guid merchantInfoId = Guid.NewGuid();
            DateTime dtNow = DateTime.UtcNow;
            MerchantInformations merchantInfo = new MerchantInformations
            {
                Id = merchantInfoId,
                CreateTime = dtNow,
                FromType = InputFromType.BOInput,
                MerchantName = model.MerchantName,
                WeekTxt = model.WeekTxt,
                Tags = model.TagList == null ? "" : string.Join(",", model.TagList),
                Introduce = model.Introduce,
                CountryId = country.Id,
                PhoneCode = country.PhoneCode,
                Address = model.Address,
                Lng = model.Lng,
                Lat = model.Lat,
                Status = Status.Enabled,
                VerifyStatus = Entities.Enums.VerifyStatus.Certified,
                MerchantAccountId = fiiipayAccount.Id,
                Phone = model.Phone,
                IsPublic = Status.Enabled,
                FileId = model.FileId,
                ThumbnailId = model.FileId,//new BlobBLL().UploadWithCompress(model.FileId),
                AccountType = Entities.Enums.AccountType.User,
                Markup = 0,
                FeeRate = 0,
                IsAllowExpense = true,
                Week = Week.Monday,
                ApplicantName = model.ApplicantName
            };
            InviteRecords inviteRecord = new InviteRecords
            {
                AccountId = fiiipayAccount.Id,
                InviterCode = inviteCode,
                Type = InviteType.FiiipayMerchant,
                InviterAccountId = inviteAccount?.Id ?? Guid.Empty,
                Timestamp = dtNow
            };
            FiiipayMerchantProfiles profile = new FiiipayMerchantProfiles
            {
                MerchantInfoId = merchantInfoId,
                BusinessLicenseImage = model.BusinessLicenseImage,
                LicenseNo = model.LicenseNo
            };
            FiiipayMerchantVerifyRecords record = new FiiipayMerchantVerifyRecords
            {
                CreateTime = dtNow,
                MerchantInfoId = merchantInfoId,
                BusinessLicenseImage = model.BusinessLicenseImage,
                LicenseNo = model.LicenseNo,
                VerifyStatus = Entities.Enums.VerifyStatus.Certified,
                VerifyTime = dtNow,
                Auditor = boUsername
            };

            List<MerchantCategorys> categorys = model.MerchantCategorys.Select(t => new MerchantCategorys
            {
                MerchantInformationId = merchantInfoId,
                Category = t
            }).ToList();

            List<MerchantOwnersFigures> figuresList = new List<MerchantOwnersFigures>();
            if (model.FigureImgIdList != null)
            {
                var blobBLL = new BlobBLL();
                for (int i = 0; i < model.FigureImgIdList.Length; i++)
                {
                    figuresList.Add(new MerchantOwnersFigures
                    {
                        MerchantInformationId = merchantInfoId,
                        FileId = model.FigureImgIdList[i],
                        Sort = i,
                        ThumbnailId = model.FigureImgIdList[i]//blobBLL.UploadWithCompress(model.FigureImgIdList[i])
                    });
                }
            }

            var coinList = FoundationDB.CryptocurrencyDb.GetList();
            List<MerchantSupportCryptos> supportCryptoList = model.SupportCoins.Select(t => new MerchantSupportCryptos
            {
                MerchantInfoId = merchantInfoId,
                CryptoId = t,
                CryptoCode = coinList == null ? "" : coinList.Find(c => c.Id == t).Code
            }).ToList();
            #endregion

            var sr = FiiiPayDB.DB.Ado.UseTran(() =>
            {
                FiiiPayDB.MerchantInformationDb.Insert(merchantInfo);
                FiiiPayDB.FiiipayMerchantProfileDb.Insert(profile);
                FiiiPayDB.FiiipayMerchantVerifyRecordDb.Insert(record);
                if (needInsertInviteRecord)
                    FiiiPayDB.InviteRecordDb.Insert(inviteRecord);
                if (categorys.Count > 0)
                    FiiiPayDB.MerchantCategoryDb.InsertRange(categorys);
                if(figuresList.Count>0)
                    FiiiPayDB.MerchantOwnersFigureDb.InsertRange(figuresList);
                if (supportCryptoList != null && supportCryptoList.Count > 0)
                    FiiiPayDB.MerchantSupportCryptoDb.InsertRange(supportCryptoList);
            });
            return new SaveResult(sr.IsSuccess, sr.ErrorMessage);
        }
        
        public SaveResult SaveEdit(string boAccountName, MerchantEditInfoModel model)
        {
            MerchantInformations originMerchantInfo = FiiiPayDB.MerchantInformationDb.GetById(model.Id);
            if (originMerchantInfo == null)
                return new SaveResult(false);
            var country = FoundationDB.CountryDb.GetSingle(t => t.Code == model.CountryCode);
            if (country == null)
                return new SaveResult(false, "无效的国家");

            DateTime dtNow = DateTime.UtcNow;
            #region entities set value
            originMerchantInfo.LastModifyBy = boAccountName;
            originMerchantInfo.LastModifyTime = dtNow;
            originMerchantInfo.MerchantName = model.MerchantName;
            originMerchantInfo.WeekTxt = model.WeekTxt;
            originMerchantInfo.Tags = model.TagList == null ? "" : string.Join(",", model.TagList);
            originMerchantInfo.Introduce = model.Introduce;
            originMerchantInfo.CountryId = country.Id;
            originMerchantInfo.PhoneCode = country.PhoneCode;
            originMerchantInfo.Address = model.Address;
            originMerchantInfo.Lng = model.Lng;
            originMerchantInfo.Lat = model.Lat;
            originMerchantInfo.Phone = model.Phone;
            if (originMerchantInfo.FileId != model.FileId)
            {
                originMerchantInfo.FileId = model.FileId;
                originMerchantInfo.ThumbnailId = new BlobBLL().UploadWithCompress(model.FileId);
            }
            originMerchantInfo.ApplicantName = model.ApplicantName;

            bool profileChanged = false;
            var originProfile = FiiiPayDB.FiiipayMerchantProfileDb.GetSingle(t => t.MerchantInfoId == originMerchantInfo.Id);
            if (originMerchantInfo.VerifyStatus != Entities.Enums.VerifyStatus.Certified)
            {
                profileChanged = profileChanged || originProfile.BusinessLicenseImage != model.BusinessLicenseImage;
                profileChanged = profileChanged || originProfile.LicenseNo != model.LicenseNo;

                originProfile.LicenseNo = model.LicenseNo;
                originProfile.BusinessLicenseImage = model.BusinessLicenseImage;
            }

            FiiipayMerchantVerifyRecords record = new FiiipayMerchantVerifyRecords
            {
                CreateTime = dtNow,
                MerchantInfoId = originMerchantInfo.Id,
                BusinessLicenseImage = model.BusinessLicenseImage,
                LicenseNo = model.LicenseNo,
                VerifyStatus = Entities.Enums.VerifyStatus.Certified,
                VerifyTime = dtNow,
                Auditor = boAccountName
            };

            List<MerchantCategorys> categorys = model.MerchantCategorys.Select(t => new MerchantCategorys
            {
                MerchantInformationId = originMerchantInfo.Id,
                Category = t
            }).ToList();

            var coinList = FoundationDB.CryptocurrencyDb.GetList();
            List<MerchantSupportCryptos> supportCryptoList = model.SupportCoins.Select(t => new MerchantSupportCryptos
            {
                MerchantInfoId = originMerchantInfo.Id,
                CryptoId = t,
                CryptoCode = coinList == null ? "" : coinList.Find(c => c.Id == t).Code
            }).ToList();

            //下面写这么多，就是为了尽可能少调图片接口
            List<MerchantOwnersFigures> toAddFiguresList = new List<MerchantOwnersFigures>();
            List<MerchantOwnersFigures> toChangeSortFiguresList = new List<MerchantOwnersFigures>();
            List<Guid> toDeleteFiguresIdList = new List<Guid>();
            var originFigureList = FiiiPayDB.MerchantOwnersFigureDb.GetList(t => t.MerchantInformationId == originMerchantInfo.Id);
            
            if (model.FigureImgIdList != null)
            {
                var blobBLL = new BlobBLL();
                for (int i = 0; i < model.FigureImgIdList.Length; i++)
                {
                    var figureItem = originFigureList == null ? null : originFigureList.Find(t => t.FileId == model.FigureImgIdList[i]);
                    if (figureItem == null)
                    {
                        toAddFiguresList.Add(new MerchantOwnersFigures
                        {
                            MerchantInformationId = originMerchantInfo.Id,
                            FileId = model.FigureImgIdList[i],
                            Sort = i,
                            ThumbnailId = blobBLL.UploadWithCompress(model.FigureImgIdList[i])
                        });
                    }
                    else
                    {
                        figureItem.Sort = i;
                        toChangeSortFiguresList.Add(figureItem);
                    }
                }
            }
            if (originFigureList != null)
            {
                foreach (var item in originFigureList)
                {
                    if (!toChangeSortFiguresList.Any(t => t.FileId == item.FileId))
                    {
                        toDeleteFiguresIdList.Add(item.FileId);
                    }
                }
            }
            #endregion

            var sr = FiiiPayDB.DB.Ado.UseTran(() =>
            {
                FiiiPayDB.MerchantInformationDb.Update(originMerchantInfo);
                if (profileChanged)
                {
                    FiiiPayDB.FiiipayMerchantProfileDb.Update(originProfile);
                    FiiiPayDB.FiiipayMerchantVerifyRecordDb.Insert(record);
                }
                FiiiPayDB.MerchantCategoryDb.Delete(t => t.MerchantInformationId == originMerchantInfo.Id);
                FiiiPayDB.MerchantCategoryDb.InsertRange(categorys);

                FiiiPayDB.MerchantSupportCryptoDb.Delete(t => t.MerchantInfoId == originMerchantInfo.Id);
                FiiiPayDB.MerchantSupportCryptoDb.InsertRange(supportCryptoList);

                if (toDeleteFiguresIdList != null && toDeleteFiguresIdList.Count > 0)
                {
                    FiiiPayDB.MerchantOwnersFigureDb.Delete(t => t.MerchantInformationId == originMerchantInfo.Id && toDeleteFiguresIdList.Contains(t.FileId));
                }
                if (toAddFiguresList != null && toAddFiguresList.Count > 0)
                {
                    FiiiPayDB.MerchantOwnersFigureDb.InsertRange(toAddFiguresList);
                }
                if (toChangeSortFiguresList != null && toChangeSortFiguresList.Count > 0)
                {
                    FiiiPayDB.MerchantOwnersFigureDb.UpdateRange(toChangeSortFiguresList);
                }
            });

            return new SaveResult(sr.IsSuccess, sr.ErrorMessage);
        }

        public SaveResult SaveVerify(Guid id, byte verifyResult, string verifyReason, string boUsername)
        {
            var info = FiiiPayDB.MerchantInformationDb.GetById(id);
            if (info.VerifyStatus != Entities.Enums.VerifyStatus.UnderApproval)
                return new SaveResult(false, "商家审核状态错误");
            if (verifyResult == (byte)Entities.Enums.VerifyStatus.Disapproval && string.IsNullOrEmpty(verifyReason))
                return new SaveResult(false, "没有填写原因");

            var verifyRecord = FiiiPayDB.FiiipayMerchantVerifyRecordDb.GetSingle(t => t.MerchantInfoId == info.Id && t.VerifyStatus == Entities.Enums.VerifyStatus.UnderApproval);
            var profile = FiiiPayDB.FiiipayMerchantProfileDb.GetSingle(t => t.MerchantInfoId == info.Id);
            var existRecord = FiiiPayDB.InviteRecordDb.GetSingle(t => t.AccountId == info.MerchantAccountId && t.Type == InviteType.FiiipayMerchant);

            DateTime dtNow = DateTime.UtcNow;

            var sr = FiiiPayDB.DB.Ado.UseTran(() =>
            {
                if (verifyResult == (byte)Entities.Enums.VerifyStatus.Certified)
                {
                    FiiiPayDB.MerchantInformationDb.Update(t => new MerchantInformations
                    {
                        Status = Status.Enabled,
                        IsAllowExpense = true,
                        IsPublic = Status.Enabled,
                        VerifyStatus = (Entities.Enums.VerifyStatus)verifyResult,
                        VerifyDate = dtNow
                    }, w => w.Id == id);
                }
                else if(verifyResult == (byte)Entities.Enums.VerifyStatus.Disapproval)
                {
                    FiiiPayDB.MerchantInformationDb.Update(t => new MerchantInformations { VerifyStatus = (Entities.Enums.VerifyStatus)verifyResult, VerifyDate = dtNow }, w => w.Id == id);
                }
                
                if (verifyRecord != null)
                {
                    FiiiPayDB.FiiipayMerchantVerifyRecordDb.Update(t => new FiiipayMerchantVerifyRecords
                    {
                        VerifyStatus = (Entities.Enums.VerifyStatus)verifyResult,
                        VerifyTime = dtNow,
                        Message = verifyReason,
                        Auditor = boUsername
                    }, w => w.Id == verifyRecord.Id);
                }
                if(verifyResult== (byte)Entities.Enums.VerifyStatus.Certified)
                {
                    if (profile == null)
                    {
                        FiiiPayDB.FiiipayMerchantProfileDb.Insert(new FiiipayMerchantProfiles
                        {
                            MerchantInfoId = info.Id,
                            LicenseNo = verifyRecord.LicenseNo,
                            BusinessLicenseImage = verifyRecord.BusinessLicenseImage
                        });
                    }
                    else
                    {
                        FiiiPayDB.FiiipayMerchantProfileDb.Update(t => new FiiipayMerchantProfiles
                        {
                            LicenseNo = verifyRecord.LicenseNo,
                            BusinessLicenseImage = verifyRecord.BusinessLicenseImage
                        }, w => w.MerchantInfoId == info.Id);
                    }
                }
            });
            if (verifyRecord != null)
                RabbitMQSender.SendMessage("FiiipayMerchantProfileVerified", new { AccountId = info.MerchantAccountId, VerifyRecordId = verifyRecord.Id, VerifyResult = verifyResult });

            return new SaveResult(sr.IsSuccess, sr.ErrorMessage);
        }
    }
}