# Guia Completo: Criando API OData com .NET 9 e PostgreSQL do Zero

Este guia detalha **TODOS** os passos necessários para criar uma API OData completa com .NET 9, Entity Framework Core e PostgreSQL, incluindo testes automatizados.

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- **.NET 9 SDK** - [Download aqui](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL** - [Download aqui](https://www.postgresql.org/download/)
- **Visual Studio** ou **VS Code** com extensão C#
- **Git** (opcional, para controle de versão)

### Verificando as instalações:
```bash
dotnet --version          # Deve mostrar versão 9.x.x
```

---

## 🚀 PARTE 1: Configuração Inicial do Projeto

### Passo 1: Criar a estrutura de pastas
```bash
# Criar pasta principal do projeto
mkdir ODataProject
cd ODataProject

# Criar solution
dotnet new sln -n ODataSolution
```

### Passo 2: Criar o projeto da API
```bash
# Criar projeto Web API
dotnet new webapi -n ODataAPI
dotnet sln add ODataAPI/ODataAPI.csproj
```

### Passo 3: Criar projeto de testes
```bash
# Criar projeto de testes
dotnet new xunit -n ODataAPI.Tests
dotnet sln add ODataAPI.Tests/ODataAPI.Tests.csproj

# Adicionar referência do projeto principal no projeto de testes
dotnet add ODataAPI.Tests/ODataAPI.Tests.csproj reference ODataAPI/ODataAPI.csproj
```

---

## 📦 PARTE 2: Instalação de Pacotes NuGet

### Passo 4: Instalar pacotes no projeto principal (ODataAPI)
```bash
cd ODataAPI

# Pacotes essenciais para OData e PostgreSQL
dotnet add package Microsoft.AspNetCore.OData --version 9.4.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.4
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.8
dotnet add package Microsoft.AspNetCore.OpenApi --version 9.0.8
```

### Passo 5: Instalar pacotes no projeto de testes
```bash
cd ../ODataAPI.Tests

# Pacotes para testes
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 9.0.8
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 9.0.8
dotnet add package FluentAssertions --version 8.6.0
dotnet add package Microsoft.NET.Test.Sdk --version 17.12.0
dotnet add package xunit --version 2.9.2
dotnet add package xunit.runner.visualstudio --version 2.8.2
dotnet add package coverlet.collector --version 6.0.2
```

---

## 🗂️ PARTE 3: Criação dos Modelos (Models)

### Passo 6: Criar pasta Models e as classes de entidade

```bash
cd ../ODataAPI
mkdir Models
```

#### 6.1: Criar Empresa.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace ODataAPI.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string CNPJ { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Endereco { get; set; } = string.Empty;
        
        public DateTime DataCriacao { get; set; }
        
        // Relacionamentos
        public virtual ICollection<Loja> Lojas { get; set; } = new List<Loja>();
    }
}
```

#### 6.2: Criar Loja.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace ODataAPI.Models
{
    public class Loja
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Endereco { get; set; } = string.Empty;
        
        [Required]
        [StringLength(15)]
        public string Telefone { get; set; } = string.Empty;
        
        public DateTime DataAbertura { get; set; }
        
        // Chave estrangeira
        public int EmpresaId { get; set; }
        
        // Relacionamentos
        public virtual Empresa Empresa { get; set; } = null!;
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
```

#### 6.3: Criar Produto.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ODataAPI.Models
{
    public class Produto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Descricao { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }
        
        public int QuantidadeEstoque { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty;
        
        public DateTime DataCadastro { get; set; }
        
        // Chave estrangeira
        public int LojaId { get; set; }
        
        // Relacionamentos
        public virtual Loja Loja { get; set; } = null!;
        public virtual ICollection<PedidoItem> PedidoItens { get; set; } = new List<PedidoItem>();
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
    }
}
```

#### 6.4: Criar Pedido.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ODataAPI.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string NomeCliente { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string EmailCliente { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorTotal { get; set; }
        
        public DateTime DataPedido { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
        
        // Relacionamentos
        public virtual ICollection<PedidoItem> PedidoItens { get; set; } = new List<PedidoItem>();
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
    }
}
```

#### 6.5: Criar PedidoItem.cs
```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace ODataAPI.Models
{
    public class PedidoItem
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecoUnitario { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }
        
        // Chaves estrangeiras
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }
        
        // Relacionamentos
        public virtual Pedido Pedido { get; set; } = null!;
        public virtual Produto Produto { get; set; } = null!;
        public virtual ICollection<DetalheItem> DetalhesItem { get; set; } = new List<DetalheItem>();
    }
}
```

