namespace FiiiPay.Framework
{
    public class ReasonCode
    {
        /// <summary>
        /// Common error
        /// </summary>
        public const int GENERAL_ERROR = 10000;

        /// <summary>
        /// Missing some required fileds or data format invalid
        /// </summary>
        public const int MISSING_REQUIRED_FIELDS = 10001;

        /// <summary>
        /// Record is not existed
        /// </summary>
        public const int RECORD_NOT_EXIST = 10002;

        /// <summary>
        /// 需要进一步验证，用于登录的时候判断用户是否开启了谷歌验证等额外验证
        /// </summary>
        public const int ADVANCED_AUTHRE_QUIRED = 10003;

        /// <summary>
        /// 用户所在的国家被禁止请求
        /// </summary>
        public const int COUNTRY_FORBBIDEN_REQUEST = 10004;

        /// <summary>
        /// 未定义的已知错误
        /// </summary>
        public const int UNDEFINED_KNOWNERROR = 10005;

        /// <summary>
        /// 未登陆，客户端根据这个错误码跳转到登陆页面
        /// </summary>
        public const int UNAUTHORIZED = 10010;

        /// <summary>
        /// 账户已被注册
        /// </summary>
        public const int ACCOUNT_EXISTS = 10011;

        /// <summary>
        /// 账户未注册或不存在
        /// </summary>
        public const int ACCOUNT_NOT_EXISTS = 10012;
        /// <summary>
        /// Pin码已连续错误5次
        /// </summary>
        public const int PIN_ERROR_5_TIMES = 10013;
        /// <summary>
        /// PIN输入错误
        /// </summary>
        public const int PIN_ERROR = 10014;

        /// <summary>
        /// 新旧Pin不能一致
        /// </summary>
        public const int PIN_MUST_BE_DIFFERENT = 10015;

        /// <summary>
        /// 邀请码不存在
        /// </summary>

        public const int INVITORCODE_NOT_EXISTS = 10016;

        /// <summary>
        /// Token过期
        /// </summary>
        public const int ACCESSTOKEN_EXPIRE = 10017;

        /// <summary>
        /// 无效的邀请码
        /// </summary>

        public const int INVALID_INVITECODE = 10018;

        /// <summary>
        /// 帐号已锁定
        /// </summary>
        public const int ACCOUNT_LOCKED = 10020;

        /// <summary>
        /// 帐号已禁用
        /// </summary>
        public const int ACCOUNT_DISABLED = 10021;
        /// <summary>
        /// 新旧密码不能一致
        /// </summary>
        public const int PWD_MUST_BE_DIFFERENT = 10022;

        /// <summary>
        /// 帐号已被解绑
        /// </summary>
        public const int ACCOUNT_UNBUNDLED = 10028;
        /// <summary>
        /// POS SN码错误  SN码不存在
        /// </summary>
        public const int POSSN_ERROR = 10029;

        /// <summary>
        /// 余额不足
        /// </summary>
        public const int INSUFFICIENT_BALANCE = 10030;

        /// <summary>
        /// 不能转给自己
        /// </summary>
        public const int CANNOT_TRANSFER_TO_YOURSELF = 10040;

        /// <summary>
        /// 禁止提币
        /// </summary>
        public const int Not_Allow_Withdrawal = 10060;

        /// <summary>
        /// 商家禁止收款
        /// </summary>
        public const int Not_Allow_AcceptPayment = 10061;

        /// <summary>
        /// 用户禁止消费
        /// </summary>
        public const int Not_Allow_Expense = 10062;
        /// <summary>
        /// 暂不支持充提币功能
        /// </summary>
        public const int NOT_SUPORT_WITHDRAWAL = 10063;

        /// <summary>
        /// 密码错误
        /// </summary>
        public const int WRONG_PASSWORD_ENTERRED = 10070;

        /// <summary>
        /// 尝试通过密码登录多次
        /// </summary>
        public const int LOGIN_ERROR_TOO_MANY_TIMES = 10071;

        /// <summary>
        /// 尝试通过短信登录多次
        /// </summary>
        public const int LOGIN_ERROR_TOO_MANY_TIMES_BYSMS = 10072;

        /// <summary>
        /// 修改密码时，旧密码错误
        /// </summary>
        public const int WRONG_OLD_PASSWORD_ENTERRED = 10073;

        /// <summary>
        /// 修改密码时，旧密码错误多次
        /// </summary>
        public const int OLD_PASSWORD_TOO_MANY_TIMES = 10074;

        /// <summary>
        /// 短信验证码错误
        /// </summary>
        public const int WRONG_CODE_ENTERRED = 10080;
        /// <summary>
        /// 短信验证失败超过多次
        /// </summary>
        public const int PHONECODE_VERIFYFAILED_TOOMANY_TEIMS = 10081;
        /// <summary>
        /// 验证码过期
        /// </summary>
        public const int CODE_EXPIRE = 10082;
        /// <summary>
        /// 一天内短信发送次数超出限制
        /// </summary>
        public const int PHONECODE_SEND_TOOMANY_TIMES = 10083;
        /// <summary>
        /// 验证信息不通过
        /// </summary>
        public const int VERIFY_FAILD = 10084;
        /// <summary>
        /// 验证信息不通过多次
        /// </summary>
        public const int VERIFY_FAILD_MORETIMES = 10085;

