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