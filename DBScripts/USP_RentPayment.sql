IF OBJECT_ID('USP_RentPayment', 'P') IS NOT NULL
DROP PROC USP_RentPayment
GO

CREATE PROC USP_RentPayment(
	@i_UserID int,
	@i_InvoiceID int,
	@v_AccountNumber varchar,
	@v_AccountType varchar(50),
	@v_TransId varchar(50),
	@v_StatusCode varchar(50),
	@v_StatusText varchar(50),
	@v_CustomerIP varchar(50),
	@d_Total decimal(15, 2),
	@v_Comments varchar(max),
	@v_Address  varchar(max),
	@v_RentDueids varchar(255))
AS
BEGIN
	DECLARE @i_PaymentID int
	DECLARE @RentDuePay TABLE(id int)

	INSERT INTO Payment (UserID, InvoiceID, TransDate, AccountNumber, AccountType, TransId, StatusCode, StatusText, CustomerIP, BillAddress, Total, Comments, Createdon)
	values(@i_UserID, @i_InvoiceID, GETDATE(), @v_AccountNumber, @v_AccountType, @v_TransId, @v_StatusCode, @v_StatusText, @v_CustomerIP, @v_Address, @d_Total, @v_Comments, GETDATE())

	set @i_PaymentID = @@identity
     

	INSERT INTO @RentDuePay 
				(id)
	SELECT id FROM ( 
	SELECT Cast(split.a.value('.', 'VARCHAR(100)') AS INT) id 
	FROM   (SELECT Cast ('<M>' + Replace(@v_RentDueids, ',', '</M><M>') + '</M>' AS XML) AS 
				   String) 
		   AS A 
		   CROSS apply string.nodes ('/M') AS Split(a) 
	) AS TT WHERE TT.id > 0
	
	INSERT INTO RentPayment 
				(PaymentID, 
				 PropertyID,
				 RentMon, 
				 RentYear, 
				 Description, 
				 Amount, 
				 Createdon) 
	SELECT @i_PaymentID,
		   PropertyID,
		   DueMon, 
		   DueYear, 
		   Description, 
		   DueAmount, 
		   Getdate() 
	FROM   RentDue P 
		   INNER JOIN @RentDuePay AS T 
				   ON P.RentDueID = T.id 

	UPDATE R 
		SET R.DueStatus = 0
	FROM RentDue AS R 
	INNER JOIN @RentDuePay AS T 
		ON R.RentDueID = T.ID
	
	SELECT * 
	FROM   Payment WHERE PaymentID = @i_PaymentID
     
END
GO
