using Blog.Business.Interfaces;
using Blog.Business.Services;
using Blog.Data.Context;
using Blog.Data.Repositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEntityFrameworkSqlServer()
                .AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DataBase"))

                );

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // Ou a URL do seu aplicativo Angular
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost"; // Endereço do servidor Redis
    options.InstanceName = "SampleInstance"; // Nome da instância do Redis
});

builder.Services.AddScoped<IPostRepositorio, PostRepositorio>();
builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowSpecificOrigin");
app.Run();
