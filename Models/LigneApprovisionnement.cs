using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionApprovisionnements.Models
{
    public class LigneApprovisionnement
    {
        public int Id { get; set; }

        [Required]
        public int ApprovisionnementId { get; set; }

        [Required]
        [Display(Name = "Article")]
        public int ArticleId { get; set; }

        [Required]
        [Display(Name = "Quantité")]
        [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être supérieure à 0")]
        public int Quantite { get; set; }

        [Required]
        [Display(Name = "Prix unitaire (FCFA)")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
        public decimal PrixUnitaire { get; set; }

        [Display(Name = "Montant (FCFA)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Montant => Quantite * PrixUnitaire;

        // Navigation
        [ForeignKey("ApprovisionnementId")]
        public virtual Approvisionnement? Approvisionnement { get; set; }

        [ForeignKey("ArticleId")]
        public virtual Article? Article { get; set; }
    }
}
