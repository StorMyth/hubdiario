/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/
if object_id('p_UpdateUserEmail') is not null
begin
	drop procedure p_UpdateUserEmail
end
go
/* =======================================================================================
	Description:	Atualiza��o do email do utilizador
======================================================================================= */

CREATE PROCEDURE [dbo].[p_UpdateUserEmail]
    @idUser INT,
    @NewEmail NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verifica se o novo email j� existe para outro utilizador
    IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE EmailUser = @NewEmail AND idUser <> @idUser)
    BEGIN
        -- Retorna erro com mensagem a indicar que o email j� est� em uso
        RAISERROR('Email j� est� em uso por outro utilizador.', 16, 1);
        RETURN;
    END

    -- Atualiza o email se n�o existir duplicata
    UPDATE [dbo].[Users]
    SET EmailUser = @NewEmail,
        UpdatedAt = GETDATE()
    WHERE idUser = @idUser;
END
