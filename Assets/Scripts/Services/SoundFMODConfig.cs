using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundFMODConfig", menuName = "Configs/SoundFMODConfig", order = 2)]
public class SoundFMODConfig : ScriptableObject
{
    [field: Header("Busses")]
    [field: SerializeField] public string MasterBusTag { get; private set; } = "bus:/";
    [field: SerializeField] public string MusicBusTag { get; private set; } = "bus:/Music";
    [field: SerializeField] public string SoundsBusTag { get; private set; } = "bus:/Sounds";
    [field: SerializeField] public string InPlayBusTag { get; private set; } = "bus:/Sounds/InPlay";

    [field: Header("Sounds")]
    [field: Header("UI")]
    [field: SerializeField] public EventReference ClickSound { get; private set; }
    [field: SerializeField] public EventReference BuySound { get; private set; }
    [field: SerializeField] public EventReference MoneyIncreaseSound { get; private set; }
    [field: SerializeField] public EventReference WarningSound { get; private set; }
    [field: SerializeField] public EventReference MissionCompleteSound { get; private set; }
    
    [field: Header("Zombie")]
    [field: SerializeField] public EventReference ZombieDeadSound { get; private set; }
    [field: SerializeField] public EventReference ZombieAttackSound { get; private set; }
    [field: SerializeField] public EventReference ZombieHitCarSound { get; private set; }

    [field: Header("Car Motors")]
    [field: SerializeField] public EventReference OldCarMotorSound { get; private set; }
    [field: SerializeField] public EventReference MuscleCarMotorSound { get; private set; }
    [field: SerializeField] public EventReference TruckMotorSound { get; private set; }

    [field: Header("Car Effects")]
    [field: SerializeField] public EventReference WheelSkidSound { get; private set; }
    [field: SerializeField] public EventReference CarHitZombieSound { get; private set; }
    [field: SerializeField] public EventReference CarHitWoodSound { get; private set; }
    [field: SerializeField] public EventReference CarHitMetalSound { get; private set; }
    [field: SerializeField] public EventReference CarHitDefaultSound { get; private set; }

    [field: Header("Other Effects")]
    [field: SerializeField] public EventReference ExplosionSound { get; private set; }
    [field: SerializeField] public EventReference PickupSound { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference MenuMusic { get; private set; }
}