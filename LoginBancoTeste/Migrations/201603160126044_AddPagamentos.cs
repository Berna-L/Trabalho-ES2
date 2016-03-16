namespace LoginBancoTeste.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPagamentos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pagamentoes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        cod_boleto = c.Long(nullable: false),
                        valor = c.Double(nullable: false),
                        descricao = c.String(),
                        data_venc = c.DateTime(nullable: false),
                        data_pagam = c.DateTime(nullable: false),
                        data_realiza = c.DateTime(nullable: false),
                        conta_Numero = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Contas", t => t.conta_Numero)
                .Index(t => t.conta_Numero);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pagamentoes", "conta_Numero", "dbo.Contas");
            DropIndex("dbo.Pagamentoes", new[] { "conta_Numero" });
            DropTable("dbo.Pagamentoes");
        }
    }
}
