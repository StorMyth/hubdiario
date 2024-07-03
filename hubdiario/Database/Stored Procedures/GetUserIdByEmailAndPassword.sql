/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_GetUserIdByEmail') is not null
begin
	drop procedure p_GetUserIdByEmail
end
go
/* =======================================================================================
	Description:	Obter o id do utilizador
======================================================================================= */

CREATE PROCEDURE [dbo].[p_GetUserIdByEmail]
    @EmailUser NVARCHAR(255),
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @UserId = idUser
    FROM Users
    WHERE emailUser = @EmailUser;
END

