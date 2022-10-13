USE [TG_Test_Bot]
GO

ALTER TABLE [dbo].[Tests] DROP CONSTRAINT [DF__Tests__Creation___3A4CA8FD]
GO

/****** Object:  Table [dbo].[Tests]    Script Date: 13.10.2022 19:20:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tests]') AND type in (N'U'))
DROP TABLE [dbo].[Tests]
GO

/****** Object:  Table [dbo].[Tests]    Script Date: 13.10.2022 19:20:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Tests](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Time_to_Take] [tinyint] NOT NULL,
	[Author_ID] [bigint] NOT NULL,
	[Creation_Date] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_Tests] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Tests] ADD  DEFAULT (getdate()) FOR [Creation_Date]
GO

