using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnPointMb))]
public class SpawnPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpawnPointMb spawnPoint = (SpawnPointMb)target;
        GameObject go = spawnPoint.gameObject;

        if (GUILayout.Button("Clone"))
        {
            SpawnerMb spawnerMb = go.GetComponentInParent<SpawnerMb>();
            GameObject newSpawnPoint = spawnerMb.AddSpawnPoint(go.transform.position, go.transform.rotation);
            Selection.SetActiveObjectWithContext(newSpawnPoint, null);
        }
    }
}
