/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_DeleteUserAccount') is not null
begin
	drop procedure p_DeleteUserAccount
end
go
/* =======================================================================================
	Description:	Eliminação da conta do utilizador
======================================================================================= */
CREATE PROCEDURE [dbo].[p_DeleteUserAccount]
    @idUser INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Exclui os alertas do utilizador
        DELETE FROM Alerts WHERE idUser = @idUser;

        -- Exclui os itens das categorias dos temas do utilizador
        DELETE I
        FROM Itens I
        INNER JOIN Categories C ON I.idCategory = C.idCategory
        INNER JOIN Themes T ON C.idTheme = T.idTheme
        WHERE T.idUser = @idUser;

        -- Exclui as categorias dos temas do utilizador
        DELETE C
        FROM Categories C
        INNER JOIN Themes T ON C.idTheme = T.idTheme
        WHERE T.idUser = @idUser;

        -- Exclui os temas do utilizador
        DELETE FROM Themes WHERE idUser = @idUser;

        -- Exclui o utilizador da tabela Users
        DELETE FROM Users WHERE idUser = @idUser;

        -- Caso haja necessidade de verificar se a operação foi bem-sucedida
        IF @@ROWCOUNT = 0
        BEGIN
            -- Nenhuma linha foi afetada, o utilizador não existe
            RAISERROR('Utilizador não encontrado.', 16, 1);
        END
    END TRY
    BEGIN CATCH
        -- Captura de erros
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        -- Lança o erro para ser capturado no código C#
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END