        /// <summary>
        /// 无效的二维码
        /// </summary>
        public const int INVALID_QRCODE = 10090;

        /// <summary>
        /// 未绑定谷歌验证
        /// </summary>
        public const int NOT_BIND_AUTHENTICATOR = 10100;
        /// <summary>
        /// 谷歌验证失败多次
        /// </summary>
        public const int GOOGLEAUTH_ERROR_TOO_MANY_TIMES = 10101;
        /// <summary>
        /// 谷歌验证失败
        /// </summary>
        public const int GOOGLEAUTH_VERIFY_FAIL = 10102;
        /// <summary>
        /// 谷歌验证码已使用过
        /// </summary>
        public const int GOOGLECODE_BEUSED = 10103;

        /// <summary>
        /// 安全验证未通过
        /// </summary>
        public const int FAIL_AUTHENTICATOR = 10110;
        /// <summary>
        /// 安全验证失败多次
        /// </summary>
        public const int SECURITY_ERROR_TOO_MANY_TIMES = 10111;
        /// <summary>
        /// 安全验证短信码验证错误
        /// </summary>
        public const int WRONG_SECURITYPHONECODE_ENTERRED = 10112;
        /// <summary>
        /// 安全验证谷歌验证码验证错误
        /// </summary>
        public const int WRONG_SECURITYGOOGLECODE_ENTERRED = 10113;
        /// <summary>
        /// 禁止转账
        /// </summary>
        public const int TRANSFER_FORBIDDEN = 10120;
        /// <summary>
        /// 转账金额超出最大值
        /// </summary>
        public const int TRANSFER_AMOUNT_OVERFLOW = 10121;
        /// <summary>
        /// 转账转出用户余额不足
        /// </summary>
        public const int TRANSFER_BALANCE_LOW = 10122;
        /// <summary>
        /// 转账转给自己
        /// </summary>
        public const int TRANSFER_TO_SELF = 10123;
        /// <summary>
        /// 货币异常
        /// </summary>
        public const int CURRENCY_FORBIDDEN = 10130;

        /// <summary>
        /// 加密币不存在
        /// </summary>
        public const int CRYPTO_NOT_EXISTS = 10016;

        /// <summary>
        /// 订单已完成或者已退款
        /// </summary>
        public const int ORDER_HAD_COMPLETE = 10150;

        /// <summary>
        /// 不能提币到FiiiPos
        /// </summary>
        public const int CAN_NOT_WITHDRAW_TO_FiiiPOS = 10200;

        /// <summary>
        /// VerifyStatus未通过
        /// </summary>
        public const int NOT_VERIFY_LV1 = 10160;

        /// <summary>
        /// 无效的地址标签
        /// </summary>
        public const int INVALID_TAG = 10170;
        /// <summary>
        /// 需要输入Tag
        /// </summary>
        public const int NEED_INPUT_TAG = 10171;

        /// <summary>
        /// 地址格式不合法
        /// </summary>
        public const int INVALID_ADDRESS = 10172;

        /// <summary>
        /// 禁止挖矿
        /// </summary>
        public const int NOT_ALLOW_MINING = 10180;
        /// <summary>
        /// 证件号使用超限
        /// </summary>
        public const int IDENTITYNO_USED_OVERLIMIT = 10190;
        /// <summary>
        /// 商家账号异常
        /// </summary>
        public const int MERCHANT_ACCOUNT_DISABLED = 10200;
        /// <summary>
        /// 接口维护
        /// </summary>
        public const int API_MAINTAIN = 10300;

        /// <summary>
        /// 邮箱和原设定不一致
        /// </summary>
        public const int EMAIL_NOT_MATCH = 10310;
        /// <summary>
        /// 新旧邮箱相同
        /// </summary>
        public const int ORIGIN_NEW_EMAIL_SAME = 10311;
        /// <summary>
        /// 邮箱已经被其它帐号使用
        /// </summary>
        public const int EMAIL_BINDBYOTHER = 10312;
        /// <summary>
        /// 商家没有门店信息
        /// </summary>
        public const int MERCHANT_NONENTITY = 103001;

        /// <summary>
        /// 商家已绑定门店
        /// </summary>
        public const int MERCAHNT_BINDING = 103002;

        /// <summary>
        /// 商家没有门店
        /// </summary>
        public const int MERCHANT_NONE = 103003;

        /// <summary>
        /// 该商家已下架
        /// </summary>
        public const int MERCHANT_NOT_PUBLIC = 103004;

        /// <summary>
        /// 证件号不符
        /// </summary>
        public const int IdentityDocNo_NOT_RIGHT = 10_20_10_25;
        /// <summary>
        /// 证件号错误超过5次
        /// </summary>
        public const int IdentityDocNo_Error_5Times = 10_20_10_27;
        /// <summary>
        /// 该手机号已注册过FiiiPay。
        /// </summary>
        public const int PhoneNumber_Exist = 10320;

