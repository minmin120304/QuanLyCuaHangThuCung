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

        [StringLength(50)]
        public string type { get; set; }

        public int price { get; set; }

        public string note { get; set; }
    }
}
