namespace FiiiPay.BackOffice.Utils
{
    public static class RedisMQ
    {
        //private static ISubscriber subscriber = RedisHelper.Connection.GetSubscriber();

        //public static void Init()
        //{
        //    //Sub("Sample", Sample);
        //}

        //private static void Sub(string channel, Action<string> action)
        //{
        //    subscriber.Subscribe(channel, (a, b) =>
        //    {
        //        action(b);
        //    });
        //}

        ///// <summary>
        ///// 推送给用户的公告
        ///// </summary>
        ///// <param name="id">公告的Id</param>
        //public static void UserArticle(int id)
        //{
        //    subscriber.Publish("UserArticle", $"{id}");
        //}

        ///// <summary>
        ///// 推送给商家的公告
        ///// </summary>
        ///// <param name="id">公告的Id</param>
        //public static void MerchantArticle(int id)
        //{
        //    subscriber.Publish("MerchantArticle", $"{id}");
        //}

        //public static void BackOfficeRefundOrder(string orderno)
        //{
        //    subscriber.Publish("BackOfficeRefundOrder", orderno);
        //}

        //public static void UserLv1Verified(long recordId)
        //{
        //    subscriber.Publish("UserLv1Verified", $"{recordId}");
        //}

        //public static void UserLv1Reject(long recordId)
        //{
        //    subscriber.Publish("UserLv1Reject", $"{recordId}");
        //}

        //public static void UserLv2Verified(long recordId)
        //{
        //    subscriber.Publish("UserLv2Verified", $"{recordId}");
        //}

        //public static void UserLv2Reject(long recordId)
        //{
        //    subscriber.Publish("UserLv2Reject", $"{recordId}");
        //}

        //public static void MerchantVerified(long recordId)
        //{
        //    subscriber.Publish("MerchantVerified", $"{recordId}");
        //}

        //public static void MerchantVerifiedFailed(long recordId)
        //{
        //    subscriber.Publish("MerchantVerifiedFailed", $"{recordId}");
        //}
    }
}