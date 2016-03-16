use master
go

IF EXISTS(select * from sys.databases where name='BGBC')
DROP DATABASE BGBC
go

create database BGBC
go

USE BGBC
GO

IF OBJECT_ID('Product', 'U') IS NOT NULL
  DROP TABLE Product; 
GO

CREATE TABLE Product(
	ProductID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	Name varchar(255) NULL,
	Description varchar(max) NULL,
	Price decimal(15, 2) NULL,
	isLocal bit not NULL,
	DisOrder int,
	Createdon datetime NULL,
	Updatedon datetime NULL,
	Deletedon datetime NULL
)
GO

IF OBJECT_ID('Properties', 'U') IS NOT NULL
  DROP TABLE Properties; 
GO
CREATE TABLE Properties(
	PropertyID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	UserID int NOT NULL,
	Name varchar(255) NULL,
	Nickname varchar(255) NULL,
	Address varchar(255) NULL,
	Address2 varchar(255) NULL,
	City varchar(255) NULL,
	State varchar(255) NULL,
	Zip varchar(255) NULL,
	RentDueDay smallint NULL,
	GracePeriod smallint NULL,
	Penalty decimal(15, 2) NULL,
	DailyPenalty decimal(15, 2) NULL,
	FinalDueDay smallint,
	LeaseDocument bit not null,
	Createdon datetime NULL,
	Updatedon datetime NULL,
	Deletedon datetime NULL
) 
GO

IF OBJECT_ID('Profile', 'U') IS NOT NULL
  DROP TABLE Profile; 
GO
CREATE TABLE Profile(
	ProfileID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	UserID int NOT NULL,
	AltEmail varchar(255) NULL,
	BillingAddressSame bit not null,
	BillingAddress varchar(255) NULL,
	BillingAddress_2 varchar(255) NULL,
	BillingCty varchar(255) NULL,
	BillingState varchar(255) NULL,
	BillingZip varchar(255) NULL,
	HomePhone varchar(255) NULL,
	WorkPhone varchar(255) NULL,
	MobilePhone varchar(255) NULL,
	FaxNumber varchar(255) NULL,
	SMSOpt bit NULL,
	PaymentMethod varchar(255) NULL,
	PaypalEmail varchar(255) NULL,
	PayoutMailAddress varchar(255) NULL,
	PayoutMailAddress2 varchar(255) NULL,
	PayoutMailCity varchar(255) NULL,
	PayoutMailState varchar(255) NULL,
	PayoutMailZip varchar(255) NULL,
	Createdon datetime NULL,
	Updatedon datetime NULL
)
GO

IF OBJECT_ID('Users', 'U') IS NOT NULL
  DROP TABLE Users; 
GO
CREATE TABLE Users(
	UserID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	FirstName varchar(255) NULL,
	LastName varchar(255) NULL,
	Email varchar(255) NULL,
	Password varchar(255) NULL,
	UserType smallint NOT NULL,
	Createdon datetime NULL,
	Updatedon datetime NULL,
	Deletedon datetime NULL
)
GO

IF OBJECT_ID('UserCart', 'U') IS NOT NULL
  DROP TABLE UserCart; 
GO
CREATE TABLE UserCart(
	ID  int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	ProductID int NOT NULL,
	UserID int NOT NULL,
	Createdon datetime NULL,
	Deletedon datetime NULL
)
GO

IF OBJECT_ID('Tenants', 'U') IS NOT NULL
  DROP TABLE Tenants; 
GO
CREATE TABLE Tenants(
	TenantID  int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	PropertyID int,
	UserID int,
	RentAmount decimal(15, 2) NULL,
	FinalDueDate datetime,
	Deposit decimal(15, 2) NULL,
	DepostDueDate datetime,
	PetDepositDue bit not null,
	PetDeposit decimal(15, 2) NULL,
	PetRentAmount  decimal(15, 2) NULL,
	Rent decimal(15, 2) NULL,
	LeaseDocName varchar(255),
	Createdon datetime NULL,
	Updatedon datetime NULL,
	Deletedon datetime NULL
)
GO

