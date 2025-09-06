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
        public virtual ICollection<PedidoItem> PedidoItens { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
    }
}