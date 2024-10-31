using UnityEditor;
using UnityEngine;

public class SpawnersHolder : MonoBehaviour
{
    [SerializeField] private SpawnerMb _spawnerPrefab;

    [SerializeField] private SpawnerMb _meleeSpawnerPrefab;
    [SerializeField] private SpawnerMb _meleeFarmSpawnerPrefab;
    [SerializeField] private SpawnerMb _rangeSpawnerPrefab;
    [SerializeField] private SpawnerMb _explosionSpawnerPrefab;

    [Header("Spawners and Zones")]
    [SerializeField] public SpawnerMb[] Spawners;
    [SerializeField] public CityZoneMb[] CityZones;

#if UNITY_EDITOR
    public GameObject GetSpawner(Vector3 position, Quaternion rotation)
    {
        GameObject spawner = (GameObject)PrefabUtility.InstantiatePrefab(_spawnerPrefab.gameObject);
        spawner.transform.position = position;
        spawner.transform.rotation = rotation;
        spawner.transform.SetParent(this.transform);
        return spawner;
    }

    public GameObject GetMeleeSpawner(Vector3 position, Quaternion rotation)
    {
        GameObject spawner = (GameObject)PrefabUtility.InstantiatePrefab(_meleeSpawnerPrefab.gameObject);
        spawner.transform.position = position;
        spawner.transform.rotation = rotation;
        spawner.transform.SetParent(this.transform);
        return spawner;
    }

    public GameObject GetMeleeFarmSpawner(Vector3 position, Quaternion rotation)
    {
        GameObject spawner = (GameObject)PrefabUtility.InstantiatePrefab(_meleeFarmSpawnerPrefab.gameObject);
        spawner.transform.position = position;
        spawner.transform.rotation = rotation;
        spawner.transform.SetParent(this.transform);
        return spawner;
    }

    public GameObject GetRangeSpawner(Vector3 position, Quaternion rotation)
    {
        GameObject spawner = (GameObject)PrefabUtility.InstantiatePrefab(_rangeSpawnerPrefab.gameObject);
        spawner.transform.position = position;
        spawner.transform.rotation = rotation;
        spawner.transform.SetParent(this.transform);
        return spawner;
    }

    public GameObject GetExplosionSpawner(Vector3 position, Quaternion rotation)
    {
        GameObject spawner = (GameObject)PrefabUtility.InstantiatePrefab(_explosionSpawnerPrefab.gameObject);
        spawner.transform.position = position;
        spawner.transform.rotation = rotation;
        spawner.transform.SetParent(this.transform);
        return spawner;
    }
#endif
}