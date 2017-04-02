using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(ParticleSystem))]
public class PhysicalParticleSystem : MonoBehaviour {

    [Range(0f, 1f)]
    public float fieldFactor = 1f;

    ParticleSystem ps;
    ParticleSystem.MainModule main;
    ParticleSystem.Particle[] particles;
    int aliveCount;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        Assert.IsNotNull(ps);
        main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
    }
    
	void Start ()
    {
        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }
	
	void FixedUpdate () {
        aliveCount = ps.GetParticles(particles);
        if (fieldFactor == 1f)
            RotationField.Instance.AffectParticles(ref particles, aliveCount);
        else
            RotationField.Instance.AffectParticles(ref particles, aliveCount, fieldFactor);
        ps.SetParticles(particles, aliveCount);
    }
}
