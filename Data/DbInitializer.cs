using GestionApprovisionnements.Models;

namespace GestionApprovisionnements.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // S'assurer que la base de données est créée
            context.Database.EnsureCreated();

            // Vérifier si des données existent déjà
            if (context.Fournisseurs.Any())
                return;

            // Créer les fournisseurs
            var fournisseurs = new Fournisseur[]
            {
                new Fournisseur
                {
                    Nom = "Textiles Dakar SARL",
                    Adresse = "Zone Industrielle, Dakar",
                    Telephone = "+221 33 123 45 67",
                    Email = "contact@textilesdakar.sn"
                },
                new Fournisseur
                {
                    Nom = "Mercerie Centrale",
                    Adresse = "Avenue Blaise Diagne, Dakar",
                    Telephone = "+221 33 987 65 43",
                    Email = "info@merceriecentrale.sn"
                },
                new Fournisseur
                {
                    Nom = "Tissus Premium",
                    Adresse = "Marché Sandaga, Dakar",
                    Telephone = "+221 77 555 44 33",
                    Email = "ventes@tissuspremium.sn"
                }
            };
            context.Fournisseurs.AddRange(fournisseurs);
            context.SaveChanges();

            // Créer les articles
            var articles = new Article[]
            {
                new Article { Nom = "Tissu Bazin Riche", Description = "Bazin de qualité supérieure", PrixUnitaire = 15000, StockActuel = 100 },
                new Article { Nom = "Tissu Wax Hollandais", Description = "Wax authentique", PrixUnitaire = 8000, StockActuel = 200 },
                new Article { Nom = "Fil à coudre (lot)", Description = "Lot de 12 bobines", PrixUnitaire = 2500, StockActuel = 50 },
                new Article { Nom = "Boutons (paquet 100)", Description = "Boutons assortis", PrixUnitaire = 1500, StockActuel = 80 },
                new Article { Nom = "Fermeture éclair 20cm", Description = "Fermetures diverses couleurs", PrixUnitaire = 500, StockActuel = 150 },
                new Article { Nom = "Dentelle brodée (mètre)", Description = "Dentelle de qualité", PrixUnitaire = 3000, StockActuel = 75 },
                new Article { Nom = "Tissu Thioup", Description = "Tissu traditionnel", PrixUnitaire = 12000, StockActuel = 60 },
                new Article { Nom = "Perles décoratives (sachet)", Description = "Perles pour broderie", PrixUnitaire = 2000, StockActuel = 40 }
            };
            context.Articles.AddRange(articles);
            context.SaveChanges();

            // Créer les approvisionnements avec leurs lignes
            var approvisionnements = new List<Approvisionnement>
            {
                new Approvisionnement
                {
                    Reference = "APP-2023-001",
                    DateApprovisionnement = new DateTime(2023, 4, 15),
                    FournisseurId = fournisseurs[0].Id,
                    Statut = StatutApprovisionnement.Recu,
                    Observations = "Commande urgente",
                    Lignes = new List<LigneApprovisionnement>
                    {
                        new LigneApprovisionnement { ArticleId = articles[0].Id, Quantite = 30, PrixUnitaire = 15000 },
                        new LigneApprovisionnement { ArticleId = articles[1].Id, Quantite = 25, PrixUnitaire = 8000 },
                        new LigneApprovisionnement { ArticleId = articles[6].Id, Quantite = 20, PrixUnitaire = 12000 }
                    }
                },
                new Approvisionnement
                {
                    Reference = "APP-2023-002",
                    DateApprovisionnement = new DateTime(2023, 4, 10),
                    FournisseurId = fournisseurs[1].Id,
                    Statut = StatutApprovisionnement.Recu,
                    Observations = "Réapprovisionnement mensuel",
                    Lignes = new List<LigneApprovisionnement>
                    {
                        new LigneApprovisionnement { ArticleId = articles[2].Id, Quantite = 50, PrixUnitaire = 2500 },
                        new LigneApprovisionnement { ArticleId = articles[3].Id, Quantite = 30, PrixUnitaire = 1500 },
                        new LigneApprovisionnement { ArticleId = articles[4].Id, Quantite = 100, PrixUnitaire = 500 },
                        new LigneApprovisionnement { ArticleId = articles[5].Id, Quantite = 40, PrixUnitaire = 3000 }
                    }
                },
                new Approvisionnement
                {
                    Reference = "APP-2023-003",
                    DateApprovisionnement = new DateTime(2023, 4, 5),
                    FournisseurId = fournisseurs[2].Id,
                    Statut = StatutApprovisionnement.EnAttente,
                    Observations = "En cours de livraison",
                    Lignes = new List<LigneApprovisionnement>
                    {
                        new LigneApprovisionnement { ArticleId = articles[0].Id, Quantite = 20, PrixUnitaire = 15000 },
                        new LigneApprovisionnement { ArticleId = articles[7].Id, Quantite = 25, PrixUnitaire = 2000 }
                    }
                },
                new Approvisionnement
                {
                    Reference = "APP-2023-004",
                    DateApprovisionnement = new DateTime(2023, 4, 1),
                    FournisseurId = fournisseurs[0].Id,
                    Statut = StatutApprovisionnement.Recu,
                    Observations = "",
                    Lignes = new List<LigneApprovisionnement>
                    {
                        new LigneApprovisionnement { ArticleId = articles[1].Id, Quantite = 50, PrixUnitaire = 8000 },
                        new LigneApprovisionnement { ArticleId = articles[6].Id, Quantite = 15, PrixUnitaire = 12000 },
                        new LigneApprovisionnement { ArticleId = articles[5].Id, Quantite = 30, PrixUnitaire = 3000 }
                    }
                },
                new Approvisionnement
                {
                    Reference = "APP-2023-005",
                    DateApprovisionnement = new DateTime(2023, 3, 25),
                    FournisseurId = fournisseurs[1].Id,
                    Statut = StatutApprovisionnement.Recu,
                    Observations = "Commande spéciale client",
                    Lignes = new List<LigneApprovisionnement>
                    {
                        new LigneApprovisionnement { ArticleId = articles[0].Id, Quantite = 25, PrixUnitaire = 15000 },
                        new LigneApprovisionnement { ArticleId = articles[2].Id, Quantite = 20, PrixUnitaire = 2500 },
                        new LigneApprovisionnement { ArticleId = articles[4].Id, Quantite = 80, PrixUnitaire = 500 }
                    }
                }
            };

            // Calculer le montant total pour chaque approvisionnement
            foreach (var appro in approvisionnements)
            {
                appro.MontantTotal = appro.Lignes.Sum(l => l.Quantite * l.PrixUnitaire);
            }

            context.Approvisionnements.AddRange(approvisionnements);
            context.SaveChanges();
        }
    }
}
