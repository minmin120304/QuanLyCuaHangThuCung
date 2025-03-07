using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangThuCung.Models
{
    [Table("Product")]
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [StringLength(50)]
        public string productName { get; set; }

        [StringLength(50)]
        public string origin { get; set; }

        [StringLength(50)]
        public string unit { get; set; }

        public int quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal price { get; set; }
    }
}
