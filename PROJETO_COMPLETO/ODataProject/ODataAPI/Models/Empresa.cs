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
        public virtual ICollection<Loja> Lojas { get; set; } = [];
    }
}