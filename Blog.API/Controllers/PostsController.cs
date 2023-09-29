using Blog.Business.Models;
using Blog.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Blog.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IDistributedCache _cache;
        public PostsController(IPostService service, IDistributedCache cache)
        {
            _postService = service;
            _cache = cache;

        }

        [AllowAnonymous]
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreatePost(Post post)
        {
            try
            {
                var createdPost = await _postService.Create(post);
                var serializedObject = JsonConvert.SerializeObject(createdPost);
                return CreatedAtAction(nameof(GetById), new { id = createdPost.Id }, serializedObject);
            }
            catch (Exception ex)
            {
                var errorResponse = new { error = ex.Message };
                var serializedError = JsonConvert.SerializeObject(errorResponse);
                return BadRequest(serializedError);
            }
        }
        [AllowAnonymous]
        [HttpPut("Update")]
        public async Task<ActionResult<string>> Update(Post post)
        {
            try
            {
                var updatePost = await _postService.Update(post);
                var serializedObject = JsonConvert.SerializeObject(updatePost);
                return Ok(serializedObject);
            }
            catch (Exception ex)
            {
                var errorResponse = new { error = ex.Message };
                var serializedError = JsonConvert.SerializeObject(errorResponse);
                return BadRequest(serializedError);
            }
        }
        [AllowAnonymous]
        [HttpDelete("Delete")]
        public async Task<ActionResult<string>> Delete(int post)
        {
            try
            {
                var updatePost = await _postService.Delete(post);
                if (!updatePost)
                {
                    var notFoundResponse = new { error = "Post não existe" };
                    var serializedNotFound = JsonConvert.SerializeObject(notFoundResponse);
                    return BadRequest(serializedNotFound);
                }
                var successResponse = new { message = "Post Excluido" };
                var serializedSuccess = JsonConvert.SerializeObject(successResponse);
                return Ok(serializedSuccess);
            }
            catch
            {
                var errorResponse = new { error = "Não foi possível excluir o Post" };
                var serializedError = JsonConvert.SerializeObject(errorResponse);
                return BadRequest(serializedError);
            }
        }
        //[AllowAnonymous]
        //[HttpGet("GetAllPosts")]
        //public async Task<ActionResult<string>> GetAllPosts()
        //{
        //    var pegar = await _postService.GetAll();
        //    var serializedObject = JsonConvert.SerializeObject(pegar);
        //    return Ok(serializedObject);
        //}
        [HttpGet("GetAllPosts")]
        public async Task<ActionResult<string>> GetAllPosts()
        {
            // Define uma chave única para este conjunto de resultados no cache
            string cacheKey = "AllPosts";

            // Tenta obter os dados do cache
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData != null)
            {
                // Se os dados estiverem no cache, retorna os dados do cache
                return Ok(cachedData);
            }
            else
            {
                // Caso contrário, busca os dados do serviço
                var posts = await _postService.GetAll();

                // Serializa os dados em JSON
                var serializedData = JsonConvert.SerializeObject(posts);

                // Define opções para o cache, como a expiração
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Expira em 10 minutos
                };

                // Armazena os dados no cache
                await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

                // Retorna os dados buscados
                return Ok(serializedData);
            }
        }
        [AllowAnonymous]
        [HttpGet("GetById")]
        public async Task<ActionResult<string>> GetById(int id)
        {
            var pegar = await _postService.GetById(id);
            var serializedObject = JsonConvert.SerializeObject(pegar);
            return Ok(serializedObject);
        }
    }
}
