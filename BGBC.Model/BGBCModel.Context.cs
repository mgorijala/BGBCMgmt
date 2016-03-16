﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BGBC.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class BGBCEntities : DbContext
    {
        public BGBCEntities()
            : base("name=BGBCEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ContactForm> ContactForms { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<JobStatu> JobStatus { get; set; }
        public virtual DbSet<LeaseFile> LeaseFiles { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<PasswordReset> PasswordResets { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductOrder> ProductOrders { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<RentAutoPay> RentAutoPays { get; set; }
        public virtual DbSet<RentDue> RentDues { get; set; }
        public virtual DbSet<RentPayment> RentPayments { get; set; }
        public virtual DbSet<TenantReferral> TenantReferrals { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<UserCart> UserCarts { get; set; }
        public virtual DbSet<UserCC> UserCCs { get; set; }
        public virtual DbSet<UserReference> UserReferences { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ZipCode> ZipCodes { get; set; }
        public virtual DbSet<vProductOrder> vProductOrders { get; set; }
        public virtual DbSet<vRentPayment> vRentPayments { get; set; }
    
        public virtual ObjectResult<Nullable<int>> SP_NextInvoiceNo()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("SP_NextInvoiceNo");
        }
    
        public virtual ObjectResult<Order> SP_ProductTrans(Nullable<int> i_UserID, Nullable<int> i_InvoiceID, string v_AccountNumber, string v_AccountType, string v_TransId, string v_StatusCode, string v_StatusText, string v_CustomerIP, string v_Address, string v_Comments, string v_Products)
        {
            var i_UserIDParameter = i_UserID.HasValue ?
                new ObjectParameter("i_UserID", i_UserID) :
                new ObjectParameter("i_UserID", typeof(int));
    
            var i_InvoiceIDParameter = i_InvoiceID.HasValue ?
                new ObjectParameter("i_InvoiceID", i_InvoiceID) :
                new ObjectParameter("i_InvoiceID", typeof(int));
    
            var v_AccountNumberParameter = v_AccountNumber != null ?
                new ObjectParameter("v_AccountNumber", v_AccountNumber) :
                new ObjectParameter("v_AccountNumber", typeof(string));
    
            var v_AccountTypeParameter = v_AccountType != null ?
                new ObjectParameter("v_AccountType", v_AccountType) :
                new ObjectParameter("v_AccountType", typeof(string));
    
            var v_TransIdParameter = v_TransId != null ?
                new ObjectParameter("v_TransId", v_TransId) :
                new ObjectParameter("v_TransId", typeof(string));
    
            var v_StatusCodeParameter = v_StatusCode != null ?
                new ObjectParameter("v_StatusCode", v_StatusCode) :
                new ObjectParameter("v_StatusCode", typeof(string));
    
            var v_StatusTextParameter = v_StatusText != null ?
                new ObjectParameter("v_StatusText", v_StatusText) :
                new ObjectParameter("v_StatusText", typeof(string));
    
            var v_CustomerIPParameter = v_CustomerIP != null ?
                new ObjectParameter("v_CustomerIP", v_CustomerIP) :
                new ObjectParameter("v_CustomerIP", typeof(string));
    
            var v_AddressParameter = v_Address != null ?
                new ObjectParameter("v_Address", v_Address) :
                new ObjectParameter("v_Address", typeof(string));
    
            var v_CommentsParameter = v_Comments != null ?
                new ObjectParameter("v_Comments", v_Comments) :
                new ObjectParameter("v_Comments", typeof(string));
    
            var v_ProductsParameter = v_Products != null ?
                new ObjectParameter("v_Products", v_Products) :
                new ObjectParameter("v_Products", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Order>("SP_ProductTrans", i_UserIDParameter, i_InvoiceIDParameter, v_AccountNumberParameter, v_AccountTypeParameter, v_TransIdParameter, v_StatusCodeParameter, v_StatusTextParameter, v_CustomerIPParameter, v_AddressParameter, v_CommentsParameter, v_ProductsParameter);
        }
    
        public virtual ObjectResult<Order> SP_ProductTrans(Nullable<int> i_UserID, Nullable<int> i_InvoiceID, string v_AccountNumber, string v_AccountType, string v_TransId, string v_StatusCode, string v_StatusText, string v_CustomerIP, string v_Address, string v_Comments, string v_Products, MergeOption mergeOption)
        {
            var i_UserIDParameter = i_UserID.HasValue ?
                new ObjectParameter("i_UserID", i_UserID) :
                new ObjectParameter("i_UserID", typeof(int));
    
            var i_InvoiceIDParameter = i_InvoiceID.HasValue ?
                new ObjectParameter("i_InvoiceID", i_InvoiceID) :
                new ObjectParameter("i_InvoiceID", typeof(int));
    
            var v_AccountNumberParameter = v_AccountNumber != null ?
                new ObjectParameter("v_AccountNumber", v_AccountNumber) :
                new ObjectParameter("v_AccountNumber", typeof(string));
    
            var v_AccountTypeParameter = v_AccountType != null ?
                new ObjectParameter("v_AccountType", v_AccountType) :
                new ObjectParameter("v_AccountType", typeof(string));
    
            var v_TransIdParameter = v_TransId != null ?
                new ObjectParameter("v_TransId", v_TransId) :
                new ObjectParameter("v_TransId", typeof(string));
    
            var v_StatusCodeParameter = v_StatusCode != null ?
                new ObjectParameter("v_StatusCode", v_StatusCode) :
                new ObjectParameter("v_StatusCode", typeof(string));
    
            var v_StatusTextParameter = v_StatusText != null ?
                new ObjectParameter("v_StatusText", v_StatusText) :
                new ObjectParameter("v_StatusText", typeof(string));
    
            var v_CustomerIPParameter = v_CustomerIP != null ?
                new ObjectParameter("v_CustomerIP", v_CustomerIP) :
                new ObjectParameter("v_CustomerIP", typeof(string));
    
            var v_AddressParameter = v_Address != null ?
                new ObjectParameter("v_Address", v_Address) :
                new ObjectParameter("v_Address", typeof(string));
    
            var v_CommentsParameter = v_Comments != null ?
                new ObjectParameter("v_Comments", v_Comments) :
                new ObjectParameter("v_Comments", typeof(string));
    
            var v_ProductsParameter = v_Products != null ?
                new ObjectParameter("v_Products", v_Products) :
                new ObjectParameter("v_Products", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Order>("SP_ProductTrans", mergeOption, i_UserIDParameter, i_InvoiceIDParameter, v_AccountNumberParameter, v_AccountTypeParameter, v_TransIdParameter, v_StatusCodeParameter, v_StatusTextParameter, v_CustomerIPParameter, v_AddressParameter, v_CommentsParameter, v_ProductsParameter);
        }
    
        public virtual int SP_RentCalcSchedule(Nullable<System.DateTime> dT_SELDATE)
        {
            var dT_SELDATEParameter = dT_SELDATE.HasValue ?
                new ObjectParameter("DT_SELDATE", dT_SELDATE) :
                new ObjectParameter("DT_SELDATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_RentCalcSchedule", dT_SELDATEParameter);
        }
    
        public virtual ObjectResult<Payment> SP_RentPayment(Nullable<int> i_UserID, Nullable<int> i_InvoiceID, string v_AccountNumber, string v_AccountType, string v_TransId, string v_StatusCode, string v_StatusText, string v_CustomerIP, Nullable<decimal> d_Total, string v_Comments, string v_Address, string v_RentDueids)
        {
            var i_UserIDParameter = i_UserID.HasValue ?
                new ObjectParameter("i_UserID", i_UserID) :
                new ObjectParameter("i_UserID", typeof(int));
    
            var i_InvoiceIDParameter = i_InvoiceID.HasValue ?
                new ObjectParameter("i_InvoiceID", i_InvoiceID) :
                new ObjectParameter("i_InvoiceID", typeof(int));
    
            var v_AccountNumberParameter = v_AccountNumber != null ?
                new ObjectParameter("v_AccountNumber", v_AccountNumber) :
                new ObjectParameter("v_AccountNumber", typeof(string));
    
            var v_AccountTypeParameter = v_AccountType != null ?
                new ObjectParameter("v_AccountType", v_AccountType) :
                new ObjectParameter("v_AccountType", typeof(string));
    
            var v_TransIdParameter = v_TransId != null ?
                new ObjectParameter("v_TransId", v_TransId) :
                new ObjectParameter("v_TransId", typeof(string));
    
            var v_StatusCodeParameter = v_StatusCode != null ?
                new ObjectParameter("v_StatusCode", v_StatusCode) :
                new ObjectParameter("v_StatusCode", typeof(string));
    
            var v_StatusTextParameter = v_StatusText != null ?
                new ObjectParameter("v_StatusText", v_StatusText) :
                new ObjectParameter("v_StatusText", typeof(string));
    
            var v_CustomerIPParameter = v_CustomerIP != null ?
                new ObjectParameter("v_CustomerIP", v_CustomerIP) :
                new ObjectParameter("v_CustomerIP", typeof(string));
    
            var d_TotalParameter = d_Total.HasValue ?
                new ObjectParameter("d_Total", d_Total) :
                new ObjectParameter("d_Total", typeof(decimal));
    
            var v_CommentsParameter = v_Comments != null ?
                new ObjectParameter("v_Comments", v_Comments) :
                new ObjectParameter("v_Comments", typeof(string));
    
            var v_AddressParameter = v_Address != null ?
                new ObjectParameter("v_Address", v_Address) :
                new ObjectParameter("v_Address", typeof(string));
    
            var v_RentDueidsParameter = v_RentDueids != null ?
                new ObjectParameter("v_RentDueids", v_RentDueids) :
                new ObjectParameter("v_RentDueids", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Payment>("SP_RentPayment", i_UserIDParameter, i_InvoiceIDParameter, v_AccountNumberParameter, v_AccountTypeParameter, v_TransIdParameter, v_StatusCodeParameter, v_StatusTextParameter, v_CustomerIPParameter, d_TotalParameter, v_CommentsParameter, v_AddressParameter, v_RentDueidsParameter);
        }
    
        public virtual ObjectResult<Payment> SP_RentPayment(Nullable<int> i_UserID, Nullable<int> i_InvoiceID, string v_AccountNumber, string v_AccountType, string v_TransId, string v_StatusCode, string v_StatusText, string v_CustomerIP, Nullable<decimal> d_Total, string v_Comments, string v_Address, string v_RentDueids, MergeOption mergeOption)
        {
            var i_UserIDParameter = i_UserID.HasValue ?
                new ObjectParameter("i_UserID", i_UserID) :
                new ObjectParameter("i_UserID", typeof(int));
    
            var i_InvoiceIDParameter = i_InvoiceID.HasValue ?
                new ObjectParameter("i_InvoiceID", i_InvoiceID) :
                new ObjectParameter("i_InvoiceID", typeof(int));
    
            var v_AccountNumberParameter = v_AccountNumber != null ?
                new ObjectParameter("v_AccountNumber", v_AccountNumber) :
                new ObjectParameter("v_AccountNumber", typeof(string));
    
            var v_AccountTypeParameter = v_AccountType != null ?
                new ObjectParameter("v_AccountType", v_AccountType) :
                new ObjectParameter("v_AccountType", typeof(string));
    
            var v_TransIdParameter = v_TransId != null ?
                new ObjectParameter("v_TransId", v_TransId) :
                new ObjectParameter("v_TransId", typeof(string));
    
            var v_StatusCodeParameter = v_StatusCode != null ?
                new ObjectParameter("v_StatusCode", v_StatusCode) :
                new ObjectParameter("v_StatusCode", typeof(string));
    
            var v_StatusTextParameter = v_StatusText != null ?
                new ObjectParameter("v_StatusText", v_StatusText) :
                new ObjectParameter("v_StatusText", typeof(string));
    
            var v_CustomerIPParameter = v_CustomerIP != null ?
                new ObjectParameter("v_CustomerIP", v_CustomerIP) :
                new ObjectParameter("v_CustomerIP", typeof(string));
    
            var d_TotalParameter = d_Total.HasValue ?
                new ObjectParameter("d_Total", d_Total) :
                new ObjectParameter("d_Total", typeof(decimal));
    
            var v_CommentsParameter = v_Comments != null ?
                new ObjectParameter("v_Comments", v_Comments) :
                new ObjectParameter("v_Comments", typeof(string));
    
            var v_AddressParameter = v_Address != null ?
                new ObjectParameter("v_Address", v_Address) :
                new ObjectParameter("v_Address", typeof(string));
    
            var v_RentDueidsParameter = v_RentDueids != null ?
                new ObjectParameter("v_RentDueids", v_RentDueids) :
                new ObjectParameter("v_RentDueids", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Payment>("SP_RentPayment", mergeOption, i_UserIDParameter, i_InvoiceIDParameter, v_AccountNumberParameter, v_AccountTypeParameter, v_TransIdParameter, v_StatusCodeParameter, v_StatusTextParameter, v_CustomerIPParameter, d_TotalParameter, v_CommentsParameter, v_AddressParameter, v_RentDueidsParameter);
        }
    
        public virtual ObjectResult<TenantOutstanding> SP_TenantOutstanding(Nullable<int> i_PropertyID)
        {
            var i_PropertyIDParameter = i_PropertyID.HasValue ?
                new ObjectParameter("i_PropertyID", i_PropertyID) :
                new ObjectParameter("i_PropertyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<TenantOutstanding>("SP_TenantOutstanding", i_PropertyIDParameter);
        }
    }
}