IF OBJECT_ID('LeaseFiles', 'U') IS NOT NULL
  DROP TABLE LeaseFiles; 
GO

CREATE TABLE LeaseFiles(
	ID int  PRIMARY KEY IDENTITY(1,1) NOT NULL,
	TenantID int,
	FileContent varbinary(max),
	ContentType varchar(255),
	Createdon datetime NULL,
	Updatedon datetime NULL
)
GO

IF OBJECT_ID('UserReferences', 'U') IS NOT NULL
  DROP TABLE UserReferences; 
GO
CREATE TABLE UserReferences(
	UserReferenceID  int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	UserID int,
	Name varchar(255) NULL,
	Phone varchar(255) NULL,
	EMail varchar(255) NULL,
	Address varchar(255) NULL,
	Address2 varchar(255) NULL,
	City varchar(255) NULL,
	State varchar(255) NULL,
	Zip varchar(255) NULL,	 
	Relationship varchar(255) NULL,
	Createdon datetime NULL,
	Updatedon datetime NULL
)
GO

IF OBJECT_ID('ContactForm', 'U') IS NOT NULL
  DROP TABLE ContactForm; 
GO
CREATE TABLE ContactForm(
	ContactusID  int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	AccountName varchar(255) NULL,
	Name varchar(255) NULL,
	EMail varchar(255) NULL,
	Phone varchar(255) NULL,
	MessageText varchar(max) null,
	MessageType int not null, -- For Contact us form 1, Contact A Realtor 2
	Createdon datetime NULL,
	Updatedon datetime NULL
)
GO

IF OBJECT_ID('TenantReferral', 'U') IS NOT NULL
  DROP TABLE TenantReferral; 
GO
CREATE TABLE TenantReferral(
	ID  int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	Name varchar(255) NULL,
	Email varchar(255) NULL,
	Phone varchar(255) NULL,
	LandlordName varchar(255) NULL,	
	LandlordPhone varchar(255) NULL,
	LandlordEmail varchar(255) NULL,
	LandlordAddress varchar(255) NULL,
	Createdon datetime NULL,
	Updatedon datetime NULL
)
GO

IF OBJECT_ID('InvoiceNo', 'U') IS NOT NULL
  DROP TABLE InvoiceNo; 
GO

CREATE TABLE InvoiceNo(
	NextNumber int,
	Updatedon datetime NULL
)
GO

IF OBJECT_ID('Orders', 'U') IS NOT NULL
  DROP TABLE Orders; 
GO

CREATE TABLE Orders(
	OrderID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	UserID Int, 
	InvoiceID int not null,
	TransDate datetime not null,
	AccountNumber varchar(50) not NULL,
	AccountType varchar(50) not NULL,
	TransId varchar(50) not NULL,
	StatusCode varchar(50) not NULL,
	StatusText varchar(50) not NULL,
	CustomerIP varchar(50) NULL,
	BillAddress varchar(1024),
	Comments varchar(max),
	Createdon datetime NULL
)
GO

IF OBJECT_ID('ProductOrders', 'U') IS NOT NULL
  DROP TABLE ProductOrders; 
GO

CREATE TABLE ProductOrders(
	ProductOrderID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	OrderID int,
	ProductID int,
	Name varchar(255) NULL,
	Price decimal(15, 2) NULL,	
)
GO

IF OBJECT_ID('Payment', 'U') IS NOT NULL
  DROP TABLE Payment; 
GO

CREATE TABLE Payment(
	PaymentID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	UserID Int,
	InvoiceID int not null,
	TransDate datetime not null,
	AccountNumber varchar(50) not NULL,
	AccountType varchar(50) not NULL,
	TransId varchar(50) not NULL, --Confirm no
	StatusCode varchar(50) not NULL,
	StatusText varchar(50) not NULL,
	CustomerIP varchar(50) NULL,
	BillAddress varchar(1024),
	Total decimal(15, 2) not NULL,
	Comments varchar(max),
	Createdon datetime NULL
)
GO

IF OBJECT_ID('RentPayment', 'U') IS NOT NULL
  DROP TABLE RentPayment; 
GO

