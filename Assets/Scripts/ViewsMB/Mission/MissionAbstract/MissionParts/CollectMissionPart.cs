using System;
using UnityEngine;

public class CollectMissionPart : MissionPartBase
{
    [field: Header("Collect Mission Region")]
    [field: SerializeField] public MissionCollectableMb CollectPrefab { get; private set; }
    [field: SerializeField] public MissionPoint[] CollectPoints { get; private set; }
}

[Serializable]
public class MissionPoint
{
    [field: SerializeField] public string InterestPointId { get; private set; }
    [field: SerializeField] public Vector3 PointPosition { get; private set; }
}