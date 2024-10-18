using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CosmosDbEF;

public class TestDbContext(DbContextOptions options) : DbContext(options)
{
    public virtual DbSet<Entity> Entities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapEntity(modelBuilder);
    }

    protected static void MapEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entity>()
            .ToContainer("Entities")
            .HasNoDiscriminator()
            .HasPartitionKey(o => o.PartitionKey);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var endpoint = configuration["CosmosDb:Endpoint"];
        var key = configuration["CosmosDb:Key"];

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("Missing CosmosDb configuration in user secrets");
        }

        optionsBuilder.UseCosmos(endpoint, key, databaseName: "TestDb");
    }
}