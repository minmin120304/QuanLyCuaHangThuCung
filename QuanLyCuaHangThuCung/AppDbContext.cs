using QuanLyCuaHangThuCung.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
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
        public DbSet<Bill> Bill { get; set; }
        public DbSet<BillDetail> BillDetail { get; set; }

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

            return base.SaveChanges();
        }

        private string GenerateID(Type entityType)
        {
            string prefix = "";

            if (entityType == typeof(Product)) prefix = "SP";
            else if (entityType == typeof(Customer)) prefix = "KH";
            else if (entityType == typeof(Employee)) prefix = "NV";
            else if (entityType == typeof(Service)) prefix = "DV";
            else if (entityType == typeof(ServiceType)) prefix = "LDV";
            else if (entityType == typeof(Bill)) prefix = "HD";
            else if (entityType == typeof(BillDetail)) prefix = "CTHD";

            var method = typeof(DbContext).GetMethod("Set", Type.EmptyTypes) // Chỉ lấy overload không có tham số
                              .MakeGenericMethod(entityType);

            var table = method.Invoke(this, null) as IQueryable<object>;

            var property = entityType.GetProperty("Id");
            if (property == null)
                throw new InvalidOperationException("Entity must have an 'Id' property");

            var parameter = Expression.Parameter(entityType, "e");
            var propertyAccess = Expression.Property(parameter, "Id");
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var orderByMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType, property.PropertyType);

            var orderedQuery = orderByMethod.Invoke(null, new object[] { table, lambda });
            var firstMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "FirstOrDefault" && m.GetParameters().Length == 1)
                .MakeGenericMethod(entityType);

            var lastEntity = firstMethod.Invoke(null, new object[] { orderedQuery });


            int newIndex = 1;
            var lastEntityValue = (lastEntity != null && property != null) ? property.GetValue(lastEntity)?.ToString() : null;

            if (lastEntityValue != null && lastEntityValue.Length >= prefix.Length &&
                int.TryParse(lastEntityValue.Substring(prefix.Length), out int lastNumber))
            {
                newIndex = lastNumber + 1;
            }
            return $"{prefix}{newIndex:D2}";
        }
    }
}
