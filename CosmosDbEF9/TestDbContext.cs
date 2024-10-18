using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CosmosDbEF9;

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

        // .NET/EF 9 now wants this...
        //modelBuilder.Entity<Entity>().ComplexProperty(o => o.Range);

        // This doesn't work on the nullable property
        // modelBuilder.Entity<Entity>().ComplexProperty(o => o.NullableRange);

        // Can do something with a string conversion, but this won't be JSON in the db as before, so won't be indexed/queryable.
        // Is there some way to map it that keeps it as JSON? Would rather it just worked as before in .NET/EF 8
        // modelBuilder.Entity<Entity>().Property(p => p.NullableRange).HasConversion(c => c.HasValue ? c.Value.ToString() : null, c => DateTimeRange.Parse(c));
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

    /// <summary>
    /// Ulid now doesn't work without this in .NET/EF 9
    /// </summary>
    //protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    //{
    //    configurationBuilder
    //    .Properties<Ulid>()
    //        .HaveConversion<UlidConverter>();
    //}
}