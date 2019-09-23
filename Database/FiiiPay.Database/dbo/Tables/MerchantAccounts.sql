CREATE TABLE [dbo].[MerchantAccounts] (
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    [PhoneCode]            VARCHAR (10)     NULL,
    [Cellphone]            VARCHAR (50)     NOT NULL,
    [Username]             NVARCHAR (50)    NOT NULL,
    [MerchantName]         NVARCHAR (100)   NULL,
    [Status]               TINYINT          CONSTRAINT [DF_MerchantAccounts_Status] DEFAULT ((1)) NULL,
    [POSId]                BIGINT           NULL,
    [Email]                VARCHAR (30)     NULL,
    [IsVerifiedEmail]      BIT              CONSTRAINT [DF_Merchants_IsVerifiedEmail] DEFAULT ((0)) NOT NULL,
    [CountryId]            INT              NOT NULL,
    [RegistrationDate]     DATETIME         NOT NULL,
    [Photo]                VARCHAR (50)     NULL,
    [PIN]                  VARCHAR (300)    NULL,
    [SecretKey]            VARCHAR (100)    NOT NULL,
    [IsAllowWithdrawal]    BIT              CONSTRAINT [DF_MerchantAccounts_IsAllowWithdraw] DEFAULT ((1)) NOT NULL,
    [IsAllowAcceptPayment] BIT              CONSTRAINT [DF_MerchantAccounts_IsAllowAcceptPayment] DEFAULT ((1)) NOT NULL,
    [FiatCurrency]         VARCHAR (50)     NOT NULL,
    [Receivables_Tier]     DECIMAL (10, 4)  NULL,
    [Markup]               DECIMAL (10, 4)  CONSTRAINT [DF_MerchantAccounts_Markup] DEFAULT ((0)) NOT NULL,
    [VerifyStatus]         TINYINT          NULL,
    [AuthSecretKey]        VARCHAR (50)     NULL,
    [ValidationFlag]       TINYINT          CONSTRAINT [DF_MerchantAccounts_ValidationFlag] DEFAULT ((0)) NOT NULL,
    [MinerCryptoAddress]   VARCHAR (50)     NULL,
    [L1VerifyStatus]       TINYINT          CONSTRAINT [DF__tmp_ms_xx__L1Ver__5C57A83E] DEFAULT ((0)) NOT NULL,
    [L2VerifyStatus]       TINYINT          CONSTRAINT [DF__tmp_ms_xx__L2Ver__5D4BCC77] DEFAULT ((0)) NOT NULL,
    [Language]             VARCHAR (10)     CONSTRAINT [DF_MerchantAccounts_Language] DEFAULT ('en-US') NULL,
    [WithdrawalFeeType]    TINYINT          CONSTRAINT [DF_MerchantAccounts_WithdrawalFee] DEFAULT ((0)) NOT NULL,
    [InvitationCode]       VARCHAR (8)      NULL,
    CONSTRAINT [PK_Merchants] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO