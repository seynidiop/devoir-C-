using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionApprovisionnements.Models.ViewModels
{
    // ViewModel pour créer/éditer un approvisionnement
    public class ApprovisionnementCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La date est obligatoire")]
        [Display(Name = "Date d'approvisionnement")]
        [DataType(DataType.Date)]
        public DateTime DateApprovisionnement { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Le fournisseur est obligatoire")]
        [Display(Name = "Fournisseur")]
        public int FournisseurId { get; set; }

        [Display(Name = "Référence")]
        public string? Reference { get; set; }

        [Display(Name = "Observations")]
        [StringLength(1000)]
        public string? Observations { get; set; }

        // Lignes d'approvisionnement
        public List<LigneApprovisionnementViewModel> Lignes { get; set; } = new();

        // Listes pour les dropdowns
        public SelectList? Fournisseurs { get; set; }
        public SelectList? Articles { get; set; }
    }

    // ViewModel pour une ligne d'approvisionnement
    public class LigneApprovisionnementViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "L'article est obligatoire")]
        [Display(Name = "Article")]
        public int ArticleId { get; set; }

        public string? ArticleNom { get; set; }

        [Required(ErrorMessage = "La quantité est obligatoire")]
        [Display(Name = "Quantité")]
        [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être supérieure à 0")]
        public int Quantite { get; set; }

        [Required(ErrorMessage = "Le prix unitaire est obligatoire")]
        [Display(Name = "Prix unitaire (FCFA)")]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
        public decimal PrixUnitaire { get; set; }

        [Display(Name = "Montant (FCFA)")]
        public decimal Montant => Quantite * PrixUnitaire;
    }

    // ViewModel pour la liste et les filtres
    public class ApprovisionnementListViewModel
    {
        // Filtres
        public string? Recherche { get; set; }
        public int? FournisseurId { get; set; }
        public int? ArticleId { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public string? TriPar { get; set; } = "date_desc";

        // Données
        public List<Approvisionnement> Approvisionnements { get; set; } = new();

        // Listes pour les filtres
        public SelectList? Fournisseurs { get; set; }
        public SelectList? Articles { get; set; }

        // Statistiques
        public decimal TotalApprovisionnements { get; set; }
        public int NombreApprovisionnements { get; set; }
        public string? FournisseurPrincipal { get; set; }
        public decimal MontantFournisseurPrincipal { get; set; }
        public decimal PourcentageFournisseurPrincipal { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 10;
    }

    // ViewModel pour les statistiques du dashboard
    public class DashboardViewModel
    {
        public decimal TotalApprovisionnements { get; set; }
        public int NombreApprovisionnements { get; set; }
        public int NombreFournisseurs { get; set; }
        public int NombreArticles { get; set; }

        public List<FournisseurStatViewModel> TopFournisseurs { get; set; } = new();
        public List<ArticleStatViewModel> TopArticles { get; set; } = new();
        public List<ApprovisionnementMensuelViewModel> ApprovisionnementsParMois { get; set; } = new();
    }

    public class FournisseurStatViewModel
    {
        public string Nom { get; set; } = string.Empty;
        public decimal MontantTotal { get; set; }
        public int NombreApprovisionnements { get; set; }
        public decimal Pourcentage { get; set; }
    }

    public class ArticleStatViewModel
    {
        public string Nom { get; set; } = string.Empty;
        public int QuantiteTotale { get; set; }
        public decimal MontantTotal { get; set; }
    }

    public class ApprovisionnementMensuelViewModel
    {
        public string Mois { get; set; } = string.Empty;
        public decimal MontantTotal { get; set; }
        public int NombreApprovisionnements { get; set; }
    }
}
