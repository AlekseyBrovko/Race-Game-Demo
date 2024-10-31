using System;
using UnityEngine;

public abstract class MissionPartBase : ScriptableObject
{
    public virtual Enums.MissionType MissionType { get; private set; }

    [field: Header("Base Region")]
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string LocalizationDescriptionTag { get; private set; }
    [field: SerializeField] public bool SaveMissionProgress { get; private set; }

    [field: Header("Mission On Time Region")]
    [field: SerializeField] public bool MissionOnTime { get; private set; }
    [field: SerializeField] public float TimeOnMission { get; private set; }

#if UNITY_EDITOR
    private void Reset() =>
            Id = Guid.NewGuid().GetHashCode();
    private void OnValidate()
    {
        if (Id == 0)
            Id = Guid.NewGuid().GetHashCode();
    }
#endif
}