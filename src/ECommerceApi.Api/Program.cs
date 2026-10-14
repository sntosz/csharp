using ECommerceApi.Api.Middlewares;
using ECommerceApi.Application;
using ECommerceApi.Infrastructure;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adicionar camadas de aplicação e infraestrutura à Injeção de Dependências (IoC Container)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger / OpenAPI com documentação clara
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ECommerce Management API - Mercado .NET",
        Version = "v1",
        Description = "API RESTful construída com Arquitetura Limpa (Clean Architecture), CQRS, MediatR, FluentValidation e EF Core em .NET 8.",
        Contact = new OpenApiContact
        {
            Name = "Suporte Dev",
            Email = "dev@exemplo.com"
        }
    });
});

var app = builder.Build();

// Middleware de tratamento global de exceções
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Configuração do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API v1");
        c.RoutePrefix = string.Empty; // Define o Swagger UI na raiz da aplicação (http://localhost:5000/)
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Aplicar Migrations e Seeds de Banco de Dados ao Iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    // Seeding de Produtos de exemplo para facilitar estudos imediatos
    if (!await dbContext.Products.AnyAsync())
    {
        dbContext.Products.AddRange(
            new ECommerceApi.Domain.Entities.Product("Notebook Gamer", "Notebook de alta performance 16GB RAM SSD 1TB", 5499.90m, 10),
            new ECommerceApi.Domain.Entities.Product("Mouse Sem Fio Ergoclick", "Mouse ergonômico óptico sem fio 2.4GHz", 129.90m, 50),
            new ECommerceApi.Domain.Entities.Product("Teclado Mecânico RGB", "Teclado mecânico switch blue retroiluminado", 299.90m, 25),
            new ECommerceApi.Domain.Entities.Product("Monitor UltraWide 29\"", "Monitor LG Full HD IPS 75Hz", 1199.00m, 15)
        );
        await dbContext.SaveChangesAsync();
    }
}

app.Run();
