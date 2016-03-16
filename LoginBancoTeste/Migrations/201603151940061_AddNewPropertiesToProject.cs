namespace LoginBancoTeste.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewPropertiesToProject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cheques",
                c => new
                    {
                        numCheque = c.Long(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.numCheque);
            
            CreateTable(
                "dbo.Agencias",
                c => new
                    {
                        numAgencia = c.Long(nullable: false, identity: true),
                        banco_numBanco = c.Long(),
                    })
                .PrimaryKey(t => t.numAgencia)
                .ForeignKey("dbo.Bancoes", t => t.banco_numBanco)
                .Index(t => t.banco_numBanco);
            
            CreateTable(
                "dbo.Bancoes",
                c => new
                    {
                        numBanco = c.Long(nullable: false, identity: true),
                        nome = c.String(),
                    })
                .PrimaryKey(t => t.numBanco);
            
            AddColumn("dbo.Contas", "agencia_numAgencia", c => c.Long());
            CreateIndex("dbo.Contas", "agencia_numAgencia");
            AddForeignKey("dbo.Contas", "agencia_numAgencia", "dbo.Agencias", "numAgencia");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contas", "agencia_numAgencia", "dbo.Agencias");
            DropForeignKey("dbo.Agencias", "banco_numBanco", "dbo.Bancoes");
            DropIndex("dbo.Agencias", new[] { "banco_numBanco" });
            DropIndex("dbo.Contas", new[] { "agencia_numAgencia" });
            DropColumn("dbo.Contas", "agencia_numAgencia");
            DropTable("dbo.Bancoes");
            DropTable("dbo.Agencias");
            DropTable("dbo.Cheques");
        }
    }
}
