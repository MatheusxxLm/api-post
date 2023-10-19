using Blog.Business.Models;
using Blog.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Blog.API.Controllers
{
    [Authorize]
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

        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreatePost(Post post)
        {
            try
            {
                var createdPost = await _postService.Create(post);
                if (createdPost == null)
                {
                    var errorResponse = new { error = "Não foi possível criar o Post" };
                    var serializedError = JsonConvert.SerializeObject(errorResponse);
                    return BadRequest(serializedError);
                }

                // Converta o ID do post em uma string para usá-lo como chave de cache
                string cacheKey = createdPost.Id.ToString();

                // Serializa o objeto post criado em JSON para armazenar no cache
                var serializedPost = JsonConvert.SerializeObject(createdPost);

                // Define opções para o cache, como a expiração
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Expira em 10 minutos
                };

                // Armazena o objeto post no cache com a chave correspondente ao ID do post
                await _cache.SetStringAsync(cacheKey, serializedPost, cacheOptions);

                // Retorna a resposta com o objeto post criado
                return CreatedAtAction(nameof(GetById), new { id = createdPost.Id }, serializedPost);
            }
            catch (Exception ex)
            {
                var errorResponse = new { error = ex.Message };
                var serializedError = JsonConvert.SerializeObject(errorResponse);
                return BadRequest(serializedError);
            }
        }

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

                // Converta o ID do post em uma string para usá-lo como chave de cache
                string cacheKey = post.ToString();

                // Remova o item correspondente ao post do cache
                await _cache.RemoveAsync(cacheKey);
                return Ok(serializedSuccess);
            }
            catch
            {
                var errorResponse = new { error = "Não foi possível excluir o Post" };
                var serializedError = JsonConvert.SerializeObject(errorResponse);
                return BadRequest(serializedError);
            }
        }

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

        [HttpGet("GetById")]
        public async Task<ActionResult<string>> GetById(int id)
        {
            var pegar = await _postService.GetById(id);
            var serializedObject = JsonConvert.SerializeObject(pegar);
            return Ok(serializedObject);
        }
    }
}
