/*#######################################################################################*/
/*  STORED PROCEDURES  */
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
