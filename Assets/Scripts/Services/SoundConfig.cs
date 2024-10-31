using UnityEngine;

[CreateAssetMenu(fileName = "SoundsConfig", menuName = "Configs/SoundsConfig", order = 2)]
public class SoundConfig : ScriptableObject
{
    [Header("Prefab")]
    public AudioBehaviourMB AudioBehaviourPrefab;

    [Header("Sounds")]
    public AudioClip LoseSound;
    public AudioClip WinSound;
    public AudioClip ClickSound;
    public AudioClip BuySound;
    public AudioClip FailClickSound;

    [Header("Motor Sounds")]
    public AudioClip SimpleMotorSound;

    [Header("Tire Sounds")]
    public AudioClip AsphaltSlipSound;
    public AudioClip[] AsphaltSlipSounds;

    [Header("Zombie Sounds")]
    public AudioClip[] ZombieHurtSounds;

    public AudioClip[] HitImpactSounds;
    public AudioClip[] MetalImpactSounds;
    public AudioClip[] GroundImpactSounds;
    public AudioClip[] FlashImpactSounds;


    public AudioClip[] ZombieDeathSound;

    public AudioClip GetRandomAsphaltSkidSound() =>
        AsphaltSlipSounds[Random.Range(0, AsphaltSlipSounds.Length)];

    public AudioClip GetRandomImpactSound() =>
        HitImpactSounds[Random.Range(0, HitImpactSounds.Length)];

    public AudioClip GetRandomGroundImpactSound() =>
        GroundImpactSounds[Random.Range(0, GroundImpactSounds.Length)];

    public AudioClip GetRandomMetalImpactSound() =>
        MetalImpactSounds[Random.Range(0, MetalImpactSounds.Length)];

    public AudioClip GetRandomFlashSound() =>
        FlashImpactSounds[Random.Range(0, FlashImpactSounds.Length)];
}