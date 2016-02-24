IF OBJECT_ID('USP_ProductTrans', 'P') IS NOT NULL
DROP PROC USP_ProductTrans
GO

CREATE PROC USP_ProductTrans(
@i_UserID int,
@i_InvoiceID int,
@v_AccountNumber varchar,
@v_AccountType varchar(50),
@v_TransId varchar(50),
@v_StatusCode varchar(50),
@v_StatusText varchar(50),
@v_CustomerIP varchar(50),
@v_Address  varchar(max),
@v_Comments varchar(max),
@v_Products varchar(255))
AS
BEGIN
     declare @i_orderid int;
     
     insert into Orders(UserID, InvoiceID, TransDate, AccountNumber, AccountType, TransId, StatusCode, StatusText, CustomerIP, BillAddress, Comments, Createdon)
     values(@i_UserID, @i_InvoiceID, GETDATE(), @v_AccountNumber, @v_AccountType, @v_TransId, @v_StatusCode, @v_StatusText, @v_CustomerIP, @v_Address, @v_Comments, GETDATE())
     
     set @i_orderid = @@identity
     
      insert into ProductOrders(OrderID, ProductID, Name, Price)
     select @i_orderid, A.ProductID, A.Name, A.Price from Product  A
     inner join (
     
     SELECT   @i_orderid OrderID,
     CAST(Split.a.value('.', 'VARCHAR(100)') AS int) ProductID  
 FROM  (SELECT    
         CAST ('<M>' + REPLACE(@v_Products, ',', '</M><M>') + '</M>' AS XML) AS String  
     ) AS A CROSS APPLY String.nodes ('/M') AS Split(a)
     
     ) as B on A.ProductID = B.ProductID
     
     select * from Orders where OrderID = @i_orderid
     
END
GO
