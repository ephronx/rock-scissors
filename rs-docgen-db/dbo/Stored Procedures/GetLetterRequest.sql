CREATE procedure dbo.GetLetterRequest(@Action varchar(25), @requestId uniqueidentifier)
as
begin
	if @Action = 'GETREQUESTSTATUS' begin
		SELECT [requestid]
			  ,[createddate]
			  ,[requesttype]
			  ,[processing]
			  ,[startdate]
			  ,[enddate]
			  ,[status]
			  ,[errors]
			  ,[outputlocation]
			  ,[batchid]
			  ,[apikey]
		  FROM [dbo].[letterqueue]
		where requestid = @requestid
	end
end