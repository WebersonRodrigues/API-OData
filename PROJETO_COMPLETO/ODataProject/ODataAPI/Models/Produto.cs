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
        [Required]
        public int LojaId { get; set; }
        
        // Relacionamentos
        public virtual Loja Loja { get; set; } = null!;
        public virtual ICollection<PedidoItem> PedidoItens { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
    }
}