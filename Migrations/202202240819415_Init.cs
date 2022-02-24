namespace BugRepro.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MyEntities",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MySubEntities",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        MyEntityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MyEntities", t => t.MyEntityId, cascadeDelete: true)
                .Index(t => t.MyEntityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MySubEntities", "MyEntityId", "dbo.MyEntities");
            DropIndex("dbo.MySubEntities", new[] { "MyEntityId" });
            DropTable("dbo.MySubEntities");
            DropTable("dbo.MyEntities");
        }
    }
}
