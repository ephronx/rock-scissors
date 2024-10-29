-- dbo.getuser 'BYUSERNAME', 'shane.aboyne@gmail.com'

CREATE procedure [dbo].[getuser](@Action varchar(25) = '', @ApiKey uniqueidentifier = null, @username varchar(100) = null)
as
begin

	if @Action = 'BYUSERNAME' and @username is not null begin
		select * from dbo.[user]
		 where username = @username
	end

	if @Action = 'BYAPIKEY' and @ApiKey is not null  begin
		select apikey as api_key from dbo.[group]
		 where apikey = @ApiKey
	end

end