IF OBJECT_ID('USP_NextInvoiceNo', 'P') IS NOT NULL
DROP PROC USP_NextInvoiceNo
GO

CREATE PROC USP_NextInvoiceNo
AS
BEGIN
    DECLARE @i_InvNo int
    SELECT @i_InvNo = NextNumber from InvoiceNo;
    
    set @i_InvNo = @i_InvNo + 1;
    update  InvoiceNo set NextNumber = @i_InvNo, Updatedon = GETDATE();
    select @i_InvNo AS InvoiceNo;
END
GO