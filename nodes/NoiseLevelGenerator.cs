using Godot;
using System.Collections.Generic;

public record POI(int x, int y);

public partial class NoiseLevelGenerator : Node
{
    [Export]
    public FastNoiseLite Noise { get; set; } = default!;

    private float MaxNoiseValue { get; set; } = 0.0f;

    private float POIPercentage { get; set; } = 0.8f;
    private float NoiseThreshold => POIPercentage * MaxNoiseValue;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        var rng = new RandomNumberGenerator();
        rng.Randomize();

        Noise = new FastNoiseLite();
        Noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
        Noise.Frequency = 0.2f;
        Noise.Seed = (int)rng.Randi();

        float[,] data = GetNoiseData(64, 64);

        List<POI> poi = GetPointsOfInterest(data);

        List<POI> m_poi = new List<POI>();
        m_poi.Add(poi[0]);

        for (int i = 1; i < poi.Count; i++)
        {
            bool isValid = true;
            foreach (POI p in m_poi)
            {
                float distance = Mathf.Sqrt(Mathf.Pow(p.x - poi[i].x, 2) + Mathf.Pow(p.y - poi[i].y, 2));

                if (distance < 8)
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
                m_poi.Add(poi[i]);
        }

        GD.Print($"Number of valid POIs = {m_poi.Count}");

        Node3D container = GetNode<Node3D>("../Boxes");

        foreach (POI p in poi)
        {
            CSGBox3D box = new CSGBox3D();
            box.Position = new Vector3(p.x, 0, p.y);

            if (m_poi.Contains(p))
            {
                StandardMaterial3D mat = new StandardMaterial3D();
                mat.AlbedoColor = new Color(0.0f, 1.0f, 0.0f);
                
                box.MaterialOverride = mat;
            }

            container.AddChild(box);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// private void Generate
    private float[,] GetNoiseData(int width, int height)
    {
        float[,] data = new float[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float value = Noise.GetNoise2d(i, j);
                data[i, j] = value;

                if (value > MaxNoiseValue)
                    MaxNoiseValue = value;
            }
        }

        GD.Print($"Maximum value = {MaxNoiseValue}");

        return data;
    }
    
    private List<POI> GetPointsOfInterest(float[,] data)
    {
        List<POI> points = new List<POI>();

        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                float value = data[i, j];

                if (value > NoiseThreshold)
                {
                    points.Add(new POI(i, j));
                }
            }
        }

        GD.Print($"POIs generated: {points.Count}");

        return points;
    }
}
