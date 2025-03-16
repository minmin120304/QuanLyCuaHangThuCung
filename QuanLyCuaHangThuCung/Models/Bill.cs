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
        public decimal TotalAmount { get; set; }
        public string Note { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
    }
}
