/*#######################################################################################*/
/*  STORED PROCEDURES  */
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
