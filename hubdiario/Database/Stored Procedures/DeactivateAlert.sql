/*#######################################################################################*/
/*  STORED PROCEDURES  */
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
