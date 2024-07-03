/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_DeleteThemeById') is not null
begin
	drop procedure p_DeleteThemeById
end
go
/* =======================================================================================
	Description:	Apaga o tema com base no ID
======================================================================================= */

CREATE PROCEDURE p_DeleteThemeById
    @idTheme INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Themes 
    WHERE idTheme = @idTheme;
END
