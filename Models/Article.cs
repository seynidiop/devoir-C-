using System.ComponentModel.DataAnnotations;

namespace GestionApprovisionnements.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [Display(Name = "Nom de l'article")]
        [StringLength(200)]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Prix unitaire (FCFA)")]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
        public decimal PrixUnitaire { get; set; }

        [Display(Name = "Stock actuel")]
        public int StockActuel { get; set; }

        // Navigation
        public virtual ICollection<LigneApprovisionnement> LignesApprovisionnement { get; set; } = new List<LigneApprovisionnement>();
    }
}
