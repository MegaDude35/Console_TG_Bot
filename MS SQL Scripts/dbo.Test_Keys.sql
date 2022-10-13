USE [TG_Test_Bot]
GO

ALTER TABLE [dbo].[Test_Keys] DROP CONSTRAINT [FK_Test_Keys_Tests]
GO

/****** Object:  Table [dbo].[Test_Keys]    Script Date: 13.10.2022 19:20:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Test_Keys]') AND type in (N'U'))
DROP TABLE [dbo].[Test_Keys]
GO

/****** Object:  Table [dbo].[Test_Keys]    Script Date: 13.10.2022 19:20:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Test_Keys](
	[ID] [int] NOT NULL,
	[Test_Key] [nchar](32) NULL,
	[Test_ID] [int] NOT NULL,
	[User_ID] [bigint] NULL,
	[Date_Enable_Key] [smalldatetime] NOT NULL,
	[Date_Disable_Key] [smalldatetime] NOT NULL,
	[Tryes] [tinyint] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Test_Keys] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Test_Keys]  WITH CHECK ADD  CONSTRAINT [FK_Test_Keys_Tests] FOREIGN KEY([Test_ID])
REFERENCES [dbo].[Tests] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Test_Keys] CHECK CONSTRAINT [FK_Test_Keys_Tests]
GO

