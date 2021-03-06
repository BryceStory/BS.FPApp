﻿CREATE TABLE [dbo].[UserLoginLog] (
    [Id]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserAccountId] UNIQUEIDENTIFIER NOT NULL,
    [IP]            VARCHAR (50)     NULL,
    [Manufacturer]  VARCHAR (50)     NULL,
    [Model]         VARCHAR (50)     NULL,
    [OS]            VARCHAR (50)     NULL,
    [OSVersion]     VARCHAR (50)     NULL,
    [AppVersion]    VARCHAR (50)     NULL,
    [Timestamp]     DATETIME         NOT NULL,
    [IsSuccess]     BIT              CONSTRAINT [DF_LoginLog_IsSuccess] DEFAULT ((0)) NOT NULL,
    [Remark]        NVARCHAR (500)   NULL,
    CONSTRAINT [PK_UserLoginLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);



