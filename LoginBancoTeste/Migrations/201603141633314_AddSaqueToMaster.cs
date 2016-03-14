namespace LoginBancoTeste.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSaqueToMaster : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Cliente", newName: "Clientes");
            RenameTable(name: "dbo.Conta", newName: "Contas");
            RenameTable(name: "dbo.Extrato", newName: "Extratoes");
            RenameTable(name: "dbo.Endereco", newName: "Enderecoes");
            RenameTable(name: "dbo.Investimento", newName: "Investimentoes");
            RenameTable(name: "dbo.TipoInvestimento", newName: "TipoInvestimentoes");
            CreateTable(
                "dbo.Estoques",
                c => new
                    {
                        EstoqueID = c.Int(nullable: false, identity: true),
                        QtdNotas10 = c.Int(nullable: false),
                        QtdNotas20 = c.Int(nullable: false),
                        QtdNotas50 = c.Int(nullable: false),
                        QtdNotas100 = c.Int(nullable: false),
                        QtdCheques = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EstoqueID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Estoques");
            RenameTable(name: "dbo.TipoInvestimentoes", newName: "TipoInvestimento");
            RenameTable(name: "dbo.Investimentoes", newName: "Investimento");
            RenameTable(name: "dbo.Enderecoes", newName: "Endereco");
            RenameTable(name: "dbo.Extratoes", newName: "Extrato");
            RenameTable(name: "dbo.Contas", newName: "Conta");
            RenameTable(name: "dbo.Clientes", newName: "Cliente");
        }
    }
}
