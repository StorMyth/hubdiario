/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_UpdateUserPassword') is not null
begin
	drop procedure p_UpdateUserPassword
end
go
/* =======================================================================================
	Description:	Atualização da palavra-passe
======================================================================================= */
CREATE PROCEDURE p_UpdateUserPassword
    @idUser INT,
    @CurrentPassword NVARCHAR(100),
    @NewPassword NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verifica se a palavra-passe atual está correta
    IF EXISTS (SELECT 1 FROM Users WHERE idUser = @idUser AND PasswordUser = @CurrentPassword)
    BEGIN
        -- Atualiza a palavra-passe
        UPDATE Users
        SET PasswordUser = @NewPassword, UpdatedAt = GETDATE()
        WHERE idUser = @idUser;

        RETURN;
    END
    ELSE
    BEGIN
        -- Retorna erro se a palavra-passe atual estiver incorreta
        RAISERROR('A palavra-passe atual está incorreta.', 16, 1);
        RETURN;
    END
END
GO
