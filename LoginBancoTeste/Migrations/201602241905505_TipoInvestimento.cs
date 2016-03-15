namespace LoginBancoTeste.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TipoInvestimento : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TipoInvestimento",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(),
                        jurosDia = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("dbo.Investimento", "tipo_invest_id", c => c.Int(nullable: false));
            CreateIndex("dbo.Investimento", "tipo_invest_id");
            AddForeignKey("dbo.Investimento", "tipo_invest_id", "dbo.TipoInvestimento", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Investimento", "tipo_invest_id", "dbo.TipoInvestimento");
            DropIndex("dbo.Investimento", new[] { "tipo_invest_id" });
            DropColumn("dbo.Investimento", "tipo_invest_id");
            DropTable("dbo.TipoInvestimento");
        }
    }
}
