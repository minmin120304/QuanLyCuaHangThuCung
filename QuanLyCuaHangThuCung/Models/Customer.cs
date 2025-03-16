using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangThuCung.Models
{
    [Table("Customer")]
    public class Customer : IHasID
    {
        [Key]
        [StringLength(10)]
        public string Id { get; set; }

        [StringLength(50)]
        public string customerName { get; set; }

        [StringLength(150)]
        public string address { get; set; }

        [Required, Phone, StringLength(12)]
        public string phoneNum { get; set; }
        
        public virtual ICollection<BuyHistory> BuyHistories { get; set; } = new List<BuyHistory>();
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
}
