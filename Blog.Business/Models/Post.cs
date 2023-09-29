using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blog.Business.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        [Required]
        public string Subtitulo { get; set; }
        [Required]
        public string Img { get; set; }
        // ao acessar a classe POST e a propriedade ISPOST, ele irá invocar o metodo Validade();
        //public bool IsPost => Validate();
        [JsonIgnore]
        public bool IsPost { get; set; }

        public Post(int id, string titulo, string subtitulo, string img)
        {
            Id = id;
            Titulo = titulo;
            Subtitulo = subtitulo;
            Img = img;
        }

        // Método para atualizar os dados do post
        public void AtualizarDados(string novoTitulo, string novoSubtitulo, string novaImg)
        {
            if (!string.IsNullOrWhiteSpace(novoTitulo))
                Titulo = novoTitulo;

            if (!string.IsNullOrWhiteSpace(novoSubtitulo))
                Subtitulo = novoSubtitulo;

            if (!string.IsNullOrWhiteSpace(novaImg))
                Img = novaImg;
        }
        public bool Validate(Post id)
        {
            if (id.Titulo.Length == 10)
            {
                id.IsPost = false;
                Invalidate();
            }      
            id.IsPost = true;
            return true;
        }

        private string Invalidate()
        {
            throw new NotImplementedException("Post inválido");
        }
    }
}
