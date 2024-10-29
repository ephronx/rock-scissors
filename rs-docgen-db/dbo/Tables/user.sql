CREATE TABLE [dbo].[user] (
    [userid]   INT           IDENTITY (1, 1) NOT NULL,
    [username] VARCHAR (200) NOT NULL,
    [active]   BIT           CONSTRAINT [DF_user_active] DEFAULT ((1)) NOT NULL
);

