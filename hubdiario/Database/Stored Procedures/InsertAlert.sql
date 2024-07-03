/*#######################################################################################*/
/*  STORED PROCEDURES  */
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
