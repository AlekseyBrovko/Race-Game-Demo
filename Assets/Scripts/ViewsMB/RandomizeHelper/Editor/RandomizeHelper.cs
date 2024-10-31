using UnityEngine;
using UnityEditor;

public class RandomizeHelper : EditorWindow
{
    private GameObject[] _selectedGo;

    [MenuItem("Tools/Randomize Helper")]
    public static void Open()
    {
        RandomizeHelper window = GetWindow<RandomizeHelper>();
        window.Show();
    }

    private void OnGUI()
    {
        _selectedGo = Selection.gameObjects;
        if (_selectedGo != null && _selectedGo.Length > 0)
        {
            if (GUILayout.Button("Randomize Y rotation"))
                RandomizeYRotation(_selectedGo);
        }
    }

    private void RandomizeYRotation(GameObject[] selectedGo)
    {   
        foreach (var go in selectedGo)
        {
            Undo.RecordObject(go.transform, "Randomize Y");
            Vector3 rot = go.transform.eulerAngles;
            Vector3 newRot = new Vector3(rot.x, Random.Range(0f, 360f), rot.z);
            go.transform.rotation = Quaternion.Euler(newRot);
            Undo.RecordObject(go.transform, "Randomize Y");
        }
    }

    private void OnEnable() =>
        Selection.selectionChanged += OnSelectionChange;

    private void OnSelectionChange()
    {
        _selectedGo = Selection.gameObjects;
        Repaint();
    }

    private void OnDisable() =>
        Selection.selectionChanged -= OnSelectionChange;
}