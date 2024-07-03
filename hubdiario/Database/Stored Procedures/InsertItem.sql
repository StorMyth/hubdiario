/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_InsertItem') is not null
begin
	drop procedure p_InsertItem
end
go
/* =======================================================================================
	Description:	Insere Item
======================================================================================= */

CREATE PROCEDURE p_InsertItem
    @CategoryId INT,
    @TextItem NVARCHAR(MAX),
    @Quantity INT = NULL,
    @DateItem DATETIME = NULL,
    @Price DECIMAL(18, 2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Itens (idCategory, TextItem, Quantity, DateItem, Price, CreatedAt, UpdatedAt, Active)
    VALUES (@CategoryId, @TextItem, @Quantity, @DateItem, @Price, GETDATE(), GETDATE(), 1);
END
