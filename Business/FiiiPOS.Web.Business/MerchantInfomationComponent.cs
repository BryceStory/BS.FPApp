using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FiiiPOS.Web.Business
{
    public class MerchantInformationComponent
    {
        /// <summary>
        /// 新增门店信息
        /// </summary>
        /// <param name="information"></param>
        /// <param name="ownersFigures"></param>
        /// <param name="merchantRecommends"></param>
        /// <param name="merchantCategories"></param>
        /// <param name="phone"></param>
        public void InsertMerchantInformation(MerchantInformation information, List<MerchantOwnersFigure> ownersFigures, List<MerchantRecommend> merchantRecommends, List<MerchantCategory> merchantCategories)
        {
            if (new MerchantAccountDAC().GetById(information.MerchantAccountId) == null)
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.ACCOUNT_NOT_EXISTS, "商家账户未注册或不存在");
            }
            if (new MerchantInformationDAC().GetByMerchantAccountId(information.MerchantAccountId) != null)
            {
                throw new CommonException(ReasonCode.MERCAHNT_BINDING, "该商家已绑定门店");
            }
            Task.WhenAll(Task.Run(() => Parallel.ForEach(merchantRecommends, item =>
            {
                var tumnailId = new MasterImageAgent().UploadWithCompress(item.RecommendPicture);
                item.ThumbnailId = tumnailId == Guid.Empty ? item.RecommendPicture : tumnailId;
            })), Task.Run(() => Parallel.ForEach(ownersFigures, item =>
            {
                var tumnailId = new MasterImageAgent().UploadWithCompress(item.FileId);
                item.ThumbnailId = tumnailId == Guid.Empty ? item.FileId : tumnailId;
            }))).Wait();

            using (var scope = new TransactionScope())
            {
                new MerchantInformationDAC().Insert(information);
                ownersFigures.ForEach(item => new MerchantOwnersFigureDAC().Insert(item));
                merchantRecommends.ForEach(item => new MerchantRecommendDAC().Insert(item));
                merchantCategories.ForEach(item => new MerchantCategoryDAC().Insert(item));
                //new MerchantAccountDAC().UpdateCellphone(information.MerchantAccountId, phone);
                //new MerchantAccountDAC().UpdateMerchantName(information.MerchantAccountId, information.MerchantName);
                scope.Complete();
            }
        }

        /// <summary>
        /// 查询所有门店信息
        /// </summary>
        /// <param name="account"></param>
        public MerchantInformationES SelectMerchantInformation(Guid accountId, int countryId)
        {
            var account = new MerchantAccountDAC().GetById(accountId);
            if (account == null)
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.ACCOUNT_NOT_EXISTS, "商家账户未注册或不存在");
            }
            var information = new MerchantInformationDAC().GetByMerchantAccountId(account.Id);
            var country = new CountryDAC().GetById(countryId);
            if (information == null)
            {
                throw new CommonException(ReasonCode.MERCHANT_NONE, "商家没有门店");
                //return new MerchantInformationES()
                //{
                //    Countrys = new Countrys()
                //    {
                //        Name_CN = country.Name_CN,
                //        Name = country.Name
                //    }
                //};
            }

            var figures = new MerchantOwnersFigureDAC().GetOwnersFiguresById(information.Id);
            var category = new MerchantCategoryDAC().GetByMerchantInformationId(information.Id);
            var recommend = new MerchantRecommendDAC().GetRecommendsById(information.Id);

            return new MerchantInformationES()
            {
                MerchantName = information.MerchantName,
                Categorys = category.Select(item => item.Category).ToList(),
                Week = information.Week,
                StartTime = information.StartTime,
                EndTime = information.EndTime,
                Tags = information.Tags,
                Phone = information.Phone,
                Introduce = information.Introduce,
                Address = information.Address,
                Lng = information.Lng,
                Lat = information.Lat,
                VerifyStatus = information.VerifyStatus,
                OwnersFigures = figures.Select(item => item.FileId).ToList(),
                Recommends = recommend.Select(item => new Recommend() { Content = item.RecommendContent, Picture = item.RecommendPicture }).ToList(),
                Countrys = new Countrys()
                {
                    Name_CN = country.Name_CN,
                    Name = country.Name
                },
                IsPublic = information.IsPublic
            };
        }

        public Guid GetInformationIdByAccount(Guid accountId)
        {
            return new MerchantInformationDAC().GetByMerchantAccountId(accountId).Id;
        }

        public Country CountrysQuerAll(int countryId)
        {
            return new CountryDAC().GetById(countryId);
        }

        /// <summary>
        /// 更新所有门店信息
        /// </summary>
        /// <param name="information"></param>
        /// <param name="ownersFigures"></param>
        /// <param name="merchantRecommends"></param>
        /// <param name="merchantCategories"></param>
        /// <param name="phone"></param>
        public void UpdateMerchantInformation(MerchantInformation information, List<MerchantOwnersFigure> ownersFigures, List<MerchantRecommend> merchantRecommends, List<MerchantCategory> merchantCategories, string phone)
        {
            if (new MerchantAccountDAC().GetById(information.MerchantAccountId) == null)
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.ACCOUNT_NOT_EXISTS, "商家账户未注册或不存在");
            }

            //var Categories = merchantCategories;

            var owners = new MerchantOwnersFigureDAC().GetOwnersFiguresById(information.Id); //查询商家主图id

            var recommends = new MerchantRecommendDAC().GetRecommendsById(information.Id); //查询商家推荐图id

            var merchantInfo = new MerchantInformationDAC().GetByMerchantAccountId(information.MerchantAccountId);

            //判断 商家名称，经营内容，商家介绍，门店相册是否一样
            var isSame = merchantInfo.MerchantName == information.MerchantName && merchantInfo.Tags == information.Tags && merchantInfo.Introduce == information.Introduce;

            isSame &= owners.Select(item => item.FileId).All(item => ownersFigures.ToDictionary(_ => _.FileId).ContainsKey(item)) && owners.Count == ownersFigures.Count;

            isSame &= recommends.Select(item => item.RecommendPicture).All(item => merchantRecommends.ToDictionary(_ => _.RecommendPicture).ContainsKey(item)) && recommends.Count == merchantRecommends.Count && recommends.Select(item => item.RecommendContent).All(item => merchantRecommends.ToDictionary(_ => _.RecommendContent).ContainsKey(item));


            if (merchantInfo.VerifyStatus == FiiiPay.Entities.Enums.VerifyStatus.Disapproval)
            {
                Task.WhenAll(Task.Run(() =>
                {
                    Parallel.ForEach(owners, item =>
                    {
                        try
                        {
                            if (!ownersFigures.Any(_ => _.FileId == item.FileId))
                            {
                                Task.WhenAll(Task.Run(() => { new MasterImageAgent().Delete(item.FileId.ToString()); }), Task.Run(() => { new MasterImageAgent().Delete(item.ThumbnailId.ToString()); }));
                            }
                        }
                        catch
                        {

                        }
                    });
                }), Task.Run(() =>
                {
                    Parallel.ForEach(recommends, item =>
                    {
                        if (!merchantRecommends.Any(_ => _.RecommendPicture == item.RecommendPicture))
                        {
                            Task.WhenAll(Task.Run(() => { new MasterImageAgent().Delete(item.RecommendPicture.ToString()); }), Task.Run(() => { new MasterImageAgent().Delete(item.ThumbnailId.ToString()); }));
                        }
                    });
                }), Task.Run(() =>
                {
                    Parallel.ForEach(ownersFigures, item =>
                    {
                        if (!owners.Any(_ => _.FileId == item.FileId))
                        {
                            var tumnailId = new MasterImageAgent().UploadWithCompress(item.FileId);
                            item.ThumbnailId = tumnailId == Guid.Empty ? item.FileId : tumnailId;
                        }
                        else
                        {
                            item.ThumbnailId = owners.FirstOrDefault(_ => _.FileId == item.FileId)?.ThumbnailId ?? item.FileId;
                        }
                    });
                }),
               Task.Run(() =>
               {
                   Parallel.ForEach(merchantRecommends, item =>
                   {
                       if (!recommends.Any(_ => _.RecommendPicture == item.RecommendPicture))
                       {
                           var tumnailId = new MasterImageAgent().UploadWithCompress(item.RecommendPicture);
                           item.ThumbnailId = tumnailId == Guid.Empty ? item.RecommendPicture : tumnailId;
                       }
                       else
                       {
                           item.ThumbnailId = recommends.FirstOrDefault(_ => _.RecommendPicture == item.RecommendPicture)?.ThumbnailId ?? item.RecommendPicture;
                       }
                   });
               })).Wait();
                using (var scope = new TransactionScope())
                {
                    new MerchantInformationDAC().Update(information.Id, information);

                    new MerchantCategoryDAC().Delete(information.Id);

                    merchantCategories.ForEach(item => new MerchantCategoryDAC().Insert(item));

                    new MerchantOwnersFigureDAC().Delete(information.Id);

                    ownersFigures.ForEach(item => new MerchantOwnersFigureDAC().Insert(item));

                    new MerchantRecommendDAC().Delete(information.Id);

                    merchantRecommends.ForEach(item => new MerchantRecommendDAC().Insert(item));

                    scope.Complete();
                }
            }

            else if (merchantInfo.VerifyStatus == FiiiPay.Entities.Enums.VerifyStatus.Certified)
            {

            if (isSame)
            {
                using (var scope = new TransactionScope())
                {
                    new MerchantInformationDAC().UpdatePartialInformation(information.Id, information);

                    new MerchantCategoryDAC().Delete(information.Id);

                    merchantCategories.ForEach(item => new MerchantCategoryDAC().Insert(item));

                    scope.Complete();
                }
            }
            else
            {
                Task.WhenAll(Task.Run(() =>
                {
                    Parallel.ForEach(owners, item =>
                    {
                        try
                        {
                            if (!ownersFigures.Any(_ => _.FileId == item.FileId))
                            {
                                Task.WhenAll(Task.Run(() => { new MasterImageAgent().Delete(item.FileId.ToString()); }), Task.Run(() => { new MasterImageAgent().Delete(item.ThumbnailId.ToString()); }));
                            }
                        }
                        catch
                        {

                        }
                    });
                }), Task.Run(() =>
                {
                    Parallel.ForEach(recommends, item =>
                    {
                        if (!merchantRecommends.Any(_ => _.RecommendPicture == item.RecommendPicture))
                        {
                            Task.WhenAll(Task.Run(() => { new MasterImageAgent().Delete(item.RecommendPicture.ToString()); }), Task.Run(() => { new MasterImageAgent().Delete(item.ThumbnailId.ToString()); }));
                        }
                    });
                }), Task.Run(() =>
                {
                    Parallel.ForEach(ownersFigures, item =>
                    {
                        if (!owners.Any(_ => _.FileId == item.FileId))
                        {
                            var tumnailId = new MasterImageAgent().UploadWithCompress(item.FileId);
                            item.ThumbnailId = tumnailId == Guid.Empty ? item.FileId : tumnailId;
                        }
                        else
                        {
                            item.ThumbnailId = owners.FirstOrDefault(_ => _.FileId == item.FileId)?.ThumbnailId ?? item.FileId;
                        }
                    });
                }),
                Task.Run(() =>
                {
                    Parallel.ForEach(merchantRecommends, item =>
                    {
                        if (!recommends.Any(_ => _.RecommendPicture == item.RecommendPicture))
                        {
                            var tumnailId = new MasterImageAgent().UploadWithCompress(item.RecommendPicture);
                            item.ThumbnailId = tumnailId == Guid.Empty ? item.RecommendPicture : tumnailId;
                        }
                        else
                        {
                            item.ThumbnailId = recommends.FirstOrDefault(_ => _.RecommendPicture == item.RecommendPicture)?.ThumbnailId ?? item.RecommendPicture;
                        }
                    });
                })).Wait();
                using (var scope = new TransactionScope())
                {
                    new MerchantInformationDAC().Update(information.Id, information);

                    new MerchantCategoryDAC().Delete(information.Id);

                    merchantCategories.ForEach(item => new MerchantCategoryDAC().Insert(item));

                    new MerchantOwnersFigureDAC().Delete(information.Id);

                    ownersFigures.ForEach(item => new MerchantOwnersFigureDAC().Insert(item));

                    new MerchantRecommendDAC().Delete(information.Id);

                    merchantRecommends.ForEach(item => new MerchantRecommendDAC().Insert(item));

                    scope.Complete();
                }
            }
            }
        }


        /// <summary>
        /// 获取商家类别
        /// </summary>
        /// <returns></returns>
        public List<StoreType> StoreTypeQuerAll()
        {
            return new StoreTypeDAC().GetList();
        }

        /// <summary>
        /// 查询商家是否停用
        /// </summary>
        /// <returns></returns>
        public MerchantInformation GetStatusId(Guid accountId)
        {
            return new MerchantInformationDAC().GetByMerchantAccountId(accountId);
        }

        /// <summary>
        /// 商家门店按钮是否停用
        /// </summary>
        /// <param name="accountId"></param>
        public void UpdateStatus(Guid accountId)
        {
            new MerchantInformationDAC().UpdateStatus(accountId);
        }

        /// <summary>
        /// 商家门店审核失败原因
        /// </summary>
        /// <param name="accountId"></param>
        public string CauseFailure(Guid accountId)
        {
            return new MerchantInformationDAC().CauseFailure(accountId);
        }

    }
}

