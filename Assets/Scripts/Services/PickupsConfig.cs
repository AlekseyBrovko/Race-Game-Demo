using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PickupsConfig", menuName = "Configs/PickupsConfig", order = 2)]
public class PickupsConfig : ScriptableObject
{
    [SerializeField] public PickupInStore[] Pickups;
}

[Serializable]
public class PickupInStore
{
    public int Weight;
    public GameObject PickupPrefab;
    public Enums.PickupType PickupType;
}