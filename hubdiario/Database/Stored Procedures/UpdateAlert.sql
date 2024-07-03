/*#######################################################################################*/
/*  STORED PROCEDURES  */
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
