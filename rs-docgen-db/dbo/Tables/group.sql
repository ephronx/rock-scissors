CREATE TABLE [dbo].[group] (
    [group_id]   INT              IDENTITY (1, 1) NOT NULL,
    [group_name] VARCHAR (200)    NOT NULL,
    [apikey]     UNIQUEIDENTIFIER NULL
);



