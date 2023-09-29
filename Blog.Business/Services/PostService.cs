using Blog.Business.Interfaces;
using Blog.Business.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Blog.Business.Services
{
    public class PostService : IPostService, IPostRepositorio
    {
        private readonly IPostRepositorio _repo;

        public PostService(IPostRepositorio repo)
        {
            _repo = repo;
        }
        public async Task<Post> Create(Post post)
        {
            // Exemplo de validação: Verificar se o título não está vazio.
            if (string.IsNullOrWhiteSpace(post.Titulo))
            {
                throw new ArgumentException("O título do post não pode estar vazio.");
            }

            // Aqui você pode adicionar mais validações e regras de negócios, se necessário.
            post.Validate(post);
            await _repo.Create(post); // Chama o método do repositório para criar o post.
           
            return post;
        }

        public async Task<bool> Delete(int post)
        {
            return await _repo.Delete(post);
        }

        public Task<List<Post>> GetAll()
        {
            return _repo.GetAll();
        }

        public Task<Post> GetById(int id)
        {
            return _repo.GetById(id);
        }

        public async Task<Post> Update(Post post)
        {
            
            if (post == null)
            {
                throw new InvalidOperationException("Post não encontrado.");
            }

            // Chama o método AtualizarDados para atualizar as propriedades do post.
            //if(!post.IsPost)
            //{
            //    throw new InvalidOperationException("Post não é válido");
            //}
            post.Validate(post);
            await _repo.Update(post); // Atualiza o post no repositório.
            return post;
        }
    }
}
