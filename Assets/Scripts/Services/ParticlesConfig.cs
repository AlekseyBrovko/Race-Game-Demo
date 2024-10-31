using UnityEngine;

[CreateAssetMenu(fileName = "ParticlesConfig", menuName = "Configs/ParticlesConfig", order = 2)]
public class ParticlesConfig : ScriptableObject
{
    [Header("Explosions")]
    public GameObject ExplosionParticle;
}