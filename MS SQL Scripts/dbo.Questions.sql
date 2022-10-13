USE [TG_Test_Bot]
GO

ALTER TABLE [dbo].[Questions] DROP CONSTRAINT [FK_Questions_Tests]
GO

/****** Object:  Table [dbo].[Questions]    Script Date: 13.10.2022 20:13:13 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Questions]') AND type in (N'U'))
DROP TABLE [dbo].[Questions]
GO

/****** Object:  Table [dbo].[Questions]    Script Date: 13.10.2022 20:13:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Questions](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Test_ID] [int] NOT NULL,
	[Question_Group] [int] NOT NULL,
	[Question_Type] [tinyint] NOT NULL,
	[Question_Text] [text] NOT NULL,
	[Question_Ball] [real] NOT NULL,
 CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Questions]  WITH CHECK ADD  CONSTRAINT [FK_Questions_Tests] FOREIGN KEY([Test_ID])
REFERENCES [dbo].[Tests] ([ID])
GO

ALTER TABLE [dbo].[Questions] CHECK CONSTRAINT [FK_Questions_Tests]
GO

