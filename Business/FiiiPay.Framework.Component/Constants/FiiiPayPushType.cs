﻿namespace FiiiPay.Framework.Constants
{
    public static class FiiiPayPushType
    {
        /// <summary>
        /// 订单待支付
        /// </summary>
        public const int TYPE_PUSH_PAY_ORDER = 0;
        /// <summary>
        /// 收款成功
        /// 需要保存在消息列表
        /// </summary>
        public const int TYPE_RECEIPT = 1;
        /// <summary>
        /// 消费（支付成功）
        /// 需要保存在消息列表
        /// </summary>
        public const int TYPE_CONSUME = 2;
        /// <summary>
        /// 退款成功
        /// 需要保存在消息列表
        /// </summary>
        public const int TYPE_REFUND_ORDER = 3;
        /// <summary>
        /// 公告
        /// </summary>
        public const int TYPE_ARTICLE = 4;
        /// <summary>
        /// 充币成功
        /// </summary>
        /// 需要保存在消息列表
        public const int TYPE_DEPOSIT = 5;
        /// <summary>
        /// 提币成功
        /// </summary>
        /// 需要保存在消息列表
        public const int TYPE_WITHDRAWAL = 6;
        /// <summary>
        /// 提币失败
        /// </summary>
        /// 需要保存在消息列表
        public const int TYPE_WITHDRAWAL_Reject = 7;
        /// <summary>
        /// 用户KYCLv1认证通过
        /// </summary>
        public const int TYPE_USER_KYC_LV1_VERIFIED = 8;
        /// <summary>
        /// 用户KYCLv1认证失败
        /// </summary>
        public const int TYPE_USER_KYC_LV1_REJECT = 9;
        /// <summary>
        /// 用户KYCLv2认证通过
        /// </summary>
        public const int TYPE_USER_KYC_LV2_VERIFIED = 10;
        /// <summary>
        /// 用户KYCLv2认证失败
        /// </summary>
        public const int TYPE_USER_KYC_LV2_REJECT = 11;
        /// <summary>
        /// 商家KYCLv1认证通过
        /// </summary>
        public const int TYPE_Merchant_KYC_LV1_VERIFIED = 12;
        /// <summary>
        /// 商家KYCLv1认证失败
        /// </summary>
        public const int TYPE_Merchant_KYC_LV1_REJECT = 13;
        /// <summary>
        /// 商家KYCLv2认证通过
        /// </summary>
        public const int TYPE_Merchant_KYC_LV2_VERIFIED = 20;
        /// <summary>
        /// 商家KYCLv2认证失败
        /// </summary>
        public const int TYPE_Merchant_KYC_LV2_REJECT = 21;
        /// <summary>
        /// 充币失败
        /// </summary>
        /// 需要保存在消息列表
        public const int TYPE_DEPOSIT_CANCEL = 14;
        /// <summary>
        /// 用户收到转账
        /// </summary>
        public const int TYPE_USER_TRANSFER_INTO = 15;
        /// <summary>
        /// 转账成功
        /// </summary>
        public const int TYPE_USER_TRANSFER_OUT = 16;
        /// <summary>
        /// 从FiiiEx划转成功
        /// </summary>
        public const int TYPE_TRANSFER_FROM_EX = 17;
        /// <summary>
        /// 划出到FiiiEX成功
        /// </summary>
        public const int TYPE_TRANSFER_TO_EX = 18;
        /// <summary>
        /// 邀请注册获得奖励
        /// </summary>
        public const int TYPE_INVITE_REWARD = 19;
    }
}