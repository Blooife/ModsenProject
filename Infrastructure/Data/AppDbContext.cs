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
            
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
    }
    
    public virtual async Task<int> SaveChangesAsync()
    {
        var result = await base.SaveChangesAsync();

        return result;
    }
}