/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/
if object_id('p_GetUserThemes') is not null
begin
	drop procedure p_GetUserThemes
end
go
/* =======================================================================================
	Description:	Procura os temas de um utilizador
======================================================================================= */

CREATE PROCEDURE [dbo].[p_GetUserThemes]
    @EmailUser NVARCHAR(255)
AS
BEGIN
    SELECT 
        t.idTheme,
        t.NameTheme,
        t.CreatedAt AS ThemeCreatedAt,
        t.UpdatedAt AS ThemeUpdatedAt,
        t.Active AS ThemeEnable
    FROM 
        [dbo].[Users] u
    JOIN 
        [dbo].[Themes] t ON u.idUser = t.idUser
    WHERE
        u.EmailUser = @EmailUser;
END
GO

