using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangThuCung.Models
{
    [Table("BuyHistory")]
    public class BuyHistory
    {
        [Key]
        [StringLength(10)]
        public string Id { get; set; }
        public string BillId { get; set; }
        public string CustomerId { get; set; } 

        [ForeignKey("BillId")]
        public virtual Bill Bill { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}
