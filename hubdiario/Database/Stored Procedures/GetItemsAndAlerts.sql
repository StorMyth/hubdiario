/*#######################################################################################*/
/*  STORED PROCEDURES  */
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
