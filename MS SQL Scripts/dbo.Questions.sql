CREATE TABLE [dbo].[Questions]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Test_Id] INT NOT NULL,
    [Question] TEXT NOT NULL, 
    [Default_Rate] TINYINT NOT NULL DEFAULT 1
    
)
