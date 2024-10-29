CREATE TABLE [dbo].[template] (
    [templateId]           UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL,
    [apikey]               UNIQUEIDENTIFIER CONSTRAINT [DF_template_apikey] DEFAULT (newid()) NULL,
    [internaltemplatename] VARCHAR (200)    NOT NULL,
    [templatecode]         VARCHAR (120)    NOT NULL,
    [category]             VARCHAR (100)    NULL,
    [active]               BIT              NOT NULL,
    [createddate]          DATETIME         NOT NULL,
    [enddate]              DATETIME         NULL,
    [defaultpriority]      INT              CONSTRAINT [DEFAULT_template_defaultpriority] DEFAULT ((1)) NOT NULL,
    [templatedoc]          VARBINARY (MAX)  NULL,
    [version]              INT              CONSTRAINT [DF_template_version] DEFAULT ((1)) NOT NULL,
    [datahost]             VARCHAR (500)    NULL,
    [hosttype]             VARCHAR (100)    NULL,
    [db_name]              VARCHAR (200)    NULL,
    [sp_name]              VARCHAR (200)    NULL,
    [params]               VARCHAR (200)    NULL,
    CONSTRAINT [PK_CorrespondenceTemplate] PRIMARY KEY CLUSTERED ([templateId] ASC),
    UNIQUE NONCLUSTERED ([templateId] ASC)
);




GO
