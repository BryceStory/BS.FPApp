CREATE TABLE [dbo].[Advertisings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Link] [nvarchar](100) NOT NULL,
	[Status] [bit] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[PictureZh] [uniqueidentifier] NOT NULL,
	[PictureEn] [uniqueidentifier] NOT NULL,
	[Version] [int] NOT NULL,
	[StartDate] DATETIME NOT NULL,
	[EndDate] DATETIME NOT NULL,
	[LinkType] [bigint] NOT NULL,
 CONSTRAINT [PK__Advertis__3214EC0712D132B8] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


