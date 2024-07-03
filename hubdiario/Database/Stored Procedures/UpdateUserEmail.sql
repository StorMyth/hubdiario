/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/
if object_id('p_UpdateUserEmail') is not null
begin
	drop procedure p_UpdateUserEmail
end
go
/* =======================================================================================
	Description:	Atualização do email do utilizador
======================================================================================= */

CREATE PROCEDURE [dbo].[p_UpdateUserEmail]
    @idUser INT,
    @NewEmail NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verifica se o novo email já existe para outro utilizador
    IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE EmailUser = @NewEmail AND idUser <> @idUser)
    BEGIN
        -- Retorna erro com mensagem a indicar que o email já está em uso
        RAISERROR('Email já está em uso por outro utilizador.', 16, 1);
        RETURN;
    END

    -- Atualiza o email se não existir duplicata
    UPDATE [dbo].[Users]
    SET EmailUser = @NewEmail,
        UpdatedAt = GETDATE()
    WHERE idUser = @idUser;
END
