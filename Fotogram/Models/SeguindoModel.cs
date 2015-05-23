using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.SqlServer.Server;

namespace Fotogram.Models
{
    /// <summary>
    /// Classe que representa a tabela no banco de dados
    /// </summary>
    public class SeguindoModel
    {
        /// <summary>
        /// Identificação do usuário que será seguido
        /// </summary>
        public int UsuarioSeguidoId { get; set; }

        /// <summary>
        /// Identificação do usuário seguidor
        /// </summary>
        public int UsuarioSeguidorId { get; set; }

        /// <summary>
        /// Data da atualização do registro
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public DateTime DataAtualizacao { get; set; }

        /// <summary>
        /// Classe Usuario (usuário que será seguido)
        /// </summary>
        public virtual UsuarioModel UsuarioSeguido { get; set; }

        /// <summary>
        /// Classe Usuario (usuário seguidor)
        /// </summary>
        public virtual UsuarioModel UsuarioSeguidor { get; set; }
    }

    /// <summary>
    /// Classe simplificada para o método POST
    /// </summary>
    public class SeguindoViewModel
    {
        /// <summary>
        /// Nome do usuário que será seguido
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public string NomeUsuarioSeguido { get; set; }
    }
}
