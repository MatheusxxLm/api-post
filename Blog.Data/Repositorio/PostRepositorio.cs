using Blog.Business.Models;
using Blog.Business.Interfaces;
using Blog.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Blog.Data.Repositorio
{
    public class PostRepositorio : IPostRepositorio
    {
        private readonly DataContext _context;
        public PostRepositorio(DataContext context)
        {
            _context = context;
        }
        public async Task<Post> Create(Post post)
        {
            await _context.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> Delete(int post)
        {
            var obj = await _context.Posts.FirstOrDefaultAsync(x => x.Id == post);
            try
            {
                if(obj == null)
                    return false;

                _context.Remove(obj);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Post>> GetAll()
        {
            return await _context.Posts.AsNoTracking().ToListAsync();
        }

        public async Task<Post> GetById(int id)
        {
            var verifica = _context.Posts.FirstOrDefault(x => x.Id == id);
            if (!verifica.IsPost)
                throw new Exception("IsPost é falsooo");

            return await _context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Post> Update(Post post)
        {
            var existingPost = await _context.Posts.FindAsync(post.Id);
            existingPost.Subtitulo = post.Subtitulo;
            existingPost.Titulo = post.Titulo;
            existingPost.Img = post.Img;

            _context.Update(existingPost);
            await _context.SaveChangesAsync();
            return existingPost;
        }
    }
}
