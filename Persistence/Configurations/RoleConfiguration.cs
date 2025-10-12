using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(g => g.Id)
            .ValueGeneratedNever();
            
        builder.HasIndex(r => r.Name)
            .IsUnique();
        
        builder.HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        var roles = Enum
            .GetValues<Role>()
            .Select(r => new RoleEntity
            {
                Id = (int)r,
                Name = r.ToString()
            });

        builder.HasData(roles);
    }
}