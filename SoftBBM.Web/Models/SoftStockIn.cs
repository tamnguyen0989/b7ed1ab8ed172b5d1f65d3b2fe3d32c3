//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftBBM.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SoftStockIn
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SoftStockIn()
        {
            this.SoftNotifications = new HashSet<SoftNotification>();
            this.SoftStockInDetails = new HashSet<SoftStockInDetail>();
        }
    
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<long> Total { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string StatusId { get; set; }
        public Nullable<int> FromBranchId { get; set; }
        public Nullable<int> ToBranchId { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public string FromBranchStatusId { get; set; }
        public string ToBranchStatusId { get; set; }
        public string SupplierStatusId { get; set; }
        public Nullable<System.DateTime> StockInDate { get; set; }
        public Nullable<System.DateTime> StockOutDate { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        public Nullable<int> PaymentMethodId { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public Nullable<int> PaymentStatusId { get; set; }
        public string FromSuppliers { get; set; }
    
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ApplicationUser ApplicationUser1 { get; set; }
        public virtual SoftBranch SoftBranch { get; set; }
        public virtual SoftBranch SoftBranch1 { get; set; }
        public virtual SoftBranch SoftBranch2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SoftNotification> SoftNotifications { get; set; }
        public virtual SoftStockInCategory SoftStockInCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SoftStockInDetail> SoftStockInDetails { get; set; }
        public virtual SoftStockInStatu SoftStockInStatu { get; set; }
        public virtual SoftStockInStatu SoftStockInStatu1 { get; set; }
        public virtual SoftStockInStatu SoftStockInStatu2 { get; set; }
        public virtual SoftStockInStatu SoftStockInStatu3 { get; set; }
        public virtual SoftSupplier SoftSupplier { get; set; }
        public virtual SoftStockInPaymentMethod SoftStockInPaymentMethod { get; set; }
        public virtual SoftStockInPaymentType SoftStockInPaymentType { get; set; }
        public virtual SoftStockInPaymentStatus SoftStockInPaymentStatus { get; set; }
    }
}
