namespace QuanLyCuaHangThuCung.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductTest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductList",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        productName = c.String(maxLength: 50),
                        origin = c.String(maxLength: 50),
                        unit = c.String(maxLength: 50),
                        quantity = c.Int(nullable: false),
                        price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductList");
        }
    }
}
