using Godot;

public partial class Star : StaticBody3D
{
    public const int MIN_PLANETS = 2;
    public const int MAX_PLANETS = 10;
    public const int MIN_RADIUS = 100;
    public const int MAX_RADIUS = 500;

    public float Radius { get; set; }
    public float Mass => Density * Radius * Radius * Radius;
    public float Density { get; set; } = 1;

    public int PlanetCount { get; private set; }
    public PlanetParameters[] PlanetParams { get; set; }

    private RandomNumberGenerator _rng = new RandomNumberGenerator();
    private PackedScene _planetScene;

	public Star()
	{
        Radius = 40; // TODO: randomly generate radius...
    }

    public override void _Ready()
	{
        // radius = GetNode<CSGSphere3D>("Sphere").Radius;

        _planetScene = ResourceLoader.Load("res://entities/planet/planet.tscn") as PackedScene;
        
        PlanetCount = _rng.RandiRange(MIN_PLANETS, MAX_PLANETS);
        PlanetParams = new PlanetParameters[PlanetCount];

        GD.Print($"Initialising {PlanetCount} planets...");

        // Generate planets...
        GeneratePlanets();

        // Spawn planets...
        SpawnPlanets();
    }
	
	public override void _Process(double delta)
	{
	}

    private void GeneratePlanets()
	{
        int minRadius = MIN_RADIUS;
        int maxRadius = MAX_RADIUS;

        int numGenerated = 0;

        while (numGenerated < PlanetCount)
		{
            var planetParameters = GeneratePlanarParameters(minRadius, maxRadius);

            PlanetParams[numGenerated] = planetParameters;
            
            numGenerated++;

            minRadius = maxRadius;
            maxRadius = (int)(1.5 * minRadius);
        }
	}

	private PlanetParameters GeneratePlanarParameters(int min, int max)
	{
        _rng.Randomize();
        int periapsis = _rng.RandiRange((int)min/2, (int)max/2);
        
        _rng.Randomize();
        int apoapsis = _rng.RandiRange(periapsis, max);
        
        _rng.Randomize();
        int longitude = _rng.RandiRange(0, 359);

        var parameters = new PlanetParameters(apoapsis, periapsis, longitude, 0, 0);

        return parameters;
    }

	private void SpawnPlanets()
	{
		foreach (var parameters in PlanetParams)
		{
            GD.Print(parameters.ToString());

            var planetInstance = _planetScene.Instantiate() as Planet;

            planetInstance.Init(parameters, this);
            AddChild(planetInstance);
        }
	}
}
