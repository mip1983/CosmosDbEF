using System.Text.Json.Serialization;

namespace CosmosDbEF;

[Serializable]
public struct DateTimeRange
{
    public DateTime From { get; set; }

    public DateTime To { get; set; }

    [JsonConstructor]
    public DateTimeRange()
    {
    }

    public DateTimeRange(DateTime from, DateTime to)
    {
        if (from > to)
        {
            throw new ArgumentException("From date must be less than or equal to To date.");
        }

        From = from;
        To = to;
    }

    public DateTimeRange(DateOnly from, DateOnly to)
    {
        if (from > to)
        {
            throw new ArgumentException("From date must be less than or equal to To date.");
        }

        From = from.ToDateTime(TimeOnly.MinValue);
        To = to.ToDateTime(TimeOnly.MaxValue);
    }

    public static DateTimeRange? Parse(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;

        var parts = value.Split(" - ");
        if (parts.Length != 2)
        {
            return null;
        }

        var from = DateTime.Parse(parts[0]);
        var to = DateTime.Parse(parts[1]);

        return new DateTimeRange(from, to);
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is DateTimeRange other)
        {
            return From == other.From && To == other.To;
        }

        return false;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(From, To);
    }

    public override readonly string ToString()
    {
        return $"{From} - {To}";
    }

    public readonly string ToString(string formatString, IFormatProvider? formatProvider = null)
    {
        var from = From.ToString(formatString, formatProvider);
        var to = To.ToString(formatString, formatProvider);

        return $"{from} - {to}";
    }

    public static bool operator ==(DateTimeRange left, DateTimeRange right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DateTimeRange left, DateTimeRange right)
    {
        return !left.Equals(right);
    }
}
