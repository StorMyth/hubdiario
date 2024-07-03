/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/
if object_id('p_InsertCategory') is not null
begin
	drop procedure p_InsertCategory
end
go
/* =======================================================================================
	Description:	Inserir uma nova categoria
======================================================================================= */

CREATE PROCEDURE [dbo].[p_InsertCategory]
    @idTheme INT,
    @NameCategory NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verifica se já existe uma categoria com o mesmo nome no tema
    IF NOT EXISTS (SELECT 1 FROM Categories WHERE idTheme = @idTheme AND NameCategory = @NameCategory)
    BEGIN
        INSERT INTO Categories (idTheme, NameCategory, CreatedAt, UpdatedAt, Active)
        VALUES (@idTheme, @NameCategory, GETDATE(), GETDATE(), 1);
    END
    ELSE
    BEGIN
        RAISERROR('Já existe uma categoria com este nome no tema.', 16, 1);
		RETURN;
    END
END
GO
