using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ODataAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CNPJ = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Endereco = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeCliente = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmailCliente = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lojas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Endereco = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lojas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lojas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Preco = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    QuantidadeEstoque = table.Column<int>(type: "integer", nullable: false),
                    Categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LojaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Lojas_LojaId",
                        column: x => x.LojaId,
                        principalTable: "Lojas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Avaliacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nota = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    NomeAvaliador = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DataAvaliacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    PedidoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    PedidoId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RespostasAvaliacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Resposta = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    NomeResponsavel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CargoResponsavel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataResposta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AvaliacaoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespostasAvaliacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespostasAvaliacao_Avaliacoes_AvaliacaoId",
                        column: x => x.AvaliacaoId,
                        principalTable: "Avaliacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalhesItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoDetalhe = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PedidoItemId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalhesItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalhesItem_PedidoItens_PedidoItemId",
                        column: x => x.PedidoItemId,
                        principalTable: "PedidoItens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Empresas",
                columns: new[] { "Id", "CNPJ", "DataCriacao", "Endereco", "Nome" },
                values: new object[,]
                {
                    { 1, "12.345.678/0001-90", new DateTime(2019, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Av. Paulista, 1000", "TechStore Brasil" },
                    { 2, "98.765.432/0001-10", new DateTime(2021, 3, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Rua das Flores, 500", "MegaShop Ltda" }
                });

            migrationBuilder.InsertData(
                table: "Pedidos",
                columns: new[] { "Id", "DataPedido", "EmailCliente", "NomeCliente", "Status", "ValorTotal" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 12, 15, 0, 0, 0, 0, DateTimeKind.Utc), "joao@email.com", "João Silva", "Entregue", 1599.98m },
                    { 2, new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Utc), "maria@email.com", "Maria Santos", "Enviado", 3499.99m }
                });

            migrationBuilder.InsertData(
                table: "Lojas",
                columns: new[] { "Id", "DataAbertura", "EmpresaId", "Endereco", "Nome", "Telefone" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Shopping Ibirapuera", "TechStore SP", "(11) 1234-5678" },
                    { 2, new DateTime(2022, 8, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Shopping Leblon", "TechStore RJ", "(21) 8765-4321" },
                    { 3, new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Rua XV de Novembro, 200", "MegaShop Centro", "(11) 5555-1234" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Categoria", "DataCadastro", "Descricao", "LojaId", "Nome", "Preco", "QuantidadeEstoque" },
                values: new object[,]
                {
                    { 1, "Eletrônicos", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Smartphone Android com 128GB", 1, "Smartphone Galaxy", 1299.99m, 50 },
                    { 2, "Informática", new DateTime(2024, 8, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Notebook i7 16GB RAM 512GB SSD", 1, "Notebook Dell", 3499.99m, 25 },
                    { 3, "Acessórios", new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Fone sem fio com cancelamento de ruído", 2, "Fone Bluetooth", 299.99m, 100 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_PedidoId",
                table: "Avaliacoes",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_ProdutoId",
                table: "Avaliacoes",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalhesItem_PedidoItemId",
                table: "DetalhesItem",
                column: "PedidoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Lojas_EmpresaId",
                table: "Lojas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_PedidoId",
                table: "PedidoItens",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_ProdutoId",
                table: "PedidoItens",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_LojaId",
                table: "Produtos",
                column: "LojaId");

            migrationBuilder.CreateIndex(
                name: "IX_RespostasAvaliacao_AvaliacaoId",
                table: "RespostasAvaliacao",
                column: "AvaliacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalhesItem");

            migrationBuilder.DropTable(
                name: "RespostasAvaliacao");

            migrationBuilder.DropTable(
                name: "PedidoItens");

            migrationBuilder.DropTable(
                name: "Avaliacoes");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Lojas");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
