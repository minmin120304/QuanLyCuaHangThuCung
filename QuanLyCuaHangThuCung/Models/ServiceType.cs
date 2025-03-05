using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuanLyCuaHangThuCung.Models
{
    [Table("ServiceType")]
    public class ServiceType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [StringLength(50)]

        public string typeID {get; set; }
        
        public string typeName { get; set; }

        public string note { get; set; }
    }
}
