USE [RR]
GO

/****** Object:  Table [dbo].[TestTable]    Script Date: 28/11/2564 00:08:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TestTable](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TransactionID] [nvarchar](50) NULL,
	[Amount] [decimal](18, 2) NULL,
	[CurrencyCode] [nvarchar](5) NULL,
	[TransactionDate] [datetime] NULL,
	[Status] [nvarchar](5) NULL,
	[FileType] [nvarchar](5) NULL,
	[FileStatus] [nvarchar](50) NULL,
 CONSTRAINT [PK_TestTable] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


