using System;
using System.ComponentModel.DataAnnotations;

namespace Fotogram.Models
{
    /// <summary>
    /// Classe que representa a tabela no banco de dados
    /// </summary>
    public class CurtidaModel
    {
        /// <summary>
        /// Identificador
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Identificador da postagem
        /// </summary>
        [Required]
        public int PostagemModelId { get; set; }

        /// <summary>
        /// Identificador do usuário
        /// </summary>
        [Required]
        public int UsuarioModelId { get; set; }

        /// <summary>
        /// Data da atualização do registro
        /// </summary>
        [Required]
        public DateTime DataAtualizacao { get; set; }

        /// <summary>
        /// Classe Postagem
        /// </summary>
        public virtual PostagemModel Postagem { get; set; }

        /// <summary>
        /// Classe Usuario
        /// </summary>
        public virtual UsuarioModel Usuario { get; set; }
    }

    /// <summary>
    /// Classe simplificada para o método POST
    /// </summary>
    public class CurtidaViewModel
    {
        /// <summary>
        /// Identificador da postagem
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public int PostagemModelId { get; set; }

        /// <summary>
        /// Identificador do usuário
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public int UsuarioModelId { get; set; }
    }
}
