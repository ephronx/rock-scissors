CREATE TABLE [dbo].[config_settings] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [code]        VARCHAR (10)  NOT NULL,
    [description] VARCHAR (100) NOT NULL,
    [value]       VARCHAR (100) NOT NULL,
    [type]        VARCHAR (50)  NOT NULL
);
GO

ALTER TABLE [dbo].[config_settings]
    ADD CONSTRAINT [config_settings_pkey] PRIMARY KEY CLUSTERED ([id] ASC);
GO

