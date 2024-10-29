CREATE TABLE [dbo].[hostheartbeat] (
    [heartbeatid] INT           IDENTITY (1, 1) NOT NULL,
    [hostname]    VARCHAR (250) NOT NULL,
    [aliveat]     DATETIME      NOT NULL,
    CONSTRAINT [PK_hostheartbeat] PRIMARY KEY CLUSTERED ([heartbeatid] ASC)
);

