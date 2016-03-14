namespace LoginBancoTeste.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdicionaInvestimentos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Extrato",
                c => new
                    {
                        ExtratoId = c.Int(nullable: false, identity: true),
                        Data = c.DateTime(nullable: false),
                        Lancamento = c.String(),
                        Valor = c.Double(nullable: false),
                        SaldoAtual = c.Double(nullable: false),
                        Conta_Numero = c.Int(),
                    })
                .PrimaryKey(t => t.ExtratoId)
                .ForeignKey("dbo.Conta", t => t.Conta_Numero)
                .Index(t => t.Conta_Numero);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Extrato", "Conta_Numero", "dbo.Conta");
            DropIndex("dbo.Extrato", new[] { "Conta_Numero" });
            DropTable("dbo.Extrato");
        }
    }
}
