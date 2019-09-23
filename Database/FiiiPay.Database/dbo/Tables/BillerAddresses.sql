CREATE TABLE [dbo].[BillerAddresses]
(
	[Id] INT NOT NULL IDENTITY (1, 1), 
    [AccountId] UNIQUEIDENTIFIER NOT NULL, 
    [BillerCode] VARCHAR(50) NOT NULL, 
    [ReferenceNumber] VARCHAR(50) NOT NULL, 
    [Tag] VARCHAR(50) NOT NULL, 
    [IconIndex] VARCHAR(30) NULL, 
    [Timestamp] DATETIME NOT NULL, 
    CONSTRAINT [PK_BillerAddress] PRIMARY KEY ([Id])
)
