using Godot;

/// <summary>
/// Class containing methods and properties pertaining to 
/// spawning planets around a central star.
/// </summary>
public partial class Star : StaticBody3D
{
    /// <summary>
    /// Minimum number of planets orbiting around a star.
    /// </summary>
    public const int MIN_PLANETS = 1;
    
    /// <summary>
    /// Maximum number of planets orbiting around a star.
    /// </summary>
    public const int MAX_PLANETS = 10;

    /// <summary>
    /// Minimum distance of planet to the star.
    /// </summary>
    public const int MIN_RADIUS = 200;

    /// <summary>
    /// Maximum distance of planet to the star.
    /// </summary>
    public const int MAX_RADIUS = 500;

    /// <summary>
    /// Stellar radius.
    /// </summary>
    public float Radius { get; set; }

    /// <summary>
    /// Stellar mass.
    /// </summary>
    public float Mass => Density * Radius * Radius * Radius;

    /// <summary>
    /// Stellar density.
    /// </summary>
    public float Density { get; set; } = 1;

    /// <summary>
    /// Number of planets orbitting around the star.
    /// </summary>
    public int PlanetCount { get; private set; }

    /// <summary>
    /// Orbital parameters for each of the orbiting planets.
    /// </summary>
    public OrbitParameters[] OrbitParams { get; set; } = default!;

    /// <summary>
    /// Random number generator instance.
    /// </summary>
    private RandomNumberGenerator Rand { get; set; } = new RandomNumberGenerator();

    /// <summary>
    /// Instance of the Planet scene.
    /// </summary>
    private PackedScene PlanetScene { get; set; } = default!;

    public Star()
	{
        // set-up random seed.
        Rand.Randomize();
        
        Radius = Rand.RandiRange(40, 100);
    }

    /// <inheritdoc />
    public override void _Ready()
	{
        GetNode<CSGSphere3D>("Sphere").Radius = Radius;

        PlanetScene = ResourceLoader.Load("res://entities/planet/planet.tscn") as PackedScene;
        
        PlanetCount = Rand.RandiRange(MIN_PLANETS, MAX_PLANETS);
        OrbitParams = new OrbitParameters[PlanetCount];

        GeneratePlanets();
        SpawnPlanets();
    }
	
    /// <summary>
    /// Generate a set of orbital parameters for each of the planets
    /// orbitting around the star.
    /// </summary>
    private void GeneratePlanets()
	{
        int minRadius = MIN_RADIUS;
        int maxRadius = MAX_RADIUS;

        int numGenerated = 0;

        while (numGenerated < PlanetCount)
		{
            var planetParameters = GeneratePlanarParameters(minRadius, maxRadius);

            OrbitParams[numGenerated] = planetParameters;
            
            numGenerated++;

            // Set a new min/max distance from the star to avoid planets colliding.
            minRadius = maxRadius;
            maxRadius = (int)(1.5 * minRadius);
        }
	}

    /// <summary>
    /// Randomly generate orbital parameters for a planet orbitting in a plane.
    /// </summary>
    /// <param name="minRadius">
    /// Minimum possible distance from the star.
    /// </param>
    /// <param name="maxRadius">
    /// Maximum possible distance from the star.
    /// </param>
    /// <returns>
    /// Orbital parameters for a partical planet.
    /// </returns>
	private OrbitParameters GeneratePlanarParameters(int minRadius, int maxRadius)
	{
        int periapsis = Rand.RandiRange((int)minRadius/2, (int)maxRadius/2);
        int apoapsis = Rand.RandiRange(periapsis, maxRadius);
        int longitude = Rand.RandiRange(0, 359);

        var parameters = new OrbitParameters(apoapsis, periapsis, longitude, 0, 0);

        return parameters;
    }

    /// <summary>
    /// Add all the planets to the level.
    /// </summary>
	private void SpawnPlanets()
	{
		foreach (var parameters in OrbitParams)
		{
            var planetInstance = PlanetScene.Instantiate() as Planet;

            GD.Print(parameters);

            planetInstance.Init(parameters, this);
            AddChild(planetInstance);
        }
	}
}
