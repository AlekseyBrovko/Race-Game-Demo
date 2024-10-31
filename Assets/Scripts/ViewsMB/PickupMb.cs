using UnityEngine;

public class PickupMb : MonoBehaviour
{
    [field: SerializeField] public Enums.PickupType PickupType;
    public bool HasPickup { get; set; }
    public int SpawnerEntity { get; private set; }

    public void SetSpawnerEntity(int spawnerEntity) =>
        SpawnerEntity = spawnerEntity;

    public void DestroyPickup() =>
        Destroy(gameObject);
}