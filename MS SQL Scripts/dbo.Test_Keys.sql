USE [TG_Test_Bot]
GO

/****** Объект: Table [dbo].[Test_Keys] Дата скрипта: 11.10.2022 18:05:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Test_Keys];


GO
CREATE TABLE [dbo].[Test_Keys] (
    [ID]               INT           NOT NULL,
    [Test_Key]         NCHAR (32)    NULL,
    [Test_ID]          INT           NOT NULL,
    [User_ID]          BIGINT        NULL,
    [Date_Enable_Key]  SMALLDATETIME NOT NULL,
    [Date_Disable_Key] SMALLDATETIME NOT NULL,
    [Tryes]            TINYINT       NOT NULL,
    [Enabled]          BIT           NOT NULL
);


