/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_DeleteCategory') is not null
begin
	drop procedure p_DeleteCategory
end
go
/* =======================================================================================
	Description:	Apaga uma categoria
======================================================================================= */

CREATE PROCEDURE p_DeleteCategory
    @idCategory INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Categories
    WHERE idCategory = @idCategory;
END
