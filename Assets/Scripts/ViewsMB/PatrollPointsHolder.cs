using UnityEditor;
using UnityEngine;

public class PatrollPointsHolder : MonoBehaviour
{
    [SerializeField] private PatrollPointMb _patrollPointPrefab;

#if UNITY_EDITOR
    public GameObject SpawnPatrollPoint(Vector3 position, Quaternion rotation)
    {
        GameObject pointGo = (GameObject)PrefabUtility.InstantiatePrefab(_patrollPointPrefab.gameObject);
        pointGo.transform.position = position;
        pointGo.transform.rotation = rotation;
        pointGo.transform.SetParent(this.transform);
        return pointGo;
    }
#endif
}