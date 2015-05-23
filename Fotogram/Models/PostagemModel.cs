﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Fotogram.Models
{
    /// <summary>
    /// Classe que representa a tabela no banco de dados
    /// </summary>
    public class PostagemModel
    {
        /// <summary>
        /// Identificador
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Identificador do usuário
        /// </summary>
        [Required]
        public int UsuarioModelId { get; set; }

        /// <summary>
        /// Url da Imagem
        /// </summary>
        [Required]
        [StringLength(300)]
        public string ImagemUrl { get; set; }

        /// <summary>
        /// Texto da postagem
        /// </summary>
        [Required]
        [StringLength(2000)]
        public string Texto { get; set; }

        /// <summary>
        /// Data da Postagem
        /// </summary>
        [Required]
        public DateTime DataPostagem { get; set; }

        /// <summary>
        /// Descrição do local, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        [StringLength(100)]
        public string Local { get; set; }

        /// <summary>
        /// Latitude, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        public Int64 Latitude { get; set; }

        /// <summary>
        /// Longitude, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        public Int64 Longitude { get; set; }

        /// <summary>
        /// Data da atualização do registro
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public DateTime DataAtualizacao { get; set; }

        /// <summary>
        /// Classe Usuario (usuário seguidor)
        /// </summary>
        public virtual UsuarioModel Usuario { get; set; }
    }

    /// <summary>
    /// Classe simplificada para o método POST
    /// </summary>
    public class NovaPostagemViewModel
    {

        /// <summary>
        /// Identificador do usuário
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public int UsuarioModelId { get; set; }

        /// <summary>
        /// Imagem da postagem
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public byte[] Imagem { get; set; }

        /// <summary>
        /// Texto da postagem
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(2000, ErrorMessage = "O campo {0} aceita no máximo {1} caracteres!")]
        public string Texto { get; set; }

        /// <summary>
        /// Descrição do local, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        [StringLength(100, ErrorMessage = "O campo {0} aceita no máximo {1} caracteres!")]
        public string Local { get; set; }

        /// <summary>
        /// Latitude, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        [StringLength(16, ErrorMessage = "O campo {0} aceita no máximo {1} caracteres!")]
        public string Latitude { get; set; }

        /// <summary>
        /// Longitude, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        [StringLength(16, ErrorMessage = "O campo {0} aceita no máximo {1} caracteres!")]
        public string Longitude { get; set; }
    }
}