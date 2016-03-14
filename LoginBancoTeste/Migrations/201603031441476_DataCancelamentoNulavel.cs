namespace LoginBancoTeste.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataCancelamentoNulavel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Investimento", "data_canc", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Investimento", "data_canc", c => c.DateTime(nullable: false));
        }
    }
}
