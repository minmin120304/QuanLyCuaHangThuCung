using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangThuCung.Models
{
    [Table("Bill")]
    public class Bill : IHasID
    {
        [Key]
        [StringLength(10)]
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string EmployeeId { get; set; }
        public string CustomerId { get; set; }
        //[Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string State { get; set; }

        //[Column(TypeName = "decimal(18,2)")]
        public decimal? VAT { get; set; }

        //[Column(TypeName = "decimal(18,2)")]
        public decimal? Discount { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
