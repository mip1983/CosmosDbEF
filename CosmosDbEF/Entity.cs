namespace CosmosDbEF;

public class Entity
{
    public Ulid Id { get; set; } = Ulid.NewUlid();

    public string PartitionKey { get; set; } = "Entity";

    public required DateTimeRange Range { get; set; }

    public DateTimeRange? NullableRange { get; set; }
}
