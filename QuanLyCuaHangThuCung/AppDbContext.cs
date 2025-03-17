using QuanLyCuaHangThuCung.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static iTextSharp.text.pdf.events.IndexEvents;
using Expression = System.Linq.Expressions.Expression;

namespace QuanLyCuaHangThuCung
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=MyConectionString") { }
        public DbSet<Product> Product { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<BillDetail> BillDetail { get; set; }
        public DbSet<User> User { get; set; }

        public override int SaveChanges()
        {
            var newEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added && e.Entity is IHasID)
                .Select(e => e.Entity as IHasID)
                .ToList();

            foreach (var entity in newEntities)
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = GenerateID(entity.GetType());
                }
            }

            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string errorMsg = $"Property: {validationError.PropertyName} - Error: {validationError.ErrorMessage}";
                        Console.WriteLine(errorMsg);
                        sb.AppendLine(errorMsg);
                    }
                }
                MessageBox.Show(sb.ToString(), "Lỗi Validation", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private string GenerateID(Type entityType)
        {
            string prefix;
            if (entityType == typeof(Product)) prefix = "SP";
            else if (entityType == typeof(Customer)) prefix = "KH";
            else if (entityType == typeof(Employee)) prefix = "NV";
            else if (entityType == typeof(Service)) prefix = "DV";
            else if (entityType == typeof(Bill)) prefix = "HD";
            else if (entityType == typeof(BillDetail)) prefix = "CTHD";
            else if (entityType == typeof(BuyHistory)) prefix = "LSMH";
            else throw new InvalidOperationException("Entity type is not supported");

            var property = entityType.GetProperty("Id");
            if (property == null)
                throw new InvalidOperationException("Entity must have an 'Id' property");

            // Lấy DbSet tương ứng
            var method = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)?.MakeGenericMethod(entityType);
            var queryable = method?.Invoke(this, null) as IQueryable<object>;
            if (queryable == null)
                throw new InvalidOperationException("Failed to get entity DbSet");

            // Lọc danh sách ID bằng cách lấy trực tiếp giá trị từ property "Id"
            // Chuyển IQueryable về bộ nhớ trước khi dùng reflection
            var idList = queryable.AsEnumerable()
                                  .Select(e => (string)property.GetValue(e, null))
                                  .Where(id => id != null)
                                  .ToList();

            // Lọc ID theo tiền tố và lấy số lớn nhất
            var maxId = idList
                .Where(id => id.StartsWith(prefix) && int.TryParse(id.Substring(prefix.Length), out _))
                .Select(id => int.Parse(id.Substring(prefix.Length)))
                .DefaultIfEmpty(0)
                .Max();

            return $"{prefix}{(maxId + 1):D2}";
        }
    }
}
