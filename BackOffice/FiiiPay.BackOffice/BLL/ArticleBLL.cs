using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Entities;
using System;
using FiiiPay.Framework.Queue;


namespace FiiiPay.BackOffice.BLL
{
    public class ArticleBLL : BaseBLL
    {

        public SaveResult Create(Articles article, int userId, string userName)
        {
            article.CreateTime = DateTime.UtcNow;
            var articleId = FiiiPayDB.ArticlesDb.InsertReturnIdentity(article);
            bool saveSuccess = articleId > 0;
            if (article.ShouldPop && article.ShouldPop && saveSuccess && (!article.HasPushed.HasValue || !article.HasPushed.Value))
            {
                if (article.AccountType == ArticleAccountType.FiiiPay)
                {
                    //Utils.MSMQ.UserArticle(articleId,0);
                    RabbitMQSender.SendMessage("UserArticle", articleId);
                }
                else
                {
                    //Utils.MSMQ.MerchantArticle(articleId,0);
                    RabbitMQSender.SendMessage("MerchantArticle", articleId);
                }
                article.HasPushed = true;
                article.Id = articleId;
                FiiiPayDB.ArticlesDb.Update(article);
            }

            if (saveSuccess)
            {
                ActionLog actionLog = new ActionLog();
                actionLog.IPAddress = GetClientIPAddress();
                actionLog.AccountId = userId;
                actionLog.CreateTime = DateTime.UtcNow;
                actionLog.ModuleCode = typeof(ArticleBLL).FullName + ".Create";
                actionLog.Username = userName;
                actionLog.LogContent = "Create Article " + articleId;
                new ActionLogBLL().Create(actionLog);
            }

            return new SaveResult(saveSuccess);
        }

        public SaveResult Update(Articles article, int userId, string userName)
        {
            Articles oldArticle = FiiiPayDB.ArticlesDb.GetById(article.Id);
            oldArticle.Title = article.Title;
            oldArticle.ShouldPop = article.ShouldPop;
            oldArticle.Body = article.Body;
            oldArticle.Descdescription = article.Descdescription;
            oldArticle.AccountType = article.AccountType;
            oldArticle.UpdateTime = DateTime.UtcNow;

            bool saveSuccess = FiiiPayDB.ArticlesDb.Update(oldArticle);

            if (article.ShouldPop && article.ShouldPop && saveSuccess && (!article.HasPushed.HasValue || !article.HasPushed.Value))
            {
                if (article.AccountType == ArticleAccountType.FiiiPay)
                {
                    //Utils.MSMQ.UserArticle(article.Id,0);
                    RabbitMQSender.SendMessage("UserArticle", article.Id);
                }
                else
                {

                    RabbitMQSender.SendMessage("MerchantArticle", article.Id);
                }
            }

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ArticleBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Article " + article.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(saveSuccess);
        }

        public SaveResult DeleteById(int id, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ArticleBLL).FullName + ".DeleteById";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete Article " + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.ArticlesDb.DeleteById(id));
        }

        public SaveResult BatchDelete(string ids, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ArticleBLL).FullName + ".BatchDelete";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete Article " + ids;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.ArticlesDb.DeleteByIds(ids.Split(',')));
        }
    }
}