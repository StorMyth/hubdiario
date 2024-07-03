/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/
if object_id('p_RegisterUser') is not null
begin
	drop procedure p_RegisterUser
end
go
/* =======================================================================================
	Description:	Registo de uma nova conta de utilizador
======================================================================================= */
CREATE PROCEDURE p_RegisterUser
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(100),
    @Success BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @Success = 0;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verifica se o email já está registado
        IF EXISTS (SELECT 1 FROM Users WHERE EmailUser = @Email)
        BEGIN
            -- Retorna erro customizado se o email já existir
            RAISERROR('O email já está registado.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Insere o novo utilizador
        INSERT INTO Users (EmailUser, PasswordUser, CreatedAt, UpdatedAt, Active)
        VALUES (@Email, @PasswordHash, GETDATE(), GETDATE(), 1);

        SET @Success = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END



