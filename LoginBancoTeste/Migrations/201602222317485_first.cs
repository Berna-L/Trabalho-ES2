namespace LoginBancoTeste.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cliente",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Telefone = c.String(nullable: false),
                        Celular = c.String(),
                        Email = c.String(nullable: false),
                        Username = c.String(),
                        Password = c.String(),
                        Endereco_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Endereco", t => t.Endereco_Id)
                .Index(t => t.Endereco_Id);
            
            CreateTable(
                "dbo.Conta",
                c => new
                    {
                        Numero = c.Int(nullable: false, identity: true),
                        Saldo = c.Double(nullable: false),
                        TipoDeConta = c.Int(nullable: false),
                        Cliente_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Numero)
                .ForeignKey("dbo.Cliente", t => t.Cliente_Id)
                .Index(t => t.Cliente_Id);
            
            CreateTable(
                "dbo.Endereco",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rua = c.String(nullable: false),
                        Cidade = c.String(nullable: false),
                        Numero = c.String(),
                        Cep = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Investimento",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        valor_ini = c.Double(nullable: false),
                        valor_acc = c.Double(nullable: false),
                        data = c.DateTime(nullable: false),
                        data_canc = c.DateTime(nullable: false),
                        cliente_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cliente", t => t.cliente_Id, cascadeDelete: true)
                .Index(t => t.cliente_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Investimento", "cliente_Id", "dbo.Cliente");
            DropForeignKey("dbo.Cliente", "Endereco_Id", "dbo.Endereco");
            DropForeignKey("dbo.Conta", "Cliente_Id", "dbo.Cliente");
            DropIndex("dbo.Investimento", new[] { "cliente_Id" });
            DropIndex("dbo.Conta", new[] { "Cliente_Id" });
            DropIndex("dbo.Cliente", new[] { "Endereco_Id" });
            DropTable("dbo.Investimento");
            DropTable("dbo.Endereco");
            DropTable("dbo.Conta");
            DropTable("dbo.Cliente");
        }
    }
}
