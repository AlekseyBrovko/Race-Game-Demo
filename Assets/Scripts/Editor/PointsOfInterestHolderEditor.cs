using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PointsOfInterestHolder))]
public class PointsOfInterestHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add PointOfInterest"))
        {
            PointsOfInterestHolder holder = (PointsOfInterestHolder)target;
            Transform holderTransform = holder.gameObject.transform;
            PointOfInterestMb point = GameObject.Instantiate(holder.PointPrefab, holderTransform);
            point.transform.position = GetPositionOfCameraLook();

            Selection.SetActiveObjectWithContext(point, null);
        }
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
}