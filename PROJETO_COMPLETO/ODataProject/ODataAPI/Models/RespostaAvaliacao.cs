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