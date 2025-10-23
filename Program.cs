using Api.Dao;
using Api.Service;
using Api.Database;
using Api.Middleware;
using Microsoft.AspNetCore.Mvc;

// Cria o builder da aplicação
WebApplicationBuilder builder = WebApplication.CreateBuilder();
// Configura Kestrel para escutar **somente na porta 8080**
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // rodar na porta 8080
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Registra dependencias no container DI
//registra dependencia de banco de dados
builder.Services.AddSingleton<MySqlDatabase>(serviceProvider =>
{
    return new MySqlDatabase(
        host: "localhost",
        user: "root",
        password: "",
        database: "gestao_rh",
        port: 3306,
        connectionLimit: 10
    );
});

//registra dependencia de DAOs e Services de Cargo
builder.Services.AddSingleton<CargoDAO>();
builder.Services.AddSingleton<CargoService>();

//registra dependencia de DAOs e Services de Funcionario
builder.Services.AddSingleton<FuncionarioDAO>();
builder.Services.AddSingleton<FuncionarioService>();

// Suporte a controllers e endpoints da API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

/*
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
*/

// Constrói a aplicação
WebApplication app = builder.Build();

app.UseCors("AllowAll");
// Middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseStaticFiles();
app.UseAuthorization();

// Mapeia controladores
app.MapControllers();

// Mensagem no console
Console.WriteLine("🚀 API rodando em: http://localhost:8080/Login.html");

// Inicia a aplicação
app.Run();
