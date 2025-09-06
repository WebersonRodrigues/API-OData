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
        public virtual ICollection<DetalheItem> DetalhesItem { get; set; } = [];
    }
}