        public const int PhoneNumber_Invalid = 10321;

        public class FiiiPosReasonCode
        {
            /// <summary>
            /// 账户未注册或不存在
            /// </summary>
            public const int ACCOUNT_NOT_EXISTS = 10_20_10_01;
            /// <summary>
            /// 帐号已禁用
            /// </summary>
            public const int ACCOUNT_DISABLED = 10_20_10_02;
            /// <summary>
            /// 用户未扫码
            /// </summary>
            public const int ACCOUNT_NOT_SCAN = 10_20_10_03;
            /// <summary>
            /// 邮箱验证码错误
            /// </summary>
            public const int WRONG_EMAIL_CODE_ENTERRED = 10_20_10_04;
            /// <summary>
            /// 邮箱验证失败多次
            /// </summary>
            public const int EMAILCODE_ERROR_TOO_MANY_TIMES = 10_20_10_05;
            /// <summary>
            /// 邮箱验证码发送次数过多
            /// </summary>
            public const int EMAILCODE_SEND_TOO_MANY_TIMES = 10_20_10_06;
            /// <summary>
            /// 获取不到图片
            /// </summary>
            public const int NO_IMAGE = 10_20_10_07;
            /// <summary>
            /// 图片格式错误
            /// </summary>
            public const int IMAGE_FORMAT_ERROR = 10_20_10_08;
            /// <summary>
            /// 账户已审核,不能再次更改
            /// </summary>
            public const int VERIFYED_STATUS = 10_20_10_09;
            /// <summary>
            /// 邀请码不存在
            /// </summary>
            public const int INVITERCODE_NOT_EXISTS = 10_20_10_10;
            /// <summary>
            /// 新旧邮箱不能一致
            /// </summary>
            public const int EMAIL_MUST_BE_DIFFERENT = 10_20_10_11;
            /// <summary>
            /// 邮箱已绑定
            /// </summary>
            public const int EMAIL_BINDED = 10_20_10_12;
            /// <summary>
            /// 邮箱地址不正确
            /// </summary>
            public const int EMAIL_NOT_RIGHT = 10_20_10_13;
            /// <summary>
            /// 验证码过期
            /// </summary>
            public const int EMAIL_CODE_EXPIRE = 10_20_10_15;
            /// <summary>
            /// 订单不存在
            /// </summary>
            public const int ORDERNO_NOT_EXISTS = 10_20_10_16;
            /// <summary>
            /// 订单不属于该用户
            /// </summary>
            public const int ORDERNO_NOTBE_ACCOUNT = 10_20_10_17;
            /// <summary>
            /// 订单已完成,不能退款
            /// </summary>
            public const int ORDER_COMPLETED = 10_20_10_18;
            /// <summary>
            /// 订单已过期,不能退款
            /// </summary>
            public const int ORDER_EXPIRE = 10_20_10_19;
            /// <summary>
            /// 退款商家余额不足
            /// </summary>
            public const int REFUND_BALANCE_LOW = 10_20_10_20;
            /// <summary>
            /// 退款商家钱包不存在
            /// </summary>
            public const int REFUND_MERCHANT_WALLET_NOT_EXISTS = 10_20_10_21;
            /// <summary>
            /// 退款用户钱包不存在
            /// </summary>
            public const int REFUND_USER_WALLET_NOT_EXISTS = 10_20_10_22;
            /// <summary>
            /// 新旧电话号码不能一致
            /// </summary>
            public const int CELLPHONE_MUST_BE_DIFFERENT = 10_20_10_23;
            /// <summary>
            /// 资料已提交,不能再次提交
            /// </summary>
            public const int COMMITTED_STATUS = 10_20_10_24;

        }
        public class FiiiShopCode
        {
            /// <summary>
            /// 扫码未登陆
            /// </summary>
            public const int ScanUnLogined = 30001;
            /// <summary>
            /// 扫码登录中
            /// </summary>
            public const int ScanLogining = 30002;
        }
        #region biller错误码
        /// <summary>
        /// 超过单笔
        /// </summary>
        public const int BillerOverMaxAmount = 10401;

        /// <summary>
        /// 超过当日
        /// </summary>
        public const int BillerOverDayMaxAmount = 10402;

        /// <summary>
        /// 超过当月
        /// </summary>
        public const int BillerOverMonthMaxAmount = 10403;

        /// <summary>
        /// 金额有误
        /// </summary>
        public const int BillerInvalidValues = 10404;

        /// <summary>
        /// 国家信息不对
        /// </summary>
        public const int BillerInvalidCountry = 10405;

        /// <summary>
        /// 超过地址添加最大数量
        /// </summary>
        public const int BillerOverMaxAddressCount = 10406;

        /// <summary>
        /// 添加地址已经存在
        /// </summary>
        public const int BillerAddressExisted = 10407;

        /// <summary>
        /// biller功能暂不能使用
        /// </summary>
        public const int BillerForbidden = 10408;

        #endregion
    }
}
