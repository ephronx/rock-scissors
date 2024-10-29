CREATE procedure [dbo].[SetBatch]
    (@Action varchar(25),
     @apiKey UNIQUEIDENTIFIER,
     @batchId uniqueidentifier = null)

as

begin

    if @Action = 'NEWBATCH' and @batchId is not null and @apiKey is not null begin
        insert dbo.batch
            (batchid, apiKey, createddate, processing, status, requestcount, completedcount, failedcount)
        values
            (@batchId, @apiKey, getdate(), 0, 'new', 0, 0, 0)
    end

end