using UnityEngine;

public class RideByCheckpointsMission : MissionPartBase
{
    [field: Header("Checkpoints Region")]
    [field: SerializeField] public MissionPoint[] InterestPointsIds { get; private set; }
}