/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_AuthenticateUser') is not null
begin
	drop procedure p_AuthenticateUser
end
go
/* =======================================================================================
	Description:	Autenticação do utilizador
======================================================================================= */
CREATE PROCEDURE [dbo].[p_AuthenticateUser]
    @EmailUser NVARCHAR(255),
    @PasswordUser NVARCHAR(255)
AS
BEGIN
	set nocount on;
	declare @acesso int = (select count(*) from Users where EmailUser = @EmailUser AND PasswordUser = @PasswordUser AND Active = 1)
	if (@acesso > 0)
		begin
			select cast(1 as bit)
		end
	else
		begin
			select cast(0 as bit)
		end


END
go
