/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_GetCategoryNameById') is not null
begin
	drop procedure p_GetCategoryNameById
end
go
/* =======================================================================================
	Description:	Seleciona o nome da categoria com base no ID da categoria
======================================================================================= */

CREATE PROCEDURE p_GetCategoryNameById
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT NameCategory 
    FROM Categories
    WHERE idCategory = @CategoryId;
END
