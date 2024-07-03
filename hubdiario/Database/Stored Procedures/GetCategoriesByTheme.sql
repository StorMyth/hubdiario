/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/
if object_id('p_GetCategoriesByTheme') is not null
begin
	drop procedure p_GetCategoriesByTheme
end
go
/* =======================================================================================
	Description:	Mostra categorias por Tema
======================================================================================= */

CREATE PROCEDURE p_GetCategoriesByTheme
    @idTheme INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT idCategory, NameCategory
    FROM Categories
    WHERE idTheme = @idTheme;
END
