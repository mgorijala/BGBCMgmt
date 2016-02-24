IF Object_id('USP_TenantOutstanding', 'P') IS NOT NULL 
  DROP PROC USP_TenantOutstanding 

go 

CREATE PROC USP_TenantOutstanding(@i_PropertyID int) 
AS 
SET NOCOUNT ON
  BEGIN 
  DECLARE @ttTenants TABLE(id int)
  

	INSERT INTO @ttTenants
	SELECT T.UserID
	FROM Tenants T
	INNER JOIN Properties P ON P.PropertyID = T.PropertyID
	WHERE P.PropertyID = @i_PropertyID
  
	SELECT U.UserID,
		   U.FirstName,
		   U.LastName,
		   U.Email,
		   ISNULL(TT.Balance, 0) Balance
	FROM Users U
	INNER JOIN @ttTenants T1 ON U.UserID = T1.id 
	LEFT OUTER JOIN
	  ( SELECT R.UserID,
			   SUM(R.DueAmount) Balance
	   FROM RentDue R
	   INNER JOIN @ttTenants T ON R.UserID = T.id
	   WHERE R.DueStatus = 1 AND R.PropertyID = @i_PropertyID
	   GROUP BY R.UserID) AS TT ON U.UserID = TT.UserID
	WHERE U.Deletedon is null
        
  END 
SET NOCOUNT OFF        
GO