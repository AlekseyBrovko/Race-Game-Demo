using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnerMb))]
public class SpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpawnerMb spawner = (SpawnerMb)target;
        Transform spawnerTransform = spawner.gameObject.transform;

        if (GUILayout.Button("Add SpawnPoint"))
        {
            GameObject spawnPoint = spawner.AddSpawnPoint(spawnerTransform.position, spawnerTransform.rotation);
            Selection.SetActiveObjectWithContext(spawnPoint, null);
        }

        if (GUILayout.Button("Clone Spawner"))
        {
            SpawnersHolder holder = spawner.GetComponentInParent<SpawnersHolder>();
            GameObject newSpawner = holder.GetSpawner(spawnerTransform.position, spawnerTransform.rotation);
            Selection.SetActiveObjectWithContext(newSpawner, null);
        }
    }
}