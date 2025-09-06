using Microsoft.EntityFrameworkCore;
using ODataAPI.Models;

namespace ODataAPI.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {

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

        private static void SeedData(ModelBuilder modelBuilder)
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