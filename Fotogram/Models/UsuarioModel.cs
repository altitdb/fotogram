using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fotogram.Models
{
    /// <summary>
    /// Classe que representa a tabela no banco de dados
    /// </summary>
    public class UsuarioModel
    {
        /// <summary>
        /// Identificador
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome Completo
        /// </summary>
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string NomeCompleto { get; set; }

        /// <summary>
        /// Nome de Usuario (único)
        /// </summary>
        [Required]
        [StringLength(30, MinimumLength = 5)]
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Data de Nascimento
        /// </summary>
        [Required]
        public DateTime DataNascimento { get; set; }

        /// <summary>
        /// Mini biografia do usuário
        /// </summary>
        [StringLength(500)]
        public string Biografia { get; set; }

        /// <summary>
        /// Avatar (url da imagem do perfil)
        /// </summary>
        [StringLength(800)]
        public string Avatar { get; set; }

        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Email { get; set; }

        /// <summary>
        /// Data da atualização do registro
        /// </summary>
        [Required]
        public DateTime DataAtualizacao { get; set; }

        /// <summary>
        /// Senha (criptografada)
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Senha { get; set; }
    }
    
    /// <summary>
    /// Classe simplificada para o método POST
    /// </summary>
    public class RegistroUsuarioViewModel
    {
        /// <summary>
        /// Nome Completo
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(200, ErrorMessage = "O campo {0} aceita no máximo {1} e no mínimo {2} caracteres!", MinimumLength = 2)]
        public string NomeCompleto { get; set; }

        /// <summary>
        /// Nome de Usuario (único)
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(30, ErrorMessage = "O campo {0} aceita no máximo {1} e no mínimo {2} caracteres!", MinimumLength = 5)]
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Data de Nascimento
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public DateTime DataNascimento { get; set; }

        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(200, ErrorMessage = "O campo {0} aceita no máximo {1} caracteres!")]
        public string Email { get; set; }

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(30, ErrorMessage = "O campo {0} aceita no máximo {1} e no mínimo {2} caracteres!", MinimumLength = 5)]
        public string Senha { get; set; }

        /// <summary>
        /// Confirmacao de senha do usuário
        /// </summary>
        public string ConfirmacaoSenha { get; set; }
    }

    /// <summary>
    /// Classe simplificada para o método POST
    /// </summary>
    public class AlteracaoUsuarioViewModel
    {
        /// <summary>
        /// Nome de usuário (não pode ser alterado)
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(200, ErrorMessage = "O campo {0} aceita no máximo {1} e no mínimo {2} caracteres!", MinimumLength = 2)]
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Nome Completo
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(200, ErrorMessage = "O campo {0} aceita no máximo {1} e no mínimo {2} caracteres!", MinimumLength = 2)]
        public string NomeCompleto { get; set; }

        /// <summary>
        /// Data de Nascimento
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public DateTime DataNascimento { get; set; }

        /// <summary>
        /// Mini biografia do usuário
        /// </summary>
        [StringLength(500, ErrorMessage = "O campo {0} aceita no máximo {1} caracteres!")]
        public string Biografia { get; set; }

        /// <summary>
        /// Avatar (arquivo da imagem do perfil)
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(200, ErrorMessage = "O campo {0} aceita no máximo {1} caracteres!")]
        public string Email { get; set; }
    }

    /// <summary>
    /// Classe simplificada para visualização do registro
    /// </summary>
    public class VisualizacaoUsuarioViewModel
    {
        /// <summary>
        /// Nome Completo
        /// </summary>
        public string NomeCompleto { get; set; }

        /// <summary>
        /// Nome de Usuário
        /// </summary>
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Data de Nascimento
        /// </summary>
        public DateTime DataNascimento { get; set; }

        /// <summary>
        /// Mini biografia do usuário
        /// </summary>
        public string Biografia { get; set; }

        /// <summary>
        /// Avatar (url da imagem do perfil)
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Quantidade de pessoas que segue
        /// </summary>
        public int Seguindo { get; set; }

        /// <summary>
        /// Quantidade de seguidores
        /// </summary>
        public int Seguidores { get; set; }
    }

    /// <summary>
    /// Classe simplificada para o método POST
    /// </summary>
    public class AlteracaoSenhaViewModel
    {
        /// <summary>
        /// Senha antiga do usuário (a senha já cadastrada)
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public string SenhaAntiga { get; set; }

        /// <summary>
        /// A nova senha do usuário (que substituirá a antiga)
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(30, ErrorMessage = "O campo {0} aceita no máximo {1} e no mínimo {2} caracteres!", MinimumLength = 5)]
        public string SenhaNova { get; set; }

        /// <summary>
        /// Confirmação de nova senha do usuário
        /// </summary>
        public string ConfirmacaoSenha { get; set; }
    }
}
