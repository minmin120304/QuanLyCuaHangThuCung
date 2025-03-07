using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuanLyCuaHangThuCung.Models
{
    [Table("Service")]
    public class Service
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [StringLength(50)]
        public string serviceName { get; set; }
        
        [Required]
        public int ServiceTypeId { get; set; }

        public decimal price { get; set; }

        public string note { get; set; }

        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }
    }
}
