namespace LoginBancoTeste.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LongInvest : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Investimento", "valor_ini", c => c.Long(nullable: false));
            AlterColumn("dbo.Investimento", "valor_acc", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Investimento", "valor_acc", c => c.Double(nullable: false));
            AlterColumn("dbo.Investimento", "valor_ini", c => c.Double(nullable: false));
        }
    }
}
