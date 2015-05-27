using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fotogram.Models
{
    /// <summary>
    /// Classe que representa a tabela no banco de dados
    /// </summary>
    public class ComentarioModel
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
        /// Texto do comentário
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Texto { get; set; }
        
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
    public class ComentarioViewModel
    {
        /// <summary>
        /// Identificador da postagem
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public int PostagemModelId { get; set; }
        
        /// <summary>
        /// Texto do comentário
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(50, ErrorMessage = "O campo {0} aceita no máximo {1} caracteres!")]
        public string Texto { get; set; }
    }

    /// <summary>
    /// Classe simplificada para o método GET
    /// </summary>
    public class ComentarioVisualizacaoViewModel
    {
        /// <summary>
        /// Identificador da postagem
        /// </summary>
        public int PostagemModelId { get; set; }

        /// <summary>
        /// Texto do comentário
        /// </summary>
        public string Texto { get; set; }

        /// <summary>
        /// Data e hora do comentário
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Nome do usuário que postou o comentário
        /// </summary>
        public string NomeUsuario { get; set; }
    }
}
