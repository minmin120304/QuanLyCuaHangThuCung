using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangThuCung.Models
{
    [Table("Product")]
    public class Product : IHasID
    {
        [Key]
        [StringLength(10)]
        public string Id { get; set; }

        [StringLength(50)]
        public string productName { get; set; }

        [StringLength(50)]
        public string origin { get; set; }
        public string type { get; set; }

        [StringLength(50)]
        public string unit { get; set; }

        [Required]
        public int quantity { get; set; }

        [Required]
        public decimal price { get; set; }
        public DateTime DateInput { get; set; } = DateTime.Now;
    }
}
