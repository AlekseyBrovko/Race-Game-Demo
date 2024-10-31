using UnityEditor;
using UnityEngine;

public class SpawnerMb : MonoBehaviour
{
    public int Entity => _entity;
    public string[] ObjectsPoolNames => _npcNames;

    [field: Header("Editor")]
    [field: SerializeField] public string ZoneName { get; set; }
    [SerializeField] private bool _drawGizmo;
    [SerializeField] private Color _gizmoColor;
    [SerializeField] private SpawnPointMb _spawnPointPrefab;

    [Header("Gameplay")]
    [SerializeField] private string[] _npcNames;

    private int _entity;
    
    #region Editor
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_drawGizmo)
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(transform.position, 2);
        }
    }

    public GameObject AddSpawnPoint(Vector3 position, Quaternion rotation)
    {
        GameObject spawnPoint = (GameObject)PrefabUtility.InstantiatePrefab(_spawnPointPrefab.gameObject);
        spawnPoint.transform.SetParent(this.transform);
        spawnPoint.transform.position = position;
        spawnPoint.transform.rotation = rotation;
        return spawnPoint;
    }
#endif
    #endregion

    public void Init(int entity) =>
        _entity = entity;
}