#### 6.6: Criar Avaliacao.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace ODataAPI.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }
        
        [Range(1, 5)]
        public int Nota { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Comentario { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string NomeAvaliador { get; set; } = string.Empty;
        
        public DateTime DataAvaliacao { get; set; }
        
        // Chaves estrangeiras
        public int ProdutoId { get; set; }
        public int PedidoId { get; set; }
        
        // Relacionamentos
        public virtual Produto Produto { get; set; } = null!;
        public virtual Pedido Pedido { get; set; } = null!;
        public virtual ICollection<RespostaAvaliacao> Respostas { get; set; } = new List<RespostaAvaliacao>();
    }
}
```

#### 6.7: Criar DetalheItem.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace ODataAPI.Models
{
    public class DetalheItem
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TipoDetalhe { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Valor { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Observacoes { get; set; } = string.Empty;
        
        public DateTime DataCriacao { get; set; }
        
        // Chave estrangeira
        public int PedidoItemId { get; set; }
        
        // Relacionamento
        public virtual PedidoItem PedidoItem { get; set; } = null!;
    }
}
```

#### 6.8: Criar RespostaAvaliacao.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace ODataAPI.Models
{
    public class RespostaAvaliacao
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Resposta { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string NomeResponsavel { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string CargoResponsavel { get; set; } = string.Empty;
        
        public DateTime DataResposta { get; set; }
        
        // Chave estrangeira
        public int AvaliacaoId { get; set; }
        
        // Relacionamento
        public virtual Avaliacao Avaliacao { get; set; } = null!;
    }
}
```

---

## 🗄️ PARTE 4: Configuração do Entity Framework

### Passo 7: Criar o DbContext

```bash
mkdir Data
```

#### 7.1: Criar ApplicationDbContext.cs
```csharp
using Microsoft.EntityFrameworkCore;
using ODataAPI.Models;

namespace ODataAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets para todas as entidades
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Loja> Lojas { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItens { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<DetalheItem> DetalhesItem { get; set; }
        public DbSet<RespostaAvaliacao> RespostasAvaliacao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações de relacionamentos
            modelBuilder.Entity<Loja>()
                .HasOne(l => l.Empresa)
                .WithMany(e => e.Lojas)
                .HasForeignKey(l => l.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Loja)
                .WithMany(l => l.Produtos)
                .HasForeignKey(p => p.LojaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PedidoItem>()
                .HasOne(pi => pi.Pedido)
                .WithMany(p => p.PedidoItens)
                .HasForeignKey(pi => pi.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PedidoItem>()
                .HasOne(pi => pi.Produto)
                .WithMany(p => p.PedidoItens)
                .HasForeignKey(pi => pi.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Produto)
                .WithMany(p => p.Avaliacoes)
                .HasForeignKey(a => a.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Pedido)
                .WithMany(p => p.Avaliacoes)
                .HasForeignKey(a => a.PedidoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalheItem>()
                .HasOne(di => di.PedidoItem)
                .WithMany(pi => pi.DetalhesItem)
                .HasForeignKey(di => di.PedidoItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RespostaAvaliacao>()
                .HasOne(ra => ra.Avaliacao)
                .WithMany(a => a.Respostas)
                .HasForeignKey(ra => ra.AvaliacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Dados iniciais (Seed Data)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Empresas
            modelBuilder.Entity<Empresa>().HasData(
                new Empresa { Id = 1, Nome = "TechStore Brasil", CNPJ = "12.345.678/0001-90", Endereco = "Av. Paulista, 1000", DataCriacao = new DateTime(2019, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
                new Empresa { Id = 2, Nome = "MegaShop Ltda", CNPJ = "98.765.432/0001-10", Endereco = "Rua das Flores, 500", DataCriacao = new DateTime(2021, 3, 20, 0, 0, 0, DateTimeKind.Utc) }
            );

            // Lojas
            modelBuilder.Entity<Loja>().HasData(
                new Loja { Id = 1, Nome = "TechStore SP", Endereco = "Shopping Ibirapuera", Telefone = "(11) 1234-5678", DataAbertura = new DateTime(2020, 5, 10, 0, 0, 0, DateTimeKind.Utc), EmpresaId = 1 },
                new Loja { Id = 2, Nome = "TechStore RJ", Endereco = "Shopping Leblon", Telefone = "(21) 8765-4321", DataAbertura = new DateTime(2022, 8, 15, 0, 0, 0, DateTimeKind.Utc), EmpresaId = 1 },
                new Loja { Id = 3, Nome = "MegaShop Centro", Endereco = "Rua XV de Novembro, 200", Telefone = "(11) 5555-1234", DataAbertura = new DateTime(2023, 2, 28, 0, 0, 0, DateTimeKind.Utc), EmpresaId = 2 }
            );

            // Produtos
            modelBuilder.Entity<Produto>().HasData(
                new Produto { Id = 1, Nome = "Smartphone Galaxy", Descricao = "Smartphone Android com 128GB", Preco = 1299.99m, QuantidadeEstoque = 50, Categoria = "Eletrônicos", DataCadastro = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc), LojaId = 1 },
                new Produto { Id = 2, Nome = "Notebook Dell", Descricao = "Notebook i7 16GB RAM 512GB SSD", Preco = 3499.99m, QuantidadeEstoque = 25, Categoria = "Informática", DataCadastro = new DateTime(2024, 8, 15, 0, 0, 0, DateTimeKind.Utc), LojaId = 1 },
                new Produto { Id = 3, Nome = "Fone Bluetooth", Descricao = "Fone sem fio com cancelamento de ruído", Preco = 299.99m, QuantidadeEstoque = 100, Categoria = "Acessórios", DataCadastro = new DateTime(2024, 10, 10, 0, 0, 0, DateTimeKind.Utc), LojaId = 2 }
            );

            // Pedidos
            modelBuilder.Entity<Pedido>().HasData(
                new Pedido { Id = 1, NomeCliente = "João Silva", EmailCliente = "joao@email.com", ValorTotal = 1599.98m, DataPedido = new DateTime(2024, 12, 15, 0, 0, 0, DateTimeKind.Utc), Status = "Entregue" },
                new Pedido { Id = 2, NomeCliente = "Maria Santos", EmailCliente = "maria@email.com", ValorTotal = 3499.99m, DataPedido = new DateTime(2024, 12, 20, 0, 0, 0, DateTimeKind.Utc), Status = "Enviado" }
            );
        }
    }
}
```

---

## 🗃️ PARTE 5: Configuração do Banco de Dados

### Passo 8: Configurar PostgreSQL

#### 8.1: Criar banco de dados no PostgreSQL
```sql
-- Conectar ao PostgreSQL como superusuário
-- Criar banco de dados
CREATE DATABASE "ODataDB";

-- Criar usuário (se necessário)
CREATE USER postgres WITH PASSWORD '123456';

-- Dar permissões
GRANT ALL PRIVILEGES ON DATABASE "ODataDB" TO postgres;
```

### Passo 9: Instalar Entity Framework Tools e criar migrations

```bash
# Instalar EF Tools globalmente
dotnet tool install --global dotnet-ef

# Navegar para o projeto principal
cd ODataAPI

# Criar migration inicial
dotnet ef migrations add InitialCreate

# Aplicar migration ao banco
dotnet ef database update
```

---

## ⚙️ PARTE 6: Configuração do Program.cs

### Passo 10: Configurar Program.cs
```csharp
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using ODataAPI.Data;
using ODataAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework com PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=ODataDB;Username=postgres;Password=123456"));

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
builder.Services.AddControllers().AddOData(
    options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null)
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
```

---

## 🎮 PARTE 7: Criação dos Controllers

### Passo 11: Criar pasta Controllers e os controllers OData

```bash
mkdir Controllers
```

#### 11.1: Criar EmpresasController.cs
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataAPI.Data;
using ODataAPI.Models;

namespace ODataAPI.Controllers
{
    public class EmpresasController : ODataController
    {
        private readonly ApplicationDbContext _context;

        public EmpresasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<Empresa> Get()
        {
            return _context.Empresas.Include(e => e.Lojas);
        }

        [EnableQuery]
        public SingleResult<Empresa> Get([FromRoute] int key)
        {
            return SingleResult.Create(_context.Empresas.Where(e => e.Id == key).Include(e => e.Lojas));
        }

        public async Task<IActionResult> Post([FromBody] Empresa empresa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            return Created(empresa);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Empresa empresa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != empresa.Id)
            {
                return BadRequest();
            }

            _context.Entry(empresa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpresaExists(key))
                {
                    return NotFound();
                }
                throw;
            }

            return Updated(empresa);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var empresa = await _context.Empresas.FindAsync(key);
            if (empresa == null)
            {
                return NotFound();
            }

            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmpresaExists(int id)
        {
            return _context.Empresas.Any(e => e.Id == id);
        }
    }
}
```

#### 11.2: Criar LojasController.cs
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataAPI.Data;
using ODataAPI.Models;

namespace ODataAPI.Controllers
{
    public class LojasController : ODataController
    {
        private readonly ApplicationDbContext _context;

        public LojasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<Loja> Get()
        {
            return _context.Lojas.Include(l => l.Empresa).Include(l => l.Produtos);
        }

        [EnableQuery]
        public SingleResult<Loja> Get([FromRoute] int key)
        {
            return SingleResult.Create(_context.Lojas.Where(l => l.Id == key)
                .Include(l => l.Empresa).Include(l => l.Produtos));
        }

        public async Task<IActionResult> Post([FromBody] Loja loja)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Lojas.Add(loja);
            await _context.SaveChangesAsync();

            return Created(loja);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Loja loja)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != loja.Id)
            {
                return BadRequest();
            }

            _context.Entry(loja).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LojaExists(key))
                {
                    return NotFound();
                }
                throw;
            }

            return Updated(loja);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var loja = await _context.Lojas.FindAsync(key);
            if (loja == null)
            {
                return NotFound();
            }

            _context.Lojas.Remove(loja);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LojaExists(int id)
        {
            return _context.Lojas.Any(l => l.Id == id);
        }
    }
}
```

#### 11.3: Criar ProdutosController.cs
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataAPI.Data;
using ODataAPI.Models;

namespace ODataAPI.Controllers
{
    public class ProdutosController : ODataController
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<Produto> Get()
        {
            return _context.Produtos.Include(p => p.Loja).Include(p => p.Avaliacoes);
        }

        [EnableQuery]
        public SingleResult<Produto> Get([FromRoute] int key)
        {
            return SingleResult.Create(_context.Produtos.Where(p => p.Id == key)
                .Include(p => p.Loja).Include(p => p.Avaliacoes));
        }

        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return Created(produto);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Produto produto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != produto.Id)
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(key))
                {
                    return NotFound();
                }
                throw;
            }

            return Updated(produto);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var produto = await _context.Produtos.FindAsync(key);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(p => p.Id == id);
        }
    }
}
```

---

## 🧪 PARTE 8: Configuração dos Testes

### Passo 12: Criar TestWebApplicationFactory.cs
```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ODataAPI.Data;

namespace ODataAPI.Tests
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o DbContext existente
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona DbContext em memória para testes
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Cria o banco de dados em memória
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        }
    }
}
```

### Passo 13: Criar testes de integração
```csharp
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ODataAPI.Data;
using ODataAPI.Models;
using System.Net.Http.Json;

namespace ODataAPI.Tests
{
    public class IntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory<Program> _factory;

        public IntegrationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Get_Empresas_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/odata/Empresas");

            // Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
        }

        [Fact]
        public async Task Get_Produtos_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/odata/Produtos");

            // Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
        }

        [Fact]
        public async Task Post_Empresa_CreatesNewEmpresa()
        {
            // Arrange
            var novaEmpresa = new Empresa
            {
                Nome = "Teste Empresa",
                CNPJ = "11.111.111/0001-11",
                Endereco = "Rua Teste, 123",
                DataCriacao = DateTime.UtcNow
            };

            // Act
            var response = await _client.PostAsJsonAsync("/odata/Empresas", novaEmpresa);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task OData_Filter_Works()
        {
            // Act
            var response = await _client.GetAsync("/odata/Produtos?$filter=Preco gt 1000");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task OData_Expand_Works()
        {
            // Act
            var response = await _client.GetAsync("/odata/Lojas?$expand=Empresa,Produtos");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
```

### Passo 14: Criar testes unitários dos controllers
```csharp
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ODataAPI.Controllers;
using ODataAPI.Data;
using ODataAPI.Models;

namespace ODataAPI.Tests
{
    public class ControllerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void EmpresasController_Get_ReturnsAllEmpresas()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var controller = new EmpresasController(context);

            // Act
            var result = controller.Get();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public void ProdutosController_Get_ReturnsAllProdutos()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var controller = new ProdutosController(context);

            // Act
            var result = controller.Get();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task EmpresasController_Post_CreatesNewEmpresa()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var controller = new EmpresasController(context);
            var novaEmpresa = new Empresa
            {
                Nome = "Nova Empresa",
                CNPJ = "22.222.222/0001-22",
                Endereco = "Nova Rua, 456",
                DataCriacao = DateTime.UtcNow
            };

            // Act
            var result = await controller.Post(novaEmpresa);

            // Assert
            result.Should().NotBeNull();
            context.Empresas.Should().Contain(e => e.Nome == "Nova Empresa");
        }
    }
}
```

---

## 🚀 PARTE 9: Execução e Testes

### Passo 15: Executar o projeto

```bash
# Navegar para o projeto principal
cd ODataAPI

# Executar a aplicação
dotnet run
```

### Passo 16: Testar os endpoints

A API estará disponível em: `https://localhost:5001` ou `http://localhost:5000`

#### Exemplos de URLs para testar:

**Básicas:**
- `GET /odata/Empresas` - Listar todas as empresas
- `GET /odata/Lojas` - Listar todas as lojas
- `GET /odata/Produtos` - Listar todos os produtos

**Com filtros OData:**
- `GET /odata/Produtos?$filter=Preco gt 1000` - Produtos com preço > 1000
- `GET /odata/Produtos?$filter=Categoria eq 'Eletrônicos'` - Produtos eletrônicos
- `GET /odata/Lojas?$filter=EmpresaId eq 1` - Lojas da empresa 1

**Com expansão:**
- `GET /odata/Lojas?$expand=Empresa` - Lojas com dados da empresa
- `GET /odata/Produtos?$expand=Loja` - Produtos com dados da loja
- `GET /odata/Lojas?$expand=Empresa,Produtos` - Lojas com empresa e produtos

**Com ordenação:**
- `GET /odata/Produtos?$orderby=Preco desc` - Produtos por preço decrescente
- `GET /odata/Empresas?$orderby=DataCriacao` - Empresas por data de criação

### Passo 17: Executar os testes

```bash
# Navegar para o projeto de testes
cd ../ODataAPI.Tests

# Executar todos os testes
dotnet test

# Executar testes com detalhes
dotnet test --verbosity normal

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

---

## 📁 PARTE 10: Estrutura Final do Projeto

Após seguir todos os passos, sua estrutura de projeto deve ficar assim:

```
ODataProject/
├── ODataSolution.sln
├── ODataAPI/
│   ├── Controllers/
│   │   ├── EmpresasController.cs
│   │   ├── LojasController.cs
│   │   └── ProdutosController.cs
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Models/
│   │   ├── Empresa.cs
│   │   ├── Loja.cs
│   │   ├── Produto.cs
│   │   ├── Pedido.cs
│   │   ├── PedidoItem.cs
│   │   ├── Avaliacao.cs
│   │   ├── DetalheItem.cs
│   │   └── RespostaAvaliacao.cs
│   ├── Migrations/
│   │   └── [arquivos de migration]
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── ODataAPI.csproj
│   └── Program.cs
└── ODataAPI.Tests/
    ├── ControllerTests.cs
    ├── IntegrationTests.cs
    ├── TestWebApplicationFactory.cs
    └── ODataAPI.Tests.csproj
```

---

## 🎯 Conclusão

Seguindo este guia passo a passo, você terá criado uma API OData completa com:

✅ **8 entidades** com relacionamentos complexos  
✅ **Entity Framework Core** com PostgreSQL  
✅ **Migrations** para criação do banco  
✅ **Dados de exemplo** (seed data)  
✅ **Controllers OData** com CRUD completo  
✅ **Testes unitários** e de integração  
✅ **Suporte completo a queries OData** ($filter, $expand, $orderby, etc.)

### Próximos passos sugeridos:
- Adicionar autenticação e autorização
- Implementar logging estruturado
- Adicionar validações customizadas
- Configurar Docker para containerização
- Implementar cache com Redis
- Adicionar documentação com Swagger/OpenAPI

### Recursos úteis:
- [Documentação OData](https://docs.microsoft.com/en-us/odata/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [PostgreSQL](https://www.postgresql.org/docs/)

---

**Autor:** Weberson Rodrigues  
**Data:** 02/09/2025  
**Versão:** 1.0