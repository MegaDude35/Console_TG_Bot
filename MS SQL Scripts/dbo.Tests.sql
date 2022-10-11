USE [TG_Test_Bot]
GO

/****** Объект: Table [dbo].[Tests] Дата скрипта: 11.10.2022 18:06:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Tests];


GO
CREATE TABLE [dbo].[Tests] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [Time_to_Take] TINYINT        NOT NULL,
    [Author_ID]    BIGINT         NOT NULL
);


