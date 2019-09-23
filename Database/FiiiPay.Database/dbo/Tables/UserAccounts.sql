CREATE TABLE [dbo].[UserAccounts] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [PhoneCode]         VARCHAR (10)     NULL,
    [Cellphone]         VARCHAR (50)     NOT NULL,
    [Email]             VARCHAR (50)     NULL,
    [IsVerifiedEmail]   BIT              CONSTRAINT [DF_UserAccounts_IsVerifiedEmail] DEFAULT ((0)) NOT NULL,
    [RegistrationDate]  DATETIME         NOT NULL,
    [CountryId]         INT              NOT NULL,
    [Photo]             UNIQUEIDENTIFIER NULL,
    [Password]          VARCHAR (300)    NOT NULL,
    [PIN]               VARCHAR (300)    NULL,
    [SecretKey]         VARCHAR (100)    NOT NULL,
    [Status]            TINYINT          NULL,
    [IsAllowWithdrawal] BIT              NULL,
    [IsAllowExpense]    BIT              NULL,
    [FiatCurrency]      VARCHAR (50)     CONSTRAINT [DF_UserAccounts_FiatCurrency] DEFAULT ('USD') NOT NULL,
    [AuthSecretKey]     VARCHAR (50)     NULL,
    [InvitationCode]    VARCHAR (8)      NOT NULL,
    [InviterCode]       VARCHAR (8)      NULL,
    [ValidationFlag]    TINYINT          CONSTRAINT [DF_UserAccounts_ValidationFlag] DEFAULT ((0)) NOT NULL,
    [IsAllowTransfer]   BIT              NULL,
    [Language]          VARCHAR (10)     CONSTRAINT [DF_UserAccounts_Language] DEFAULT ('en-US') NULL,
    [L1VerifyStatus]    TINYINT          CONSTRAINT [DF__UserAccou__L1Ver__11BF94B6] DEFAULT ((0)) NOT NULL,
    [L2VerifyStatus]    TINYINT          CONSTRAINT [DF__UserAccou__L2Ver__12B3B8EF] DEFAULT ((0)) NOT NULL,
    [IsBindingDevice] TINYINT NOT NULL, 
    CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([Id] ASC)
);