CREATE TABLE RentPayment(
	RentPaymentID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	PropertyID int,	
	PaymentID int,
	RentMon int,
	RentYear int,
	Description varchar(255),
	Amount decimal(15, 2) not NULL,
	Type smallint,
	Createdon datetime NULL
)
GO

IF OBJECT_ID('RentDue', 'U') IS NOT NULL
  DROP TABLE RentDue; 
GO

CREATE TABLE RentDue(
	RentDueID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	UserID int,
	PropertyID int,	
	DueMon int,
	DueYear int,
	DueDate datetime,
	DueAmount decimal(15, 2) not NULL,
	Description varchar(50),
	DueStatus bit, -- 1 Pending, 0 Paid
	Createdon datetime NULL
)
GO

IF OBJECT_ID('JobStatus', 'U') IS NOT NULL
  DROP TABLE JobStatus; 
GO

CREATE TABLE JobStatus(
	JobStatusID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	JobType smallint,
	JobName varchar(255),
	RunDate datetime,
	Createdon datetime NULL
)
GO

IF OBJECT_ID('Emails', 'U') IS NOT NULL
  DROP TABLE Emails; 
GO

CREATE TABLE Emails(
	EmailID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	ToAddress varchar(255) NULL,
	Subject varchar(255) NULL,
	Body nvarchar(max) NULL,
	Active bit NOT NULL,
	Createdon datetime NULL,
	Updatedon datetime NULL,
)
GO

IF OBJECT_ID('PasswordReset', 'U') IS NOT NULL
  DROP TABLE PasswordReset; 
GO

CREATE TABLE PasswordReset(
	PasswordResetID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	EmailID varchar(255) NULL,
	Token varchar(255),
	Createdon datetime NULL
)
GO

IF OBJECT_ID('UserCC', 'U') IS NOT NULL
  DROP TABLE UserCC; 
GO

CREATE TABLE UserCC(
	UserCCID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	UserID int,
	PaymentType smallint,
	CCNO varchar(512) NULL,
	ExpMon varchar(512),
	ExpYear varchar(512),
	CVV varchar(512),
	AccountType varchar(512),
	RoutingNo varchar(512),
	AccountNo varchar(512),
	Createdon datetime NULL,
	Updatedon datetime NULL
)
GO

IF OBJECT_ID('RentAutoPay', 'U') IS NOT NULL
  DROP TABLE RentAutoPay; 
GO

CREATE TABLE RentAutoPay(
	RentAutoPayID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	UserID int,
	PaymentType smallint,
	CCNO varchar(512) NULL,
	ExpMon varchar(512),
	ExpYear varchar(512),
	CVV varchar(512),
	AccountType varchar(512),
	RoutingNo varchar(512),
	AccountNo varchar(512),
	Rent decimal(15, 2) not NULL,
	Charges decimal(15, 2) not NULL,
	TotalAmt decimal(15, 2) not NULL,
	Createdon datetime NULL,
	Updatedon datetime NULL
)
GO

IF OBJECT_ID('ZipCode', 'U') IS NOT NULL
  DROP TABLE ZipCode; 
GO

CREATE TABLE ZipCode(
	ZipCodeID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	ZipCode int,
	Createdon datetime NULL
)
GO

ALTER TABLE Properties  WITH CHECK ADD  CONSTRAINT FK_Properties_Users FOREIGN KEY(UserID) REFERENCES Users (UserID)
GO

ALTER TABLE Profile  WITH CHECK ADD  CONSTRAINT FK_Profile_Users FOREIGN KEY(UserID) REFERENCES Users (UserID)
GO

ALTER TABLE Tenants  WITH CHECK ADD  CONSTRAINT FK_Tenants_Properties FOREIGN KEY(PropertyID) REFERENCES Properties (PropertyID)
GO

ALTER TABLE Tenants  WITH CHECK ADD  CONSTRAINT FK_Tenants_Users FOREIGN KEY(UserID) REFERENCES Users (UserID)
GO

ALTER TABLE UserReferences  WITH CHECK ADD  CONSTRAINT FK_UserReferences_Users FOREIGN KEY(UserID) REFERENCES Users (UserID)
GO

