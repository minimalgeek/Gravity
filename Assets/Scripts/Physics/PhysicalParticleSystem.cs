using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(ParticleSystem))]
public class PhysicalParticleSystem : MonoBehaviour {

    [Range(0f, 1f)]
    public float fieldFactor = 1f;

    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    int aliveCount;
    UniformRotationField field;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        Assert.IsNotNull(ps);
        GameObject fieldObject = GameObject.FindGameObjectWithTag("MainForceField");
        Assert.IsNotNull(fieldObject);
        field = fieldObject.GetComponent<UniformRotationField>();
        Assert.IsNotNull(field);
    }
    
	void Start ()
    {
        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }
	
	void FixedUpdate () {
        aliveCount = ps.GetParticles(particles);
        if (fieldFactor == 1f)
            field.AffectParticles(ref particles, aliveCount);
        else
            field.AffectParticles(ref particles, aliveCount, fieldFactor);
        ps.SetParticles(particles, aliveCount);
    }
}
