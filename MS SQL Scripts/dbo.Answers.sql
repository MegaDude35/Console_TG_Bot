USE [TG_Test_Bot]
GO

/****** Объект: Table [dbo].[Answers] Дата скрипта: 11.10.2022 18:05:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Answers];


GO
CREATE TABLE [dbo].[Answers] (
    [ID]          INT     IDENTITY (1, 1) NOT NULL,
    [Test_Key_ID] INT     NOT NULL,
    [User_ID]     BIGINT  NOT NULL,
    [Question_ID] INT     NOT NULL,
    [Try]         TINYINT NOT NULL
);


