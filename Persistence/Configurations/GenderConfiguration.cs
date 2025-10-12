using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class GenderConfiguration : IEntityTypeConfiguration<GenderEntity>
{
    public void Configure(EntityTypeBuilder<GenderEntity> builder)
    {
        builder.HasKey(g => g.Id);
        
        builder.Property(g => g.Id)
            .ValueGeneratedNever();
            
        builder.HasIndex(g => g.Name)
            .IsUnique();
        
        builder.HasMany(g => g.Users)
            .WithOne(u => u.Gender)
            .HasForeignKey(u => u.GenderId)
            .OnDelete(DeleteBehavior.Restrict);

        var genders = Enum
            .GetValues<Gender>()
            .Select(g => new GenderEntity
            {
                Id = (int)g,
                Name = g.ToString()
            });
        
        builder.HasData(genders);
    }
}