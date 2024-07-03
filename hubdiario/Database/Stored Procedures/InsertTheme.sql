/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/
if object_id('p_InsertTheme') is not null
begin
	drop procedure p_InsertTheme
end
go
/* =======================================================================================
	Description:	Inserir novo Tema
======================================================================================= */

CREATE PROCEDURE [dbo].[p_InsertTheme]
    @NameTheme NVARCHAR(255),
    @idUser INT,
    @idTheme INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verifica se o tema já existe para o utilizador
    IF EXISTS (SELECT 1 FROM Themes WHERE NameTheme = @NameTheme AND idUser = @idUser)
    BEGIN
        RAISERROR('Já existe um tema com este nome.', 16, 1);
        RETURN;
    END

    -- Insere o novo tema
    INSERT INTO Themes (NameTheme, idUser, CreatedAt, UpdatedAt, Active)
    VALUES (@NameTheme, @idUser, GETDATE(), GETDATE(), 1);

    -- Obtém o ID do tema inserido
    SET @idTheme = SCOPE_IDENTITY();
END
GO

