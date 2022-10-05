CREATE TABLE [dbo].[Users] (
    [Id]           INT        NOT NULL,
    [Firstname]    NCHAR (32) NULL,
    [Lastname]     NCHAR (32) NULL,
    [TG_Id]        BIGINT     NOT NULL,
    [Override_Key] BIT        DEFAULT ((0)) NOT NULL,
    [Author]       BIT        DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);