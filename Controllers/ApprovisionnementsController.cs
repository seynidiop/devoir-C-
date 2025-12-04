using GestionApprovisionnements.Data;
using GestionApprovisionnements.Models;
using GestionApprovisionnements.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionApprovisionnements.Controllers
{
    public class ApprovisionnementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;

        public ApprovisionnementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Approvisionnements
        public async Task<IActionResult> Index(ApprovisionnementListViewModel model)
        {
            // Initialiser les dates par défaut si non spécifiées
            model.DateDebut ??= DateTime.Today.AddMonths(-1);
            model.DateFin ??= DateTime.Today;
            model.Page = model.Page < 1 ? 1 : model.Page;

            // Requête de base
            var query = _context.Approvisionnements
                .Include(a => a.Fournisseur)
                .Include(a => a.Lignes)
                    .ThenInclude(l => l.Article)
                .AsQueryable();

            // Appliquer les filtres
            if (!string.IsNullOrWhiteSpace(model.Recherche))
            {
                var recherche = model.Recherche.ToLower();
                query = query.Where(a =>
                    a.Reference.ToLower().Contains(recherche) ||
                    (a.Fournisseur != null && a.Fournisseur.Nom.ToLower().Contains(recherche)));
            }

            if (model.FournisseurId.HasValue && model.FournisseurId > 0)
            {
                query = query.Where(a => a.FournisseurId == model.FournisseurId);
            }

            if (model.ArticleId.HasValue && model.ArticleId > 0)
            {
                query = query.Where(a => a.Lignes.Any(l => l.ArticleId == model.ArticleId));
            }

            if (model.DateDebut.HasValue)
            {
                query = query.Where(a => a.DateApprovisionnement >= model.DateDebut.Value);
            }

            if (model.DateFin.HasValue)
            {
                query = query.Where(a => a.DateApprovisionnement <= model.DateFin.Value);
            }

            // Tri
            query = model.TriPar switch
            {
                "date_asc" => query.OrderBy(a => a.DateApprovisionnement),
                "date_desc" => query.OrderByDescending(a => a.DateApprovisionnement),
                "montant_asc" => query.OrderBy(a => a.MontantTotal),
                "montant_desc" => query.OrderByDescending(a => a.MontantTotal),
                "reference" => query.OrderBy(a => a.Reference),
                _ => query.OrderByDescending(a => a.DateApprovisionnement)
            };

            // Statistiques (sur les données filtrées)
            var statsQuery = query;
            model.TotalApprovisionnements = await statsQuery.SumAsync(a => a.MontantTotal);
            model.NombreApprovisionnements = await statsQuery.CountAsync();

            // Fournisseur principal
            var fournisseurStats = await statsQuery
                .GroupBy(a => new { a.FournisseurId, a.Fournisseur!.Nom })
                .Select(g => new
                {
                    Nom = g.Key.Nom,
                    Total = g.Sum(a => a.MontantTotal)
                })
                .OrderByDescending(f => f.Total)
                .FirstOrDefaultAsync();

            if (fournisseurStats != null)
            {
                model.FournisseurPrincipal = fournisseurStats.Nom;
                model.MontantFournisseurPrincipal = fournisseurStats.Total;
                model.PourcentageFournisseurPrincipal = model.TotalApprovisionnements > 0
                    ? Math.Round(fournisseurStats.Total / model.TotalApprovisionnements * 100, 1)
                    : 0;
            }

            // Pagination
            model.TotalItems = await query.CountAsync();
            model.TotalPages = (int)Math.Ceiling(model.TotalItems / (double)PageSize);
            model.PageSize = PageSize;

            model.Approvisionnements = await query
                .Skip((model.Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Listes pour les filtres
            model.Fournisseurs = new SelectList(
                await _context.Fournisseurs.OrderBy(f => f.Nom).ToListAsync(),
                "Id", "Nom", model.FournisseurId);

            model.Articles = new SelectList(
                await _context.Articles.OrderBy(a => a.Nom).ToListAsync(),
                "Id", "Nom", model.ArticleId);

            return View(model);
        }

        // GET: Approvisionnements/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new ApprovisionnementCreateViewModel
            {
                DateApprovisionnement = DateTime.Today,
                Fournisseurs = new SelectList(
                    await _context.Fournisseurs.OrderBy(f => f.Nom).ToListAsync(),
                    "Id", "Nom"),
                Articles = new SelectList(
                    await _context.Articles.OrderBy(a => a.Nom).ToListAsync(),
                    "Id", "Nom")
            };

            return View(viewModel);
        }

        // POST: Approvisionnements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApprovisionnementCreateViewModel viewModel)
        {
            if (viewModel.Lignes == null || !viewModel.Lignes.Any())
            {
                ModelState.AddModelError("", "Veuillez ajouter au moins un article.");
            }

            if (ModelState.IsValid)
            {
                // Générer la référence automatiquement
                var annee = DateTime.Now.Year;
                var dernierNumero = await _context.Approvisionnements
                    .Where(a => a.Reference.StartsWith($"APP-{annee}-"))
                    .Select(a => a.Reference)
                    .OrderByDescending(r => r)
                    .FirstOrDefaultAsync();

                int numero = 1;
                if (!string.IsNullOrEmpty(dernierNumero))
                {
                    var parts = dernierNumero.Split('-');
                    if (parts.Length == 3 && int.TryParse(parts[2], out int lastNum))
                    {
                        numero = lastNum + 1;
                    }
                }

                var approvisionnement = new Approvisionnement
                {
                    Reference = $"APP-{annee}-{numero:D3}",
                    DateApprovisionnement = viewModel.DateApprovisionnement,
                    FournisseurId = viewModel.FournisseurId,
                    Observations = viewModel.Observations,
                    Statut = StatutApprovisionnement.EnAttente
                };

                // Ajouter les lignes
                decimal montantTotal = 0;
                foreach (var ligne in viewModel.Lignes)
                {
                    var ligneAppro = new LigneApprovisionnement
                    {
                        ArticleId = ligne.ArticleId,
                        Quantite = ligne.Quantite,
                        PrixUnitaire = ligne.PrixUnitaire
                    };
                    approvisionnement.Lignes.Add(ligneAppro);
                    montantTotal += ligne.Quantite * ligne.PrixUnitaire;
                }

                approvisionnement.MontantTotal = montantTotal;

                _context.Approvisionnements.Add(approvisionnement);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Approvisionnement créé avec succès.";
                return RedirectToAction(nameof(Index));
            }

            // Recharger les listes en cas d'erreur
            viewModel.Fournisseurs = new SelectList(
                await _context.Fournisseurs.OrderBy(f => f.Nom).ToListAsync(),
                "Id", "Nom", viewModel.FournisseurId);
            viewModel.Articles = new SelectList(
                await _context.Articles.OrderBy(a => a.Nom).ToListAsync(),
                "Id", "Nom");

            return View(viewModel);
        }

        // GET: Approvisionnements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var approvisionnement = await _context.Approvisionnements
                .Include(a => a.Fournisseur)
                .Include(a => a.Lignes)
                    .ThenInclude(l => l.Article)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (approvisionnement == null)
                return NotFound();

            return View(approvisionnement);
        }

        // GET: Approvisionnements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var approvisionnement = await _context.Approvisionnements
                .Include(a => a.Lignes)
                    .ThenInclude(l => l.Article)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (approvisionnement == null)
                return NotFound();

            var viewModel = new ApprovisionnementCreateViewModel
            {
                Id = approvisionnement.Id,
                DateApprovisionnement = approvisionnement.DateApprovisionnement,
                FournisseurId = approvisionnement.FournisseurId,
                Reference = approvisionnement.Reference,
                Observations = approvisionnement.Observations,
                Lignes = approvisionnement.Lignes.Select(l => new LigneApprovisionnementViewModel
                {
                    Id = l.Id,
                    ArticleId = l.ArticleId,
                    ArticleNom = l.Article?.Nom,
                    Quantite = l.Quantite,
                    PrixUnitaire = l.PrixUnitaire
                }).ToList(),
                Fournisseurs = new SelectList(
                    await _context.Fournisseurs.OrderBy(f => f.Nom).ToListAsync(),
                    "Id", "Nom", approvisionnement.FournisseurId),
                Articles = new SelectList(
                    await _context.Articles.OrderBy(a => a.Nom).ToListAsync(),
                    "Id", "Nom")
            };

            return View(viewModel);
        }

        // POST: Approvisionnements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApprovisionnementCreateViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (viewModel.Lignes == null || !viewModel.Lignes.Any())
            {
                ModelState.AddModelError("", "Veuillez ajouter au moins un article.");
            }

            if (ModelState.IsValid)
            {
                var approvisionnement = await _context.Approvisionnements
                    .Include(a => a.Lignes)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (approvisionnement == null)
                    return NotFound();

                approvisionnement.DateApprovisionnement = viewModel.DateApprovisionnement;
                approvisionnement.FournisseurId = viewModel.FournisseurId;
                approvisionnement.Observations = viewModel.Observations;

                // Supprimer les anciennes lignes
                _context.LignesApprovisionnement.RemoveRange(approvisionnement.Lignes);

                // Ajouter les nouvelles lignes
                decimal montantTotal = 0;
                foreach (var ligne in viewModel.Lignes)
                {
                    var ligneAppro = new LigneApprovisionnement
                    {
                        ApprovisionnementId = id,
                        ArticleId = ligne.ArticleId,
                        Quantite = ligne.Quantite,
                        PrixUnitaire = ligne.PrixUnitaire
                    };
                    _context.LignesApprovisionnement.Add(ligneAppro);
                    montantTotal += ligne.Quantite * ligne.PrixUnitaire;
                }

                approvisionnement.MontantTotal = montantTotal;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Approvisionnement modifié avec succès.";
                return RedirectToAction(nameof(Index));
            }

            // Recharger les listes
            viewModel.Fournisseurs = new SelectList(
                await _context.Fournisseurs.OrderBy(f => f.Nom).ToListAsync(),
                "Id", "Nom", viewModel.FournisseurId);
            viewModel.Articles = new SelectList(
                await _context.Articles.OrderBy(a => a.Nom).ToListAsync(),
                "Id", "Nom");

            return View(viewModel);
        }

        // POST: Approvisionnements/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var approvisionnement = await _context.Approvisionnements.FindAsync(id);
            if (approvisionnement != null)
            {
                _context.Approvisionnements.Remove(approvisionnement);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Approvisionnement supprimé avec succès.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Approvisionnements/UpdateStatut/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatut(int id, StatutApprovisionnement statut)
        {
            var approvisionnement = await _context.Approvisionnements.FindAsync(id);
            if (approvisionnement != null)
            {
                approvisionnement.Statut = statut;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Statut mis à jour avec succès.";
            }

            return RedirectToAction(nameof(Index));
        }

        // API pour obtenir le prix d'un article
        [HttpGet]
        public async Task<IActionResult> GetArticlePrix(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
                return NotFound();

            return Json(new { prixUnitaire = article.PrixUnitaire });
        }
    }
}
