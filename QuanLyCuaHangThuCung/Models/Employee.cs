using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangThuCung.Models
{
    [Table("Employee")]
    public class Employee : IHasID
    {
        [Key]
        [StringLength(10)]
        public string Id { get; set; }

        [StringLength(50)]
        public string employeeName { get; set; }

        [StringLength(150)]
        public string address { get; set; }

        [Required, Phone, StringLength(12)]
        public string phoneNum { get; set; }
        
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
}
