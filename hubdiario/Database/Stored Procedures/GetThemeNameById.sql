/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_GetThemeNameById') is not null
begin
	drop procedure p_GetThemeNameById
end
go
/* =======================================================================================
	Description:	Mostra o nome do Tema de um ID
======================================================================================= */

CREATE PROCEDURE p_GetThemeNameById
    @ThemeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT NameTheme 
    FROM Themes
    WHERE idTheme = @ThemeId;
END
