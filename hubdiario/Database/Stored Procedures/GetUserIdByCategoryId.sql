/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_GetUserIdByCategoryId') is not null
begin
	drop procedure p_GetUserIdByCategoryId
end
go
/* =======================================================================================
	Description:	Seleciona o ID do utilizador com base no ID da categoria
======================================================================================= */

CREATE PROCEDURE p_GetUserIdByCategoryId
    @CategoryId INT,
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @UserId = u.idUser 
    FROM Users u
    JOIN Themes t ON u.idUser = t.idUser
    JOIN Categories c ON t.idTheme = c.idTheme
    WHERE c.idCategory = @CategoryId;
END
