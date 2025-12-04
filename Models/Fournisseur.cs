using System.ComponentModel.DataAnnotations;

namespace GestionApprovisionnements.Models
{
    public class Fournisseur
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [Display(Name = "Nom du fournisseur")]
        [StringLength(200)]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Adresse")]
        [StringLength(500)]
        public string? Adresse { get; set; }

        [Display(Name = "Téléphone")]
        [StringLength(20)]
        public string? Telephone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        [StringLength(100)]
        public string? Email { get; set; }

        // Navigation
        public virtual ICollection<Approvisionnement> Approvisionnements { get; set; } = new List<Approvisionnement>();
    }
}
