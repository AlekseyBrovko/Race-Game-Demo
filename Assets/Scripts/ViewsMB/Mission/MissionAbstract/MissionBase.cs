using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "Mission/MissionBase", order = 1)]
public class MissionBase : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public BrifMaterials Brif { get; private set; }
    [field: SerializeField] public MissionPartBase[] MissionParts { get; private set; }
    [field: SerializeField] public int MoneyReward { get; private set; }
    [field: SerializeField] public string SpawnPointId { get; private set; }

#if UNITY_EDITOR
    private void Reset() =>
        Id = Guid.NewGuid().GetHashCode();
#endif
}