ALTER TABLE ProductOrders  WITH CHECK ADD  CONSTRAINT FK_ProductOrders_Orders FOREIGN KEY(OrderID) REFERENCES Orders (OrderID)
GO

ALTER TABLE ProductOrders  WITH CHECK ADD  CONSTRAINT FK_ProductOrders_Product FOREIGN KEY(ProductID) REFERENCES Product (ProductID)
GO

ALTER TABLE Orders  WITH CHECK ADD  CONSTRAINT FK_Orders_User FOREIGN KEY(UserID) REFERENCES Users (UserID)
GO

ALTER TABLE Payment  WITH CHECK ADD  CONSTRAINT FK_Payment_User FOREIGN KEY(UserID) REFERENCES Users (UserID)
GO

ALTER TABLE RentPayment  WITH CHECK ADD  CONSTRAINT FK_RentPayment_Payment FOREIGN KEY(PaymentID) REFERENCES Payment (PaymentID)
GO

ALTER TABLE RentPayment  WITH CHECK ADD  CONSTRAINT FK_RentPayment_Property FOREIGN KEY(PropertyID) REFERENCES Properties (PropertyID)
GO

ALTER TABLE RentDue  WITH CHECK ADD  CONSTRAINT FK_RentDue_User FOREIGN KEY(UserID) REFERENCES Users (UserID)
GO

ALTER TABLE RentDue  WITH CHECK ADD  CONSTRAINT FK_RentDue_Property FOREIGN KEY(PropertyID) REFERENCES Properties (PropertyID)
GO

ALTER TABLE UserCC  WITH CHECK ADD  CONSTRAINT FK_UserCC_User FOREIGN KEY(UserID) REFERENCES Users (UserID)
GO

ALTER TABLE RentAutoPay  WITH CHECK ADD  CONSTRAINT FK_RentAutoPay_User FOREIGN KEY(UserID) REFERENCES Users (UserID)
GO

ALTER TABLE LeaseFiles  WITH CHECK ADD  CONSTRAINT FK_LeaseFiles_Tenant FOREIGN KEY(TenantID) REFERENCES Tenants (TenantID)
GO

ALTER TABLE UserCart  WITH CHECK ADD  CONSTRAINT FK_UserCart_Product FOREIGN KEY(ProductID) REFERENCES Product (ProductID)
GO

ALTER TABLE UserCart  WITH CHECK ADD  CONSTRAINT FK_UserCart_Users FOREIGN KEY(UserID) REFERENCES Users (UserID)
GO

INSERT INTO InvoiceNo values(1, GETDATE());

insert into Users(FirstName, LastName, Email, Password, UserType, Createdon)
values('admin', 'admin', 'admin@test.com', '5U+YoO32mOMi1C0dAp4+Hw==', 0, getdate()) 
insert into [Profile](UserID, BillingAddressSame, Createdon) values(@@IDENTITY, 0, getdate())

insert into Users(FirstName, LastName, Email, Password, UserType, Createdon)
values('owner', 'owner', 'owner@test.com', '5U+YoO32mOMi1C0dAp4+Hw==', 1, getdate()) 
insert into [Profile](UserID, BillingAddressSame, Createdon) values(@@IDENTITY, 0 , getdate())

insert into Users(FirstName, LastName, Email, Password, UserType, Createdon)
values('tenant', 'tenant', 'tenant@test.com', '5U+YoO32mOMi1C0dAp4+Hw==', 2, getdate()) 
insert into [Profile](UserID, BillingAddressSame, Createdon) values(@@IDENTITY,0 , getdate())
 
 
 insert into Product(Name, Description, Price, isLocal, Createdon) values('Rental Applicant Screening: EXECUTIVE Package', 'Our EXECUTIVE package provides you with the criminal, credit, employment, and rental background information you need for decision making. The EXECUTIVE package includes credit and criminal records as well as one of our expert team members dedicated to performing a full employment and rental background check on your behalf.',65,0, getdate())

 
