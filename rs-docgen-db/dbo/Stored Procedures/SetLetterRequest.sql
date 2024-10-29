CREATE procedure [dbo].[SetLetterRequest](@Action varchar(25),

    @requestxml varchar(max),

    @requesttype varchar(250),

    @createddate datetime,

    @status varchar(25),

    @processing int,

    @errors varchar(300),
    @apikey uniqueidentifier = null,
    @batchid uniqueidentifier = null,

    @requestid uniqueidentifier = null,

    @newid uniqueidentifier output)

as

begin



    set nocount on



    --declare @apikey uniqueidentifier



    if @action = 'ADDNEWREQUEST' and len(@requestxml) > 0 and @requesttype != '' begin



        -- select @apikey = apikey
        -- from dbo.template
        -- where templatecode = @requesttype



        DECLARE @req_newid table([newrequestid] [uniqueidentifier]);

        insert dbo.letterqueue
            (requestxml, requesttype, processing, createddate, status, errors, apikey, batchid)

        OUTPUT INSERTED.[requestid] INTO @req_newid

        values
            (@requestxml, @requesttype, @processing, @createddate, @status, @errors, @apikey, @batchid)

        select @newid = newrequestid
        from @req_newid

    end



    if @action = 'SETASREADY' and @requestid is not null begin

        update dbo.letterqueue

                                                set processing = 0

                                where requestid = @requestid

    end

end