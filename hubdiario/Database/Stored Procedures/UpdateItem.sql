/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_UpdateItem') is not null
begin
	drop procedure p_UpdateItem
end
go
/* =======================================================================================
	Description:	Atualiza Item
======================================================================================= */

CREATE PROCEDURE p_UpdateItem
    @ItemId INT,
    @TextItem NVARCHAR(MAX),
    @Quantity INT = NULL,
    @DateItem DATETIME = NULL,
    @Price DECIMAL(18, 2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Itens
    SET TextItem = @TextItem,
        Quantity = @Quantity,
        DateItem = @DateItem,
        Price = @Price,
        UpdatedAt = GETDATE()
    WHERE idItem = @ItemId;
END
