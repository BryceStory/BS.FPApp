CREATE TABLE [dbo].[MerchantInformations]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [MerchantName] NVARCHAR(200) NOT NULL, 
    [Week] INT NOT NULL, 
    [StartTime] NVARCHAR(10) NOT NULL, 
    [EndTime] NVARCHAR(10) NOT NULL, 
    [Tags] NVARCHAR(200) NOT NULL, 
    [Introduce] NVARCHAR(4000) NULL, 
    [Address] NVARCHAR(300) NOT NULL, 
    [Lng] DECIMAL(10, 8) NOT NULL, 
    [Lat] DECIMAL(10, 8) NOT NULL, 
    [Status] TINYINT NOT NULL, 
    [VerifyStatus] TINYINT NOT NULL,
	[MerchantAccountId] UNIQUEIDENTIFIER NOT NULL, 
    [Phone] NVARCHAR(10) NOT NULL
)
