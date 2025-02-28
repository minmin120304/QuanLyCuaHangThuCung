using QuanLyCuaHangThuCung.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangThuCung.CSDL
{
     class ProductList : DbContext
    {
        public ProductList() : base("name=MyConectionString") { }
        public DbSet<Product> Product { get; set; }
    }
}
