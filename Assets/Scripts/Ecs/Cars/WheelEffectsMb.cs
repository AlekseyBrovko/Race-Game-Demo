using UnityEngine;

public class WheelEffectsMb : MonoBehaviour
{
    public TrailRenderer SkidmarksTrail => _skidmarksTrail;
    public ParticleSystem[] SmokeParticles => _smokeParticles;

    [SerializeField] private TrailRenderer _skidmarksTrail;
    [SerializeField] private ParticleSystem[] _smokeParticles;
}
