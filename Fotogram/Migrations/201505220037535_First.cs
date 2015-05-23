namespace Fotogram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ComentarioModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostagemModelId = c.Int(nullable: false),
                        UsuarioModelId = c.Int(nullable: false),
                        Texto = c.String(nullable: false, maxLength: 500),
                        DataAtualizacao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PostagemModel", t => t.PostagemModelId)
                .ForeignKey("dbo.UsuarioModel", t => t.UsuarioModelId)
                .Index(t => t.PostagemModelId)
                .Index(t => t.UsuarioModelId);
            
            CreateTable(
                "dbo.PostagemModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioModelId = c.Int(nullable: false),
                        ImagemUrl = c.String(nullable: false, maxLength: 300),
                        Texto = c.String(nullable: false, maxLength: 2000),
                        DataPostagem = c.DateTime(nullable: false),
                        Local = c.String(maxLength: 100),
                        Latitude = c.Long(nullable: false),
                        Longitude = c.Long(nullable: false),
                        DataAtualizacao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UsuarioModel", t => t.UsuarioModelId)
                .Index(t => t.UsuarioModelId);
            
            CreateTable(
                "dbo.UsuarioModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NomeCompleto = c.String(nullable: false, maxLength: 200),
                        NomeUsuario = c.String(nullable: false, maxLength: 30),
                        DataNascimento = c.DateTime(nullable: false),
                        Biografia = c.String(maxLength: 500),
                        Avatar = c.String(maxLength: 800),
                        Email = c.String(nullable: false, maxLength: 200),
                        DataAtualizacao = c.DateTime(nullable: false),
                        Senha = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CurtidaModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostagemModelId = c.Int(nullable: false),
                        UsuarioModelId = c.Int(nullable: false),
                        DataAtualizacao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PostagemModel", t => t.PostagemModelId)
                .ForeignKey("dbo.UsuarioModel", t => t.UsuarioModelId)
                .Index(t => t.PostagemModelId)
                .Index(t => t.UsuarioModelId);
            
            CreateTable(
                "dbo.SeguindoModel",
                c => new
                    {
                        UsuarioSeguidoId = c.Int(nullable: false),
                        UsuarioSeguidorId = c.Int(nullable: false),
                        DataAtualizacao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.UsuarioSeguidoId, t.UsuarioSeguidorId })
                .ForeignKey("dbo.UsuarioModel", t => t.UsuarioSeguidoId)
                .ForeignKey("dbo.UsuarioModel", t => t.UsuarioSeguidorId)
                .Index(t => t.UsuarioSeguidoId)
                .Index(t => t.UsuarioSeguidorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SeguindoModel", "UsuarioSeguidorId", "dbo.UsuarioModel");
            DropForeignKey("dbo.SeguindoModel", "UsuarioSeguidoId", "dbo.UsuarioModel");
            DropForeignKey("dbo.CurtidaModel", "UsuarioModelId", "dbo.UsuarioModel");
            DropForeignKey("dbo.CurtidaModel", "PostagemModelId", "dbo.PostagemModel");
            DropForeignKey("dbo.ComentarioModel", "UsuarioModelId", "dbo.UsuarioModel");
            DropForeignKey("dbo.ComentarioModel", "PostagemModelId", "dbo.PostagemModel");
            DropForeignKey("dbo.PostagemModel", "UsuarioModelId", "dbo.UsuarioModel");
            DropIndex("dbo.SeguindoModel", new[] { "UsuarioSeguidorId" });
            DropIndex("dbo.SeguindoModel", new[] { "UsuarioSeguidoId" });
            DropIndex("dbo.CurtidaModel", new[] { "UsuarioModelId" });
            DropIndex("dbo.CurtidaModel", new[] { "PostagemModelId" });
            DropIndex("dbo.PostagemModel", new[] { "UsuarioModelId" });
            DropIndex("dbo.ComentarioModel", new[] { "UsuarioModelId" });
            DropIndex("dbo.ComentarioModel", new[] { "PostagemModelId" });
            DropTable("dbo.SeguindoModel");
            DropTable("dbo.CurtidaModel");
            DropTable("dbo.UsuarioModel");
            DropTable("dbo.PostagemModel");
            DropTable("dbo.ComentarioModel");
        }
    }
}
