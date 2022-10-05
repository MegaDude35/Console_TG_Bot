CREATE TABLE [dbo].[Test_Keys] (
    [Id]               INT           NOT NULL,
    [Test_Key]         NCHAR (32)    NULL,
    [Id_Test]          INT           NOT NULL,
    [Id_User]          INT           NULL,
    [Date_Enable_Key]  SMALLDATETIME NOT NULL,
    [Date_Disable_Key] SMALLDATETIME NOT NULL,
    [Tryes]            TINYINT       DEFAULT ((1)) NOT NULL,
    [Enabled]          BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);