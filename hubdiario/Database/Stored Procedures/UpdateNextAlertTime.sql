/*#######################################################################################*/
/*  STORED PROCEDURES  */
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
