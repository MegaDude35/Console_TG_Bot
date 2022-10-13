USE [TG_Test_Bot]
GO

ALTER TABLE [dbo].[Answers] DROP CONSTRAINT [FK_Answers_Users]
GO

ALTER TABLE [dbo].[Answers] DROP CONSTRAINT [FK_Answers_Test_Keys]
GO

ALTER TABLE [dbo].[Answers] DROP CONSTRAINT [FK_Answers_Questions]
GO

/****** Object:  Table [dbo].[Answers]    Script Date: 13.10.2022 19:21:21 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Answers]') AND type in (N'U'))
DROP TABLE [dbo].[Answers]
GO

/****** Object:  Table [dbo].[Answers]    Script Date: 13.10.2022 19:21:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Answers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Test_Key_ID] [int] NOT NULL,
	[User_ID] [bigint] NOT NULL,
	[Question_ID] [int] NOT NULL,
	[Try] [tinyint] NOT NULL,
 CONSTRAINT [PK_Answers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Answers]  WITH CHECK ADD  CONSTRAINT [FK_Answers_Questions] FOREIGN KEY([Question_ID])
REFERENCES [dbo].[Questions] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Answers] CHECK CONSTRAINT [FK_Answers_Questions]
GO

ALTER TABLE [dbo].[Answers]  WITH CHECK ADD  CONSTRAINT [FK_Answers_Test_Keys] FOREIGN KEY([Test_Key_ID])
REFERENCES [dbo].[Test_Keys] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Answers] CHECK CONSTRAINT [FK_Answers_Test_Keys]
GO

ALTER TABLE [dbo].[Answers]  WITH CHECK ADD  CONSTRAINT [FK_Answers_Users] FOREIGN KEY([User_ID])
REFERENCES [dbo].[Users] ([TG_ID])
GO

ALTER TABLE [dbo].[Answers] CHECK CONSTRAINT [FK_Answers_Users]
GO

