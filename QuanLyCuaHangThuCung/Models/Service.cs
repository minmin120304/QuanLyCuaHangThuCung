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
    public class Service : IHasID
    {
        [Key]
        [StringLength(10)]
        public string Id { get; set; }

        [StringLength(50)]
        public string serviceName { get; set; }
        
        [Required]
        public string serviceType { get; set; }

        public decimal price { get; set; }

        public string note { get; set; }
    }
}
