/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_DeleteItem') is not null
begin
	drop procedure p_DeleteItem
end
go
/* =======================================================================================
	Description:	Apaga um Item com base no ID do item
======================================================================================= */

CREATE PROCEDURE p_DeleteItem
    @ItemId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Itens
    WHERE idItem = @ItemId;
END
