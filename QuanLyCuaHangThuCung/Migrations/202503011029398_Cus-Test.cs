namespace QuanLyCuaHangThuCung.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CusTest : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Customer", newName: "Customer");
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        customerName = c.String(maxLength: 50),
                        address = c.String(maxLength: 50),
                        phoneNum = c.String(maxLength: 12),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Customer");
            RenameTable(name: "dbo.Customer", newName: "Customer");
        }
    }
}
