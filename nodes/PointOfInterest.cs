using Godot;

/// <summary>
/// 
/// </summary>
/// <param name="Id">Point of interest identifier</param>
/// <param name="Size">Radius encompassing the point of interest.</param>
/// <param name="Deadzone">Radius in which no POI of the same type can spawn.</param>
public record PointOfInterestType(string Id, float Size, float Deadzone);

public class PointOfInterest
{
    public Vector2 Position { get; set; } = default!;
    public PointOfInterestType Type { get; set; } = default!;
    public float SizeSquared => Mathf.Pow(Type.Size, 2);
    public float DeadzoneSquared => Mathf.Pow(Type.Deadzone, 2);

    public PointOfInterest(Vector2 position, PointOfInterestType type)
    {
        Position = position;
        Type = type;
    }

    public float DistanceTo(PointOfInterest point)
    {
        return Position.DistanceTo(point.Position);
    }

    public float SquaredDistanceTo(PointOfInterest point)
    {
        return Mathf.Pow(DistanceTo(point), 2);
    }
}