/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_GetItemsByCategoryId') is not null
begin
	drop procedure p_GetItemsByCategoryId
end
go
/* =======================================================================================
	Description:	Seleciona os itens com base no ID da categoria
======================================================================================= */

CREATE PROCEDURE p_GetItemsByCategoryId
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT idItem, TextItem, Quantity, DateItem, Price 
    FROM Itens
    WHERE idCategory = @CategoryId;
END
