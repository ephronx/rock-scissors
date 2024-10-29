CREATE TABLE [dbo].[letterqueue] (
    [requestid]      UNIQUEIDENTIFIER CONSTRAINT [DF_letterqueue_requestid] DEFAULT (newsequentialid()) NOT NULL,
    [createddate]    DATETIME         CONSTRAINT [DF_letterqueue_createddate] DEFAULT (getdate()) NOT NULL,
    [requesttype]    VARCHAR (250)    NULL,
    [requestxml]     XML              NOT NULL,
    [processing]     INT              CONSTRAINT [DF__letterque__proce__6E01572D] DEFAULT ((0)) NOT NULL,
    [startdate]      DATETIME         NULL,
    [enddate]        DATETIME         NULL,
    [status]         VARCHAR (25)     CONSTRAINT [DF__letterque__statu__6EF57B66] DEFAULT ('new') NULL,
    [errors]         VARCHAR (350)    NULL,
    [processinghost] VARCHAR (200)    NULL,
    [outputlocation] VARCHAR (350)    NULL,
    [priority]       INT              NULL,
    [apikey]         UNIQUEIDENTIFIER NULL,
    [dataxml]        XML              NULL,
    [batchid]        UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_letterqueue] PRIMARY KEY CLUSTERED ([requestid] ASC)
);






GO


