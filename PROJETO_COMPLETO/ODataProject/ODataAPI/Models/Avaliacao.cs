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
        public virtual ICollection<RespostaAvaliacao> Respostas { get; set; } = [];
    }
}