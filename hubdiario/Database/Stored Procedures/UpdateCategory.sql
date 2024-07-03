/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/
if object_id('p_UpdateCategory') is not null
begin
	drop procedure p_UpdateCategory
end
go
/* =======================================================================================
	Description:	Atualização do nome da categoria
======================================================================================= */

CREATE PROCEDURE [dbo].[p_UpdateCategory]
    @idCategory INT,
    @NameCategory NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @idTheme INT;

    -- Obtém o id do tema associado à categoria
    SELECT @idTheme = idTheme FROM Categories WHERE idCategory = @idCategory;

    -- Verifica se já existe outra categoria com o mesmo nome no tema
    IF NOT EXISTS (SELECT 1 FROM Categories WHERE idTheme = @idTheme AND NameCategory = @NameCategory AND idCategory != @idCategory)
    BEGIN
        UPDATE Categories
        SET NameCategory = @NameCategory,
            UpdatedAt = GETDATE()
        WHERE idCategory = @idCategory;
    END
    ELSE
    BEGIN
        RAISERROR('Já existe uma categoria com este nome no tema.', 16, 1);
		RETURN;
    END
END
GO
