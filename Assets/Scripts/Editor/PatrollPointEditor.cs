using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PatrollPointMb))]
public class PatrollPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Clone"))
        {
            PatrollPointMb patrollPointMb = (PatrollPointMb)target;
            Transform pointTransform = patrollPointMb.gameObject.transform;
            PatrollPointsHolder holderMb = patrollPointMb.gameObject.GetComponentInParent<PatrollPointsHolder>();
            var newPoint = holderMb.SpawnPatrollPoint(pointTransform.position, pointTransform.rotation);
            Selection.SetActiveObjectWithContext(newPoint, null);
        }
    }
}

[CustomEditor(typeof(PatrollPointsHolder))]
public class PatrollPointsHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add Patroll Point"))
        {
            PatrollPointsHolder holderMb = (PatrollPointsHolder)target;
            Transform holderTransform = holderMb.gameObject.transform;
            GameObject newPatrollPoint = holderMb.SpawnPatrollPoint(holderTransform.position, holderTransform.rotation);
            Selection.SetActiveObjectWithContext(newPatrollPoint, null);
        }
    }
}