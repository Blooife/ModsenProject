using Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EventsUsersConfiguration : IEntityTypeConfiguration<EventsUsers>
{
    public void Configure(EntityTypeBuilder<EventsUsers> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
        
        builder.Property(e => e.RegistrationDate).IsRequired()
            .HasColumnType("timestamp without time zone");
        builder
            .HasOne(eu => eu.Event)
            .WithMany(e => e.EventsUsers)
            .HasForeignKey(eu => eu.EventId);

        builder
            .HasOne(eu => eu.User)
            .WithMany(u => u.EventsUsers)
            .HasForeignKey(eu => eu.UserId);
        
        builder
            .HasIndex(e => new { e.UserId, e.EventId })  
            .IsUnique(); 
    }
}