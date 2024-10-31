using UnityEngine;
using UnityEditor;

public class PositionHelperWindow : EditorWindow
{
    private PositionHelperMb _helperMb;
    private Vector3 _position;

    [MenuItem("Tools/Position Helper")]
    public static void Open()
    {
        PositionHelperWindow window = GetWindow<PositionHelperWindow>();
        window.Show();
    }

    private void OnEnable()
    {
        EditorApplication.update += GetPositionOfHelperGo;
    }

    private void OnDisable()
    {
        DeleteHelperGo();
        EditorApplication.update -= GetPositionOfHelperGo;
    }

    private void GetPositionOfHelperGo()
    {
        if (_helperMb == null)
            return;

        _position = _helperMb.transform.position;
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Get Position"))
            OnGetPositionButtonClick();

        if (_helperMb != null)
            EditorGUILayout.Vector3Field("Position", _position);
    }

    private void OnGetPositionButtonClick()
    {
        _helperMb = Object.FindObjectOfType<PositionHelperMb>();
        if (_helperMb == null)
            SpawnHelperGo();

        _position = GetPositionOfCameraLook();
        SetPositionHelperGo(_position);
        Selection.SetActiveObjectWithContext(_helperMb, null);
    }

    private void SpawnHelperGo()
    {
        GameObject helperPrefab = Resources.Load("PositionHelper/PositionHelper", typeof(GameObject)) as GameObject;
        _helperMb = Instantiate(helperPrefab).GetComponent<PositionHelperMb>();
    }

    private void SetPositionHelperGo(Vector3 position) =>
        _helperMb.transform.position = position;

    private void DeleteHelperGo()
    {
        if (_helperMb != null)
            DestroyImmediate(_helperMb.gameObject);
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