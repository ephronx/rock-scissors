CREATE TABLE [dbo].[batch] (
    [batchid]        UNIQUEIDENTIFIER NOT NULL,
    [createddate]    DATETIME         NOT NULL,
    [processing]     BIT              NOT NULL,
    [startdate]      DATETIME         NULL,
    [enddate]        DATETIME         NULL,
    [status]         VARCHAR (50)     NOT NULL,
    [apikey]         UNIQUEIDENTIFIER NOT NULL,
    [requestcount]   INT              NULL,
    [completedcount] INT              NULL,
    [failedcount]    INT              NULL,
    CONSTRAINT [PK_batch] PRIMARY KEY CLUSTERED ([batchid] ASC)
);



