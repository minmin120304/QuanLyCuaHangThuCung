using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangThuCung.Models
{
    [Table("BillDetail")]
    public class BillDetail : IHasID
    {
        [Key]
        public string Id { get; set; }
        public string BillId { get; set; }

        public string ProductId { get; set; } // Nếu là sản phẩm
        public string ServiceId { get; set; } // Nếu là dịch vụ
        public int Quantity { get; set; } // Chỉ áp dụng cho sản phẩm

        //[Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        //[Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [ForeignKey("BillId")]
        public virtual Bill Bill { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
    }
}
