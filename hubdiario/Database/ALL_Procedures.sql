/*#######################################################################################*/
/*  STORED PROCEDURES  */
/*#######################################################################################*/

if object_id('p_AuthenticateUser') is not null
begin
	drop procedure p_AuthenticateUser
end
go
/* =======================================================================================
	Description:	Autenticação do utilizador
======================================================================================= */
CREATE PROCEDURE [dbo].[p_AuthenticateUser]
    @EmailUser NVARCHAR(255),
    @PasswordUser NVARCHAR(255)
AS
BEGIN
	set nocount on;
	declare @acesso int = (select count(*) from Users where EmailUser = @EmailUser AND PasswordUser = @PasswordUser AND Active = 1)
	if (@acesso > 0)
		begin
			select cast(1 as bit)
		end
	else
		begin
			select cast(0 as bit)
		end
END
go
/*#######################################################################################*/

if object_id('p_DeactivateAlert') is not null
begin
	drop procedure p_DeactivateAlert
end
go
/* =======================================================================================
	Description:	Desativa o alerta
======================================================================================= */
CREATE PROCEDURE [dbo].[p_DeactivateAlert]
    @AlertId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Alerts
    SET Active = 0
    WHERE idAlert = @AlertId;
END
GO
/*#######################################################################################*/
if object_id('p_DeleteCategory') is not null
begin
	drop procedure p_DeleteCategory
end
go
/* =======================================================================================
	Description:	Apaga uma categoria
======================================================================================= */

CREATE PROCEDURE p_DeleteCategory
    @idCategory INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Categories
    WHERE idCategory = @idCategory;
END
go
/*#######################################################################################*/
if object_id('p_DeleteItem') is not null
begin
	drop procedure p_DeleteItem
end
go
/* =======================================================================================
	Description:	Apaga um Item com base no ID do item
======================================================================================= */

CREATE PROCEDURE p_DeleteItem
    @ItemId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Itens
    WHERE idItem = @ItemId;
END
go
/*#######################################################################################*/
if object_id('p_DeleteThemeById') is not null
begin
	drop procedure p_DeleteThemeById
end
go
/* =======================================================================================
	Description:	Apaga o tema com base no ID
======================================================================================= */

CREATE PROCEDURE p_DeleteThemeById
    @idTheme INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Themes 
    WHERE idTheme = @idTheme;
END
go
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
GO
/*#######################################################################################*/

if object_id('p_GetAlertById') is not null
begin
	drop procedure p_GetAlertById
end
go
/* =======================================================================================
	Description:	Seleciona os detalhes do alerta com base no ID do alerta
======================================================================================= */

CREATE PROCEDURE p_GetAlertById
    @AlertId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT idAlert, idCategory, AlertTime, RepeatInterval, RepeatUnit, Active 
    FROM Alerts
    WHERE idAlert = @AlertId;
END
GO
/*#######################################################################################*/

if object_id('p_GetAlertDetails') is not null
begin
	drop procedure p_GetAlertDetails
end
go
/* =======================================================================================
	Description:	Obtem os detalhes do alerta
======================================================================================= */

CREATE PROCEDURE [dbo].[p_GetAlertDetails]
    @AlertId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT RepeatInterval, RepeatUnit
    FROM Alerts
    WHERE idAlert = @AlertId;
END
GO
/*#######################################################################################*/
if object_id('p_GetAlertIdByCategoryId') is not null
begin
	drop procedure p_GetAlertIdByCategoryId
end
go
/* =======================================================================================
	Description:	Seleciona o ID do alerta com base no ID da categoria e do utilizador
======================================================================================= */

CREATE PROCEDURE p_GetAlertIdByCategoryId
    @CategoryId INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT idAlert 
    FROM Alerts
    WHERE idCategory = @CategoryId AND idUser = @UserId;
END
go
/*#######################################################################################*/

if object_id('p_GetAlertsToSend') is not null
begin
	drop procedure p_GetAlertsToSend
