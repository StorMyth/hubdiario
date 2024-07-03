/*#######################################################################################*/
/*  STORED PROCEDURES  */
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
