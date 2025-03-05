using QuanLyCuaHangThuCung.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangThuCung
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=MyConectionString") { }
        public DbSet<Product> Product { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<ServiceType> ServiceType { get; set; }
    }
}
