CREATE TABLE [dbo].[OrderWithdrawalFee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CryptoId] [int] NOT NULL,
	[Amount] [decimal](18, 8) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[OrderId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_OrderWithdrawalFee] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO