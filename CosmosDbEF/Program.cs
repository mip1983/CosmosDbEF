using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CosmosDbEF;

internal class Program
{
    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    public static void Main(string[] args)
    {
        TestEf().GetAwaiter().GetResult();
    }

    private static async Task TestEf()
    {
        Console.WriteLine("Writing and reading entities with Ulid and DateTimeRange (+ nullable)...");
        Console.WriteLine();

        using (var context = new TestDbContext(new DbContextOptions<TestDbContext>()))
        {
            // Initialize the database (ensure the collection exists)
            await context.Database.EnsureCreatedAsync();

            // Create two new entities, one with a range and nullable range populated, one with just range populated
            var entity1 = new Entity
            {
                Range = new DateTimeRange(DateTime.Now, DateTime.Now.AddDays(10)),
                NullableRange = new DateTimeRange(DateTime.Now, DateTime.Now.AddDays(10))
            };

            var entity2 = new Entity
            {
                Range = new DateTimeRange(DateTime.Now, DateTime.Now.AddDays(10))
            };

            // Save the entities to the database
            context.Entities.Add(entity1);
            context.Entities.Add(entity2);
            await context.SaveChangesAsync();

            // Read the entities from the database and output to the console as formatted/indented JSON, separated by line break
            var savedEntities = await context.Entities.ToListAsync();
            Console.WriteLine("The following entities where saved and read back from the db: ");
            Console.WriteLine();

            foreach (var entity in savedEntities)
            {                
                Console.WriteLine(JsonSerializer.Serialize(entity, jsonOptions));
                Console.WriteLine();
            }

            // Update the entities changing any ranges to +5 days and save the changes to the database
            foreach (var entity in savedEntities)
            {
                entity.Range = new DateTimeRange(entity.Range.From.AddDays(5), entity.Range.To.AddDays(5));
                if (entity.NullableRange.HasValue)
                {
                    entity.NullableRange = new DateTimeRange(entity.NullableRange.Value.From.AddDays(5), entity.NullableRange.Value.To.AddDays(5));
                }
            }
            await context.SaveChangesAsync();

            // Read the entities from the database and output to the console as formatted/indented JSON, separated by line break
            var updatedEntities = await context.Entities.ToListAsync();
            Console.WriteLine("The entities where updated with dates + 5 days and read back:");
            Console.WriteLine();

            foreach (var entity in updatedEntities)
            {                
                Console.WriteLine(JsonSerializer.Serialize(entity, jsonOptions));
                Console.WriteLine();
            }

            // Delete the entities from the database
            context.Entities.RemoveRange(updatedEntities);
            await context.SaveChangesAsync();

            // Drop the database
            await context.Database.EnsureDeletedAsync();
        }

        Console.WriteLine("Entities removed and database dropped.");
    }
}