end
go
/* =======================================================================================
	Description:	Seleciona os alertas para envio
======================================================================================= */
CREATE PROCEDURE [dbo].[p_GetAlertsToSend]
    @CurrentTime DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT a.idAlert, a.idUser, a.AlertTime, u.EmailUser
    FROM Alerts a
    JOIN Users u ON a.idUser = u.idUser
    WHERE a.AlertTime <= @CurrentTime AND a.Active = 1;
END
GO
/*#######################################################################################*/
if object_id('p_GetCategoriesByTheme') is not null
begin
	drop procedure p_GetCategoriesByTheme
end
go
/* =======================================================================================
	Description:	Mostra categorias por Tema
======================================================================================= */

CREATE PROCEDURE p_GetCategoriesByTheme
    @idTheme INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT idCategory, NameCategory
    FROM Categories
    WHERE idTheme = @idTheme;
END
go
/*#######################################################################################*/

if object_id('p_GetCategoriesByThemeId') is not null
begin
	drop procedure p_GetCategoriesByThemeId
end
go
/* =======================================================================================
	Description:	Mostra os nomes das Categorias de um Tema
======================================================================================= */

CREATE PROCEDURE p_GetCategoriesByThemeId
    @ThemeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT idCategory, NameCategory 
    FROM Categories 
    WHERE idTheme = @ThemeId AND Active = 1;
END
go
/*#######################################################################################*/

if object_id('p_GetCategoryNameById') is not null
begin
	drop procedure p_GetCategoryNameById
end
go
/* =======================================================================================
	Description:	Seleciona o nome da categoria com base no ID da categoria
======================================================================================= */

CREATE PROCEDURE p_GetCategoryNameById
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT NameCategory 
    FROM Categories
    WHERE idCategory = @CategoryId;
END
go
/*#######################################################################################*/

if object_id('p_GetItemsAndAlerts') is not null
begin
	drop procedure p_GetItemsAndAlerts
end
go
/* =======================================================================================
	Description:	Mostra os Itens e Alerta de uma Categoria
======================================================================================= */

CREATE PROCEDURE p_GetItemsAndAlerts
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        i.TextItem, 
        i.Quantity, 
        i.DateItem, 
        i.Price, 
        a.AlertTime, 
        a.RepeatInterval, 
        a.RepeatUnit 
    FROM 
        Itens i
    LEFT JOIN 
        Alerts a ON i.idCategory = a.idCategory AND a.Active = 1 -- Filtra apenas alertas ativos
    WHERE 
        i.idCategory = @CategoryId AND i.Active = 1;
END
go
/*#######################################################################################*/

if object_id('p_GetItemsByCategoryId') is not null
begin
	drop procedure p_GetItemsByCategoryId
end
go
/* =======================================================================================
	Description:	Seleciona os itens com base no ID da categoria
======================================================================================= */

CREATE PROCEDURE p_GetItemsByCategoryId
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT idItem, TextItem, Quantity, DateItem, Price 
    FROM Itens
    WHERE idCategory = @CategoryId;
END
go
/*#######################################################################################*/

if object_id('p_GetThemeNameById') is not null
begin
	drop procedure p_GetThemeNameById
end
go
/* =======================================================================================
	Description:	Mostra o nome do Tema de um ID
======================================================================================= */

CREATE PROCEDURE p_GetThemeNameById
    @ThemeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT NameTheme 
    FROM Themes
    WHERE idTheme = @ThemeId;
END
go
/*#######################################################################################*/

if object_id('p_GetUserIdByCategoryId') is not null
begin
	drop procedure p_GetUserIdByCategoryId
end
go
/* =======================================================================================
	Description:	Seleciona o ID do utilizador com base no ID da categoria
======================================================================================= */

CREATE PROCEDURE p_GetUserIdByCategoryId
    @CategoryId INT,
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @UserId = u.idUser 
    FROM Users u
    JOIN Themes t ON u.idUser = t.idUser
    JOIN Categories c ON t.idTheme = c.idTheme
    WHERE c.idCategory = @CategoryId;
END
go
/*#######################################################################################*/

if object_id('p_GetUserIdByEmail') is not null
begin
	drop procedure p_GetUserIdByEmail
end
go
/* =======================================================================================
	Description:	Obter o id do utilizador
======================================================================================= */

CREATE PROCEDURE [dbo].[p_GetUserIdByEmail]
    @EmailUser NVARCHAR(255),
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @UserId = idUser
    FROM Users
    WHERE emailUser = @EmailUser;
END
go
/*#######################################################################################*/
if object_id('p_GetUserThemes') is not null
begin
	drop procedure p_GetUserThemes
end
go
/* =======================================================================================
	Description:	Procura os temas de um utilizador
======================================================================================= */

CREATE PROCEDURE [dbo].[p_GetUserThemes]
    @EmailUser NVARCHAR(255)
AS
BEGIN
    SELECT 
        t.idTheme,
        t.NameTheme,
        t.CreatedAt AS ThemeCreatedAt,
        t.UpdatedAt AS ThemeUpdatedAt,
        t.Active AS ThemeEnable
    FROM 
        [dbo].[Users] u
    JOIN 
        [dbo].[Themes] t ON u.idUser = t.idUser
    WHERE
        u.EmailUser = @EmailUser;
END
GO
/*#######################################################################################*/

if object_id('p_InsertAlert') is not null
begin
	drop procedure p_InsertAlert
end
go
/* =======================================================================================
	Description:	Insere um Alerta
======================================================================================= */

CREATE PROCEDURE p_InsertAlert
    @UserId INT,
    @CategoryId INT,
    @AlertTime DATETIME,
    @RepeatInterval INT = NULL,
    @RepeatUnit NVARCHAR(50) = NULL,
    @Active BIT,
    @NewAlertId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Alerts (idUser, idCategory, AlertTime, RepeatInterval, RepeatUnit, Active, CreatedAt, UpdatedAt)
    VALUES (@UserId, @CategoryId, @AlertTime, @RepeatInterval, @RepeatUnit, @Active, GETDATE(), GETDATE());

    SELECT @NewAlertId = SCOPE_IDENTITY();
END
GO
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
GO
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
GO
/*#######################################################################################*/

if object_id('p_UpdateAlert') is not null
begin
	drop procedure p_UpdateAlert
end
go
/* =======================================================================================
	Description:	Atualiza um Alerta
======================================================================================= */

CREATE PROCEDURE p_UpdateAlert
    @AlertId INT,
    @AlertTime DATETIME,
    @RepeatInterval INT = NULL,
    @RepeatUnit NVARCHAR(50) = NULL,
    @Active BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Alerts
    SET AlertTime = @AlertTime,
        RepeatInterval = @RepeatInterval,
        RepeatUnit = @RepeatUnit,
        Active = @Active,
        UpdatedAt = GETDATE()
    WHERE idAlert = @AlertId;
END
GO
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
/*#######################################################################################*/

if object_id('p_UpdateItem') is not null
begin
	drop procedure p_UpdateItem
end
go
/* =======================================================================================
	Description:	Atualiza Item
======================================================================================= */

CREATE PROCEDURE p_UpdateItem
    @ItemId INT,
    @TextItem NVARCHAR(MAX),
    @Quantity INT = NULL,
    @DateItem DATETIME = NULL,
    @Price DECIMAL(18, 2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Itens
    SET TextItem = @TextItem,
        Quantity = @Quantity,
        DateItem = @DateItem,
        Price = @Price,
        UpdatedAt = GETDATE()
    WHERE idItem = @ItemId;
END
GO
/*#######################################################################################*/

if object_id('p_UpdateNextAlertTime') is not null
begin
	drop procedure p_UpdateNextAlertTime
end
go
/* =======================================================================================
	Description:	Atualiza para o próximo tempo de alerta
======================================================================================= */
CREATE PROCEDURE [dbo].[p_UpdateNextAlertTime]
    @AlertId INT,
    @NextAlertTime DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Alerts
    SET AlertTime = @NextAlertTime
    WHERE idAlert = @AlertId;
END
GO
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
GO
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
