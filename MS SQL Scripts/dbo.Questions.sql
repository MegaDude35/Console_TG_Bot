USE [TG_Test_Bot]
GO

/****** Объект: Table [dbo].[Questions] Дата скрипта: 11.10.2022 18:05:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Questions];


GO
CREATE TABLE [dbo].[Questions] (
    [ID]             INT     IDENTITY (1, 1) NOT NULL,
    [Test_ID]        INT     NOT NULL,
    [Question_Group] INT     NOT NULL,
    [Question_Type]  TINYINT NOT NULL,
    [Question_Text]  TEXT    NOT NULL,
    [Question_Ball]  REAL    NOT NULL
);


