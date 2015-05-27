using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fotogram.Models
{
    /// <summary>
    /// Contexto do EF
    /// </summary>
    public class FotogramContextDb : DbContext
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public FotogramContextDb()
            : base("name=FotogramContextDb")
        {
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        /// <summary>
        /// ComentarioModel
        /// </summary>
        public DbSet<ComentarioModel> ComentarioModel { get; set; }

        /// <summary>
        /// CurtidaModel
        /// </summary>
        public DbSet<CurtidaModel> CurtidaModel { get; set; }

        /// <summary>
        /// PostagemModel
        /// </summary>
        public DbSet<PostagemModel> PostagemModel { get; set; }

        /// <summary>
        /// SeguindoModel
        /// </summary>
        public DbSet<SeguindoModel> SeguindoModel { get; set; }

        /// <summary>
        /// UsuarioModel
        /// </summary>
        public DbSet<UsuarioModel> UsuarioModel { get; set; }

        /// <summary>
        /// Override do OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // configura as pks compostas da classe SeguindoModel
            modelBuilder.Entity<SeguindoModel>()
                .HasKey(h => new {h.UsuarioSeguidoId, h.UsuarioSeguidorId});

            base.OnModelCreating(modelBuilder);
        }
    }
}
