using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [Required]
        public int EmpresaId { get; set; }
        
        // Relacionamentos
        public virtual Empresa Empresa { get; set; } = null!;
        public virtual ICollection<Produto> Produtos { get; set; } = [];
    }
}