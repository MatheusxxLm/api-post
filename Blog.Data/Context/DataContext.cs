using Blog.Business.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions opts) : base(opts)
        {
            
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var post in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys())) post.DeleteBehavior = DeleteBehavior.ClientSetNull;

            //modelBuilder.Entity<Post>().Ignore(p => p.IsPost);
        }
    }
    
}
