using Godot;
using System.Collections.Generic;

public partial class LevelGenerator : Node
{
    public List<PointOfInterestType> Types { get; set; } = new List<PointOfInterestType>();
    public int[] TypeFrequency { get; set; } = default!;

    [Export]
    public float SizeMultiplier { get; set; } = 100.0f;

    /// <summary>
    /// Random number generator instance.
    /// </summary>
    private RandomNumberGenerator Rand { get; set; } = new RandomNumberGenerator();

    public LevelGenerator()
	{
        // set-up random seed.
        Rand.Randomize();
    }

    /// <inheritdoc />
    public override void _Ready()
    {
        // Setting up points of interest
        int pointCount = 64;

        Types.Add(new PointOfInterestType("Star", 10f, 40.0f, 1.0f, 10));
        Types.Add(new PointOfInterestType("Loot", 1.0f, 20f, 1.5f, 30));
        Types.Add(new PointOfInterestType("SpaceStation", 2.0f, 4.0f, 1.4f, 50));

        // Types.Sort((x, y) => x.ModifiedWeight.CompareTo(y.ModifiedWeight)); // Only when generating w/ GenerateWeightedPointsOfInterest

        foreach (var t in Types)
        {
            GD.Print(t.ModifiedWeight);
        }

        TypeFrequency = new int[] { 8, 40, 16 };

        // Generate points of interest
        // List<PointOfInterest> poi = GeneratePointsOfInterest(pointCount);
        // List<PointOfInterest> poi = GenerateWeightedPointsOfInterest(pointCount);
        List<PointOfInterest> poi = GenerateSequentialPointsOfInterest();

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
        
        int numGenerated = 0;

        while (numGenerated < pointCount)
        {
            // Randomly determine position and POI type.
            Vector2 position = GeneratePointOnCircleWithRadius(SizeMultiplier);
            var randomType = Types[Rand.RandiRange(0, Types.Count - 1)];

            PointOfInterest point = new PointOfInterest(position, randomType);
                
            // Compare position with other POIs to determine its validity.
            bool isValid = IsValidPointOfInterest(point, points);

            if (isValid)
            {
                points.Add(point);
                numGenerated++;
            }
        }

        return points;
    }

    private List<PointOfInterest> GenerateWeightedPointsOfInterest(int pointCount)
    {
        List<PointOfInterest> points = new List<PointOfInterest>();

        float totalWeight = 0.0f;

        foreach (PointOfInterestType t in Types)
        {
            totalWeight += t.ModifiedWeight;
        }

        int numGenerated = 0;

        while (numGenerated < pointCount)
        {
            // Randomly determine the POI type.

            float randomWeight = Rand.RandfRange(0, totalWeight);
            float cumulativeWeight = 0.0f;

            PointOfInterestType type = default!;

            foreach (PointOfInterestType t in Types) 
            {
                cumulativeWeight += t.ModifiedWeight;
                
                if (randomWeight <= cumulativeWeight)
                    type = t;
            }

            // Randomly POI position.
            float maxRadius = SizeMultiplier * type.ClumpingFactor;
            Vector2 position = GeneratePointOnCircleWithRadius(maxRadius);

            PointOfInterest point = new PointOfInterest(position, type);

            // Compare position with other POIs to determine its validity.
            bool isValid = IsValidPointOfInterest(point, points);

            if (isValid)
            {
                points.Add(point);
                numGenerated++;
            }
        }

        return points;
    }

    private List<PointOfInterest> GenerateSequentialPointsOfInterest()
    {
        List<PointOfInterest> points = new List<PointOfInterest>();

        int numGenerated = 0;
        
        int numTypesGenerated = 0;
        int typeCount = TypeFrequency.Length;

        while (numTypesGenerated < typeCount)
        {
            int numType = 0;
            int maxType = TypeFrequency[numTypesGenerated];

            while (numType < maxType)
            {
                // Randomly determine position and POI type.
                float maxRadius = SizeMultiplier;

                if (numTypesGenerated > 0)
                    maxRadius *= 1.4f;

                Vector2 position = GeneratePointOnCircleWithRadius(maxRadius);
                PointOfInterestType type = Types[numTypesGenerated];

                PointOfInterest point = new PointOfInterest(position, type);

                // Compare position with other POIs to determine its validity.
                bool isValid = IsValidPointOfInterest(point, points);

                if (isValid)
                {
                    points.Add(point);
                    numGenerated++;
                    numType++;
                }
            }

            numTypesGenerated++;
        }

        return points;
    }

    private Vector2 GeneratePointOnCircleWithRadius(float maxRadius)
    {
        float radius = maxRadius * Mathf.Sqrt(Rand.Randf());
        float angle = Mathf.Tau * Rand.Randf();

        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        
        Vector2 position = new Vector2(x, y);

        return position;
    }

    private Vector2 GeneratePointOnUnitSquare()
    {
        float x = Rand.Randf();
        float y = Rand.Randf();
        
        Vector2 position = new Vector2(x, y);

        return position;
    }

    private bool IsValidPointOfInterest(PointOfInterest point, List<PointOfInterest> points)
    {
        bool isValid = true;

        foreach(PointOfInterest p in points)
        {
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

        return isValid;
    }
}
