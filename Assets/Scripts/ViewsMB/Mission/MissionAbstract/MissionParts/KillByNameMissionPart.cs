using UnityEngine;

[CreateAssetMenu(fileName = "KillByNameMission", menuName = "Mission/KillByNameMission", order = 2)]
public class KillByNameMissionPart : MissionPartBase
{
    public override Enums.MissionType MissionType => Enums.MissionType.Kill;

    [field: Header("Kill Settings Region")]
    [field: SerializeField] public string[] EnemyIds { get; private set; }
    [field: SerializeField] public int EnemyAmount { get; private set; }
}