CREATE TABLE [dbo].[User_Answers] (
    [Id]         INT NOT NULL,
    [IdTest_Key] INT NOT NULL,
    [IdUser]     INT NOT NULL,
    [IdQuestion] INT NOT NULL,
	[Try] TINYINT NOT NULL
    PRIMARY KEY CLUSTERED ([Id] ASC)
);