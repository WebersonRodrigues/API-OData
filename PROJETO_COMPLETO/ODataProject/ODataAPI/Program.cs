using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using ODataAPI.Data;
using ODataAPI.Models;
using ODataAPI.ModelBinders;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.EnvironmentName == "Testing")
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("TestDatabase"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5433;Database=ODataDB;Username=postgres;Password=123456"));
}

// Configuração do Entity Framework com PostgreSQL

// Configuração do OData
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Empresa>("Empresas");
modelBuilder.EntitySet<Loja>("Lojas");
modelBuilder.EntitySet<Produto>("Produtos");
modelBuilder.EntitySet<Pedido>("Pedidos");
modelBuilder.EntitySet<PedidoItem>("PedidoItens");
modelBuilder.EntitySet<Avaliacao>("Avaliacoes");
modelBuilder.EntitySet<DetalheItem>("DetalhesItem");
modelBuilder.EntitySet<RespostaAvaliacao>("RespostasAvaliacao");

// Add services to the container.
builder.Services.AddControllers(options =>
    {
        options.ModelBinderProviders.Insert(0, new ODataModelBinderProvider());
    })
    .AddOData(options => options
        .Select().Filter().OrderBy().Expand().Count().SetMaxTop(null)
        .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

// CORS para permitir requisições de qualquer origem
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Criar banco de dados se não existir
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();

// Torna a classe Program acessível para testes
public partial class Program { }