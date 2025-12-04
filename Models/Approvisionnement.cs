using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionApprovisionnements.Models
{
    public enum StatutApprovisionnement
    {
        [Display(Name = "En attente")]
        EnAttente,
        [Display(Name = "Reçu")]
        Recu,
        [Display(Name = "Annulé")]
        Annule
    }

    public class Approvisionnement
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Référence")]
        [StringLength(50)]
        public string Reference { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Date d'approvisionnement")]
        [DataType(DataType.Date)]
        public DateTime DateApprovisionnement { get; set; }

        [Required]
        [Display(Name = "Fournisseur")]
        public int FournisseurId { get; set; }

        [Display(Name = "Observations")]
        [StringLength(1000)]
        public string? Observations { get; set; }

        [Display(Name = "Statut")]
        public StatutApprovisionnement Statut { get; set; } = StatutApprovisionnement.EnAttente;

        [Display(Name = "Montant total (FCFA)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantTotal { get; set; }

        // Navigation
        [ForeignKey("FournisseurId")]
        public virtual Fournisseur? Fournisseur { get; set; }

        public virtual ICollection<LigneApprovisionnement> Lignes { get; set; } = new List<LigneApprovisionnement>();

        // Propriété calculée pour le nombre d'articles
        [NotMapped]
        public int NombreArticles => Lignes?.Count ?? 0;
    }
}
