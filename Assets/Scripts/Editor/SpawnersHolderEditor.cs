using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnersHolder))]
public class SpawnersHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add Random Spawner"))
        {
            SpawnersHolder holder = (SpawnersHolder)target;
            Transform holderTransform = holder.gameObject.transform;
            GameObject spawner = holder.GetSpawner(GetPositionOfCameraLook(), holderTransform.rotation);
            Selection.SetActiveObjectWithContext(spawner, null);
        }

        if (GUILayout.Button("Add Melee Spawner"))
        {
            SpawnersHolder holder = (SpawnersHolder)target;
            Transform holderTransform = holder.gameObject.transform;
            GameObject spawner = holder.GetMeleeSpawner(GetPositionOfCameraLook(), holderTransform.rotation);
            Selection.SetActiveObjectWithContext(spawner, null);
        }

        if (GUILayout.Button("Add Melee Farm Spawner"))
        {
            SpawnersHolder holder = (SpawnersHolder)target;
            Transform holderTransform = holder.gameObject.transform;
            GameObject spawner = holder.GetMeleeFarmSpawner(GetPositionOfCameraLook(), holderTransform.rotation);
            Selection.SetActiveObjectWithContext(spawner, null);
        }

        if (GUILayout.Button("Add Range Spawner"))
        {
            SpawnersHolder holder = (SpawnersHolder)target;
            Transform holderTransform = holder.gameObject.transform;
            GameObject spawner = holder.GetRangeSpawner(GetPositionOfCameraLook(), holderTransform.rotation);
            Selection.SetActiveObjectWithContext(spawner, null);
        }

        if (GUILayout.Button("Add Explosion Spawner"))
        {
            SpawnersHolder holder = (SpawnersHolder)target;
            Transform holderTransform = holder.gameObject.transform;
            GameObject spawner = holder.GetExplosionSpawner(GetPositionOfCameraLook(), holderTransform.rotation);
            Selection.SetActiveObjectWithContext(spawner, null);
        }

        if (GUILayout.Button("Init Spawners"))
            InitSpawners((SpawnersHolder)target);
    }

    private Vector3 GetPositionOfCameraLook()
    {
        Vector3 result = new Vector3();
        Camera editorCamera = SceneView.lastActiveSceneView.camera;
        Vector3 cameraPos = editorCamera.transform.position;
        Vector3 cameraForward = editorCamera.transform.forward;

        Ray ray = new Ray(cameraPos, cameraForward);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
            result = hit.point;

        return result;
    }

    private void InitSpawners(SpawnersHolder holder)
    {
        CityZoneMb[] zones = holder.GetComponentsInChildren<CityZoneMb>();
        Debug.Log("zones.Length = " + zones.Length);
        holder.CityZones = zones;
        foreach (var zone in zones)
        {
            SpawnerMb[] spawnersOnZone = zone.GetComponentsInChildren<SpawnerMb>();
            foreach (SpawnerMb spawner in spawnersOnZone)
            {
                spawner.ZoneName = zone.ZoneName;
                EditorUtility.SetDirty(zone);
                EditorUtility.SetDirty(spawner);
            }   
        }

        SpawnerMb[] spawners = holder.GetComponentsInChildren<SpawnerMb>();
        Debug.Log("spawners.Length = " + spawners.Length);
        holder.Spawners = spawners;

        EditorUtility.SetDirty(holder);
    }
}