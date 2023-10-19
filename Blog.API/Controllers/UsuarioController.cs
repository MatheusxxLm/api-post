using Blog.Business.Models;
using Blog.Business.Services;
using Blog.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.API.Controllers
{
    [Route("v1/Usuario")]
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly DataContext _context;
        public UsuarioController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Usuario>>> GetAll()
        {
            var usuarios = await _context.Usuarios.AsNoTracking().ToListAsync();
            return Ok(usuarios);
        }

        [HttpPost]
        [Route("Criar")]
        public async Task<ActionResult<Usuario>> Create([FromBody] Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            // esconde a senha 
            usuario.Senha = "";
            return Ok(usuario);
        }

        [HttpPut]
        [Route("Alterar")]
        public async Task<ActionResult<Usuario>> Update([FromBody] Usuario usuarios)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == usuarios.Id);
            if (user == null) return NotFound(new { message = "Usuário não encontrado" });
            user.Nome = usuarios.Nome;
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Usuário atualizado com sucesso" });

        }

        [HttpPost]
        [Route("Entrar")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Login([FromBody] Usuario usuario)
        {
            var ver = await _context.Usuarios.AsNoTracking().Where(x => x.Nome == usuario.Nome && x.Senha == usuario.Senha).FirstOrDefaultAsync();
            if (ver == null) return NotFound(new { message = "Usuario não encontrado" });

            var token = TokenService.GenerateToken(ver);
            // esconde a senha
            ver.Senha = "";
            return new
            {
                ver = ver,
                token = token,
            };
        }
    }
}