insert into Properties (Name, UserID, Nickname, Address, Address2, City, State, Zip, RentDueDay, GracePeriod, Penalty, DailyPenalty, Createdon, LeaseDocument)
values('Test Property 1', 2, null, 'Address', 'Address2', 'City', 'State', '37201', 1, 2, 3, 4, getdate(), 0);

insert into Tenants(PropertyID, UserID, RentAmount, FinalDueDate, Deposit, PetDeposit, PetDepositDue, Createdon)
values(1, 3, 100, '2020-10-1', 100, 1, 0, GETDATE())

GO
insert into ZipCode values(12345, GETDATE());
GO

/*
insert into Payment(UserID, PropertyID, InvoiceID, TransDate, AccountNumber, AccountType, TransId, StatusCode, StatusText, CustomerIP, Tax, Total, Comments, Createdon)
values(3, 1, 111, GETDATE(), 'XXXXX11', 'Master', '12345678', '1', 'Approved', '192.168.1.1', 1, 100, 'Comments', GETDATE())

insert into RentPayment(PaymentID, RentMon, RentYear, Description, Amount, AmountType, Createdon)
values(1, null, null, 'Advance', 50, 1, GETDATE()) 

insert into RentPayment(PaymentID, RentMon, RentYear, Description, Amount, AmountType, Createdon)
values(1, 1, 2016, 'Jan 2016 Rent', 50, 1, GETDATE()) 

insert into RentDue(UserID, DueMon, DueYear, DueDate, DueAmount, Description, DueStatus, Createdon)
values(3, null, null, getdate(), 100, 'Deposit', 1, getdate())

insert into RentDue(UserID, DueMon, DueYear, DueDate, DueAmount, Description, DueStatus, Createdon)
values(3, 1, 2016, getdate(), 100, 'Pet Deposit', 1, getdate())

begin transaction
exec [dbo].[USP_RentPayment] @i_UserID=3,@i_InvoiceID=4,@v_AccountNumber='XXXX1111',@v_AccountType='Visa',@v_TransId='2250650216',@v_StatusCode='1',@v_StatusText='This transaction has been approved.',@v_CustomerIP='::1',@d_Tax=0,@d_Total=200.00,@v_Comments=NULL,@v_RentDueids='1,2'
select * from Payment
select * from RentPayment
select * from RentDue
rollback tran

*/

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS
         WHERE TABLE_NAME = 'vRentPayment')
   DROP VIEW vRentPayment
GO

CREATE VIEW vRentPayment AS
SELECT R.RentPaymentID ID, 
       PS.PropertyID, 
       P.TransDate, 
       P.TransID, 
       U.UserID        'TenantUserID', 
       U.FirstName     'TenantFirstName', 
       U.LastName      'TenantLastName', 
       P.StatusText, 
       P.Comments, 
       R.Description, 
       R.Amount, 
       UP.FirstName    'OwnerFirstName', 
       UP.LastName     'OwnerLastName', 
       UP.UserID       'OwnerUserID' 
FROM   Payment P 
       INNER JOIN RentPayment R 
               ON P.PaymentID = R.PaymentID 
       INNER JOIN Users U 
               ON U.UserID = P.UserID 
       INNER JOIN Properties PS 
               ON PS.PropertyID = R.PropertyID 
       INNER JOIN Users UP 
               ON UP.UserID = PS.UserID
GO
IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS
         WHERE TABLE_NAME = 'vProductOrders')
   DROP VIEW vProductOrders
GO

CREATE VIEW vProductOrders AS
SELECT PO.ProductOrderID,
	   O.OrderID,
	   P.ProductID,
	   U.UserID,
	   O.TransId,
       O.TransDate, 
       U.FirstName  'CustomerFName',
       U.LastName 'CustomerLName', 
       U.UserType,
       P.Name, 
       P.Description,       
		PO.Price,
       O.Comments
FROM   Orders AS O
       INNER JOIN ProductOrders AS PO 
               ON O.OrderID = PO.OrderId 
       INNER JOIN Product AS P
               ON P.ProductID = PO.ProductID 
       INNER JOIN Users AS U
               ON U.UserID = O.UserID 
GO         
