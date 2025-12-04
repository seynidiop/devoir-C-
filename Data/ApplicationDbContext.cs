using GestionApprovisionnements.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionApprovisionnements.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Fournisseur> Fournisseurs { get; set; }
        public DbSet<Approvisionnement> Approvisionnements { get; set; }
        public DbSet<LigneApprovisionnement> LignesApprovisionnement { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des relations
            modelBuilder.Entity<Approvisionnement>()
                .HasOne(a => a.Fournisseur)
                .WithMany(f => f.Approvisionnements)
                .HasForeignKey(a => a.FournisseurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LigneApprovisionnement>()
                .HasOne(l => l.Approvisionnement)
                .WithMany(a => a.Lignes)
                .HasForeignKey(l => l.ApprovisionnementId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LigneApprovisionnement>()
                .HasOne(l => l.Article)
                .WithMany(a => a.LignesApprovisionnement)
                .HasForeignKey(l => l.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            modelBuilder.Entity<Approvisionnement>()
                .HasIndex(a => a.Reference)
                .IsUnique();

            modelBuilder.Entity<Approvisionnement>()
                .HasIndex(a => a.DateApprovisionnement);
        }
    }
}
