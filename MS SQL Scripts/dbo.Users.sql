USE [TG_Test_Bot]
GO

ALTER TABLE [dbo].[Users] DROP CONSTRAINT [DF_Users_Author]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 13.10.2022 22:54:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
DROP TABLE [dbo].[Users]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 13.10.2022 22:54:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[TG_ID] [bigint] NOT NULL,
	[Firstname] [nchar](32) NOT NULL,
	[Lastname] [nchar](32) NULL,
	[Author] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[TG_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_Author]  DEFAULT ((0)) FOR [Author]
GO

