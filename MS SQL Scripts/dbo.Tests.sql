CREATE TABLE [dbo].[Tests] (
    [Id]           INT            NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [Time_to_Take] TIME (0)       NOT NULL,
    [Author_ID]    INT            NOT NULL,
    [Test_Key]     CHAR (32)      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);