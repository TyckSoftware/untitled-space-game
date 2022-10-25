using Godot;

/// <summary>
/// 
/// </summary>
/// <param name="Apoapsis"></param>
/// <param name="Periapsis"></param>
/// <param name="Longitude"></param>
/// <param name="Argument"></param>
/// <param name="Inclination"></param>
public record PlanetParameters(int Apoapsis, int Periapsis, int Longitude, int Argument, int Inclination);

public partial class Planet : StaticBody3D
{
    public float Radius { get; set; }
    public float Mass => Density * Radius * Radius * Radius;
    public float Density { get; set; } = 1;
	public float MeanAngularMotion { get; private set; }
	public float SemiMajorAxis { get; private set; }
	public float SemiMinorAxis { get; private set; }
	public float Eccentricity { get; private set; }
	public float PeriapsisTime { get; set; } = 10.0f;
    public PlanetParameters Parameters { get; set; }
    public Star Star { get; private set; }

	public Planet()
	{
        Radius = 10;
    }

    public void Init(PlanetParameters parameters, Star star)
	{
        Parameters = parameters;
        Star = star;
    }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Orbital parameters
		Eccentricity = (Parameters.Apoapsis - Parameters.Periapsis) / (Parameters.Apoapsis + Parameters.Periapsis);
        SemiMajorAxis = 0.5f * (Parameters.Periapsis + Parameters.Apoapsis);
        SemiMinorAxis = SemiMajorAxis * Mathf.Sqrt(1 - Mathf.Pow(Eccentricity, 2));
        MeanAngularMotion = Mathf.Sqrt(Star.Mass/Mathf.Pow(SemiMajorAxis, 3));
	}
	
	public override void _PhysicsProcess(double delta)
	{
        Position = GetOrbitalPosition((float)LevelManager.Time);
	}

	/// <summary>
    /// Method <c>OrbitalPosition</c> computes the position of the planet at time <c>time</c> according to Kepler's laws.
	/// See https://en.wikipedia.org/wiki/Kepler%27s_equation <seeal>
    /// </summary>
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

	private float GetEccentricAnomaly(float time, int maxIterations = 20, float rTol = 1E-7f)
	{	
		// The mean anomaly
		float meanAnomaly = (time - PeriapsisTime) * MeanAngularMotion;

        // Initial guess for E
        float E = Eccentricity > 0.8 ? Mathf.Pi : meanAnomaly;

		for (int i = 0; i < maxIterations; i++)
		{
			float numerator = Eccentricity * Mathf.Sin(E) + meanAnomaly - E;
			float denominator = Eccentricity * Mathf.Cos(E) - 1;
			
			var newE = E - numerator / denominator;

			float tolerance = Mathf.Abs((newE - E)/newE);
			
			if (tolerance <= rTol) 
			{
				return newE;
			}

			E = newE;
		}

		return E;
	}
}
