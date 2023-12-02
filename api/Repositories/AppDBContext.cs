using System.Reflection;
using api.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    /// <summary>
    /// The application database context responsible for interacting with the database.
    /// </summary>
    public class AppDBContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDBContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by a DbContext.</param>
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options ?? throw new ArgumentNullException(nameof(options))) { }

        /// <summary>
        /// Gets or sets the DbSet for Users.
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Configures the schema needed for the context by applying configurations from the specified assembly.
        /// </summary>
        /// <param name="modelBuilder">Defines the shape of your entities, the relationships between them, and how they map to the database.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
