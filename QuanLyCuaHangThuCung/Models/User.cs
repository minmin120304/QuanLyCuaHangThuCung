using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangThuCung.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        [StringLength(50)]
        public string Username { get; set; } // Tên đăng nhập

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } // Mật khẩu (đã mã hóa)

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } // Họ tên

        [StringLength(100)]
        public string Email { get; set; } 
        public string phoneNum { get; set; } 
        public string Address { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ngày tạo

    }
}