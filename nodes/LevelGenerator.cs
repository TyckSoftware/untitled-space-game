using Godot;
using System.Collections.Generic;

public partial class LevelGenerator : Node
{
    public List<PointOfInterestType> Types { get; set; } = new List<PointOfInterestType>();

    [Export]
    public float SizeMultiplier { get; set; } = 100.0f;

    /// <inheritdoc />
    public override void _Ready()
    {
        // Setting up points of interest
        int pointCount = 64;

        Types.Add(new PointOfInterestType("Star", 2.0f, 4.0f));
        Types.Add(new PointOfInterestType("SpaceStation", 10f, 40f));
        Types.Add(new PointOfInterestType("Loot", 0.5f, 20f));

        // Generate points of interest
        List<PointOfInterest> poi = GeneratePointsOfInterest(pointCount);

        // Visualise points of interest
        Node3D container = GetNode<Node3D>("../Container");

        foreach (PointOfInterest p in poi)
        {
            CSGSphere3D sphere = new CSGSphere3D();
            sphere.Radius = p.Type.Size;
            sphere.Position = new Vector3(p.Position.x, 0, p.Position.y);

            container.AddChild(sphere);
        }
    }
    
    private List<PointOfInterest> GeneratePointsOfInterest(int pointCount)
    {
        List<PointOfInterest> points = new List<PointOfInterest>();

        var rng = new RandomNumberGenerator();
        rng.Randomize();

        int numGenerated = 0;

        while (numGenerated < pointCount)
        {
            float radius = SizeMultiplier * Mathf.Sqrt(rng.Randf());
            float angle = Mathf.Tau * rng.Randf();

            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            
            Vector2 position = new Vector2(x, y);

            var randomType = Types[rng.RandiRange(0, Types.Count - 1)];
            PointOfInterest point = new PointOfInterest(position, randomType);

            bool isValid = true;

            for (int i = 0; i < numGenerated; i++)
            {
                var p = points[i];
                bool isOverlapping = point.IsOverlappingWith(p);

                bool isInsideDeadzone = false;

                if (p.Type.Id == point.Type.Id)
                    isInsideDeadzone = point.IsInsideDeadzoneOf(p);

                if (isOverlapping || isInsideDeadzone)
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                points.Add(point);
                numGenerated++;
            }
        }

        return points;
    }
}
