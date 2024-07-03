/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_GetCategoriesByThemeId') is not null
begin
	drop procedure p_GetCategoriesByThemeId
end
go
/* =======================================================================================
	Description:	Mostra os nomes das Categorias de um Tema
======================================================================================= */

CREATE PROCEDURE p_GetCategoriesByThemeId
    @ThemeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT idCategory, NameCategory 
    FROM Categories 
    WHERE idTheme = @ThemeId AND Active = 1;
END
