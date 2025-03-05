namespace QuanLyCuaHangThuCung.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServieTest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Service",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        serviceName = c.String(maxLength: 50),
                        type = c.String(),
                        price = c.Int(nullable: false),
                        note = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ServiceType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        typeID = c.String(maxLength: 50),
                        typeName = c.String(),
                        note = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ServiceType");
            DropTable("dbo.Service");
        }
    }
}
