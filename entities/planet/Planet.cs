using Godot;

/// <summary>
/// Defines orbital parameters for a planet.
/// </summary>
/// <param name="Apoapsis">
/// Maximum distance from the star.
/// </param>
/// <param name="Periapsis">
/// Minimum distance to the star.
/// </param>
/// <param name="Longitude">
/// Horizontally orients the orbit.
/// </param>
/// <param name="Argument">
/// Orientation of the orbit in the orbital plane.
/// </param>
/// <param name="Inclination">
/// Vertical tilt of the ellipse.
/// </param>
public record OrbitParameters(int Apoapsis, int Periapsis, int Longitude, int Argument, int Inclination);

/// <summary>
/// Contains methods and properties pertaining to the
/// movement of planets around a star.
/// </summary>
public partial class Planet : StaticBody3D
{
	/// <summary>
    /// Planetary radius.
    /// </summary>
    public float Radius { get; set; }

	/// <summary>
    /// Planetary mass.
    /// </summary>
    public float Mass => Density * Radius * Radius * Radius;

	/// <summary>
    /// Planetary density.
    /// </summary>
    public float Density { get; set; } = 1;

	/// <summary>
    /// Average angular speed per orbit. 
    /// </summary>
	public float MeanAngularMotion { get; private set; }

	/// <summary>
    /// Half the length of the major axis.
    /// </summary>
	public float SemiMajorAxis { get; private set; }

	/// <summary>
    /// Half the length of the minor axis.
    /// </summary>
	public float SemiMinorAxis { get; private set; }

	/// <summary>
    /// Measure of how circular an orbit is.
    /// </summary>
	public float Eccentricity { get; private set; }

	/// <summary>
    /// Time at which the planet is at its periapsis point.
    /// </summary>
	public float PeriapsisTime { get; set; } = 0.0f;

    /// <summary>
    /// The planet's orbital parameters.
    /// </summary>
    public OrbitParameters Parameters { get; set; } = default!;

    /// <summary>
    /// The star around which the planet orbits.
    /// </summary>
    public Star Star { get; private set; } = default!;

    public Planet()
	{
		var rng = new RandomNumberGenerator();
        Radius = rng.RandiRange(10, 20);
    }

	/// <summary>
    /// Set the planets host star and orbital parameters.
    /// </summary>
    /// <param name="parameters">
    /// The planet's orbital parameters.
    /// </param>
    /// <param name="star">
    /// The host star.
    /// </param>
    public void Init(OrbitParameters parameters, Star star)
	{
        Parameters = parameters;
        Star = star;
    }
	
	/// <inheritdoc />
	public override void _Ready()
	{
		GetNode<CSGSphere3D>("Sphere").Radius = Radius;

		// Setting orbital parameters.
		Eccentricity = (Parameters.Apoapsis - Parameters.Periapsis) / (Parameters.Apoapsis + Parameters.Periapsis);
        SemiMajorAxis = 0.5f * (Parameters.Periapsis + Parameters.Apoapsis);
        SemiMinorAxis = SemiMajorAxis * Mathf.Sqrt(1 - Mathf.Pow(Eccentricity, 2));
        MeanAngularMotion = Mathf.Sqrt(Star.Mass/Mathf.Pow(SemiMajorAxis, 3));
	}
	
	/// <inheritdoc />
	public override void _PhysicsProcess(double delta)
	{
        Position = GetOrbitalPosition(LevelManager.Time);
	}

	/// <summary>
    /// Method <c>OrbitalPosition</c> computes the position of the planet at time <c>time</c> according to Kepler's laws.
	/// See https://en.wikipedia.org/wiki/Kepler%27s_equation.
    /// </summary>
    /// <param name="time">
    /// Time spent in the level.
    /// </param>
    /// <returns>
    /// Position in cartesian coordinates.
    /// </returns>
	private Vector3 GetOrbitalPosition(float time)
	{
		float anomaly = GetEccentricAnomaly(time);
		
        Vector3 position = new Vector3(
			SemiMajorAxis * (Mathf.Cos(anomaly) - Eccentricity), 
			0,
			SemiMinorAxis * Mathf.Sin(anomaly)
		);

		Vector3 rotatedPosition = position.Rotated(Vector3.Up, Mathf.DegToRad(Parameters.Argument));

		rotatedPosition = rotatedPosition.Rotated(Vector3.Right, Mathf.DegToRad(Parameters.Inclination));
		rotatedPosition = rotatedPosition.Rotated(Vector3.Up, Mathf.DegToRad(Parameters.Longitude));

		return rotatedPosition;
	}

	/// <summary>
    /// Get the angular position of the planet along the orbit
    /// as a function of time using the Newton–Raphson method.
    /// See https://en.wikipedia.org/wiki/Newton%27s_method.
    /// </summary>
    /// <param name="time">
    /// Ingame time.
    /// </param>
    /// <param name="maxIterations">
    /// Maximum number of iterations for the root finding algorithm.
    /// </param>
    /// <param name="relTolerance">
    /// Minimum relative difference between solutions for the eccentric anomaly.
    /// </param>
    /// <returns>
    /// The angular position along the orbit.
    /// </returns>
	private float GetEccentricAnomaly(float time, int maxIterations = 20, float relTolerance = 1E-7f)
	{	
		// The mean anomaly.
		float meanAnomaly = (time - PeriapsisTime) * MeanAngularMotion;

        // Initial guess for the eccentric anomaly per https://en.wikipedia.org/wiki/Kepler%27s_equation
        float anomaly = Eccentricity > 0.8f ? Mathf.Pi : meanAnomaly;

		// Iteratively find the root of Kepler's Equation
		for (int i = 0; i < maxIterations; i++)
		{
			// Define Kepler's Equation (numerator) and it's derivative (denominator).
			float numerator = Eccentricity * Mathf.Sin(anomaly) + meanAnomaly - anomaly;
			float denominator = Eccentricity * Mathf.Cos(anomaly) - 1.0f;
			
			var newAnomaly = anomaly - numerator / denominator;

			// If the relative difference (tolerance) is small enough we found a sufficient solution.
			float tolerance = Mathf.Abs((newAnomaly - anomaly)/newAnomaly);
			
			if (tolerance <= relTolerance) 
			{
                return newAnomaly;
			}

			anomaly = newAnomaly;
		}

		return anomaly;
	}
}
