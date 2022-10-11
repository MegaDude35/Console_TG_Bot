USE [TG_Test_Bot]
GO

/****** Объект: Table [dbo].[Users] Дата скрипта: 11.10.2022 18:06:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Users];


GO
CREATE TABLE [dbo].[Users] (
    [TG_ID]     BIGINT     NOT NULL,
    [Firstname] NCHAR (32) NOT NULL,
    [Lastname]  NCHAR (32) NULL,
    [Author]    BIT        NOT NULL
);


