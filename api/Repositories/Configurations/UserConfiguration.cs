using api.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Repositories.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);

            // Configure the Id column
            builder.Property(x => x.Id).HasColumnName("Id").ValueGeneratedOnAdd(); // Ensure ID is auto-generated

            // Configure the Username column
            builder
                .Property(x => x.Username)
                .HasColumnName("Username")
                .IsRequired() // Username is required
                .HasMaxLength(50); // Set maximum length

            // Add a unique index to the Username column
            builder.HasIndex(x => x.Username).IsUnique();

            // Configure the Password column
            builder
                .Property(x => x.Password)
                .HasColumnName("Password")
                .IsRequired() // Password is required
                .HasMaxLength(100); // Set maximum length

            // Additional configurations can be added here
        }
    }
}
