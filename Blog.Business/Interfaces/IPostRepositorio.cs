using Blog.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Business.Interfaces
{
    public interface IPostRepositorio
    {
        Task<Post> Create(Post post);
        Task<Post> Update(Post post);
        Task<bool> Delete(int post);
        Task<List<Post>> GetAll();
        Task<Post> GetById(int id);
    }
}
