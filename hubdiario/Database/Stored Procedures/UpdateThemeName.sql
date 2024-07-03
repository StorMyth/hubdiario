/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/
if object_id('p_UpdateThemeName') is not null
begin
	drop procedure p_UpdateThemeName
end
go
/* =======================================================================================
	Description:	Atualiza o nome do tema
======================================================================================= */

CREATE PROCEDURE [dbo].[p_UpdateThemeName]
    @idTheme INT,
    @newName NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @idUser INT;

    -- Obtém o idUser com base no idTheme
    SELECT @idUser = idUser
    FROM Themes
    WHERE idTheme = @idTheme;

    -- Verifica se o novo nome do tema já existe para o utilizador
    IF EXISTS (SELECT 1 FROM Themes WHERE NameTheme = @newName AND idUser = @idUser AND idTheme <> @idTheme)
    BEGIN
        RAISERROR('Já existe um tema com este nome.', 16, 1);
        RETURN;
    END

    -- Atualiza o nome do tema
    UPDATE [dbo].[Themes]
    SET NameTheme = @newName,
        UpdatedAt = GETDATE()
    WHERE idTheme = @idTheme;
END
GO
