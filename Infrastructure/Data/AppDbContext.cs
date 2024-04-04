using System.Reflection;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public virtual DbSet<Event> Events { get; set; }
    public virtual DbSet<ApplicationUser> Users { get; set; }
    
    public DbSet<EventsUsers> EventsUsers { get; set; }

    public AppDbContext() : base()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432; Database=modsen;Username=postgres;Password=postgres")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
    }
    
    public virtual async Task<int> SaveChangesAsync()
    {
        ChangeTracker.DetectChanges();

        var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Логика для BeforeSave
            if (entry.Entity is Event entity)
            {
                // Ваша логика перед сохранением для сущности YourEntity
            }
        }

        foreach (var entry in entries)
        {
            // Логика для AfterSave
            if (entry.Entity is EventsUsers entity)
            {
                // Ваша логика после сохранения для сущности YourEntity
            }
        }
        var result = await base.SaveChangesAsync();

        return result;
    }
}