using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

public class MapEditor : EditorWindow
{
    private float _squareSide = 150f;

    private int _xSquersCount = 8;
    private int _zSquersCount = 8;

    private float _xOffset = -100;
    private float _zOffset = -150;

    private Color _backgroundColor;
    private float _cameraHeight = 100;

    private List<Rect> _minimapRects = new List<Rect>();

    private Texture2D[] _screens;
    private Texture2D[] _savedScreens;

    [MenuItem("Tools/Mini Map Creator")]
    public static void Open()
    {
        MapEditor window = GetWindow<MapEditor>();

        window.Show();
    }

    private void OnEnable()
    {
        EditorApplication.update += SetBorders;
    }

    private void OnDisable()
    {
        EditorApplication.update -= SetBorders;
    }

    private void SetBorders()
    {
        DrawDebug();
    }

    private void DrawDebug()
    {
        if (_minimapRects.Count == 0)
            return;

        foreach (Rect rect in _minimapRects)
            DrawSquer(rect);
    }

    private void OnGUI()
    {
        _squareSide = FloatField("Square Side", _squareSide);
        _xSquersCount = IntField("X Squers Count", _xSquersCount);
        _zSquersCount = IntField("Z Squers Count", _zSquersCount);
        _xOffset = FloatField("X Center Offset", _xOffset);
        _zOffset = FloatField("Z Center Offset", _zOffset);

        _cameraHeight = FloatField("Camera Height", _cameraHeight);
        _backgroundColor = ColorField("Background Color", _backgroundColor);

        if (GUILayout.Button("Calculate Squers"))
            CalculateSquers();

        if (_minimapRects.Count > 0)
        {
            if (GUILayout.Button("Make Screanshoots"))
                MakeScreanshoots();

            if (GUILayout.Button("Reset Bounds"))
                ResetSquers();
        }

        if (_screens != null && _screens.Length > 0)
        {
            if (GUILayout.Button("Save Minimap"))
                SaveMinimapData();
        }
    }

    private void ResetSquers() =>
        _minimapRects.Clear();

    private void SaveMinimapData()
    {
        _savedScreens = new Texture2D[_screens.Length];

        for (int i = 0; i < _screens.Length; i++)
            _savedScreens[i] = MapSaveLoadUtils.GetSaveTexture(i);

        MinimapDataSo minimapDataSo = CreateInstance<MinimapDataSo>();
        minimapDataSo.SquareSide = _squareSide;
        minimapDataSo.HorizontalSquareCount = _xSquersCount;
        minimapDataSo.VerticalSquareCount = _zSquersCount;
        minimapDataSo.HorizontalOffset = _xOffset;
        minimapDataSo.VerticalOffset = _zOffset;
        minimapDataSo.PartsOfMinimap = _savedScreens;

        AssetDatabase.CreateAsset(minimapDataSo, "Assets/Configs/Minimap/testMinimap.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(minimapDataSo);
    }

    private void MakeScreanshoots()
    {
        if (_minimapRects.Count == 0)
        {
            Debug.LogWarning("Сначала нужно просчитать квадраты");
            return;
        }

        _screens = new Texture2D[_minimapRects.Count];
        //создать камеру

        for (int i = 0; i < _minimapRects.Count; i++)
        {
            _screens[i] = CameraTextureUtils.CreateScreen(_backgroundColor, _cameraHeight, _minimapRects[i]);
            MapSaveLoadUtils.SaveTexture(_screens[i], i);
        }

        //удалить камеру
    }

    private void CalculateSquers()
    {
        if ((_xSquersCount % 2) != 0 || (_zSquersCount % 2) != 0)
        {
            Debug.LogWarning("Количество ячеек должно быть чётное");
            return;
        }

        Vector2 centerPos = Vector2.zero + new Vector2(_xOffset, _zOffset);

        if ((_xSquersCount % 2) != 0 || (_zSquersCount % 2) != 0)
        {
            Debug.LogWarning("Количество ячеек должно быть чётное");
            return;
        }

        Vector2 leftBottomBoundPoint = centerPos - Vector2.up * _squareSide * _zSquersCount / 2 - Vector2.right * _squareSide * _xSquersCount / 2;

        Vector2 tempSquerPoint = leftBottomBoundPoint;

        Vector2 rectSize = new Vector2(_squareSide, _squareSide);
        _minimapRects.Clear();

        for (int j = 0; j < _xSquersCount; j++)
        {
            for (int i = 0; i < _xSquersCount; i++)
            {
                Rect rect = new Rect(tempSquerPoint, rectSize);
                _minimapRects.Add(rect);
                tempSquerPoint = tempSquerPoint + Vector2.right * _squareSide;
            }
            tempSquerPoint = tempSquerPoint - Vector2.right * _squareSide * _xSquersCount + Vector2.up * _squareSide;
        }
    }

    private void DrawMapPreview()
    {
        //const float previewSize = 400;
        //const float previewYPadding = 50;
        //const float previewXPadding = 5;

        //Rect mapRect = BeginHorizontal(GL.Width(previewXPadding + previewSize), GL.Height(previewYPadding + previewSize));
        //{
        //    float scale = previewSize / _subdivideCount;

        //    for (int x = 0; x < _subdivideCount; x++)
        //    {
        //        for (int z = 0; z < _subdivideCount; z++)
        //        {
        //            int index = x * _subdivideCount + z;

        //            Rect worldRect = _borders;
        //            Rect previewRect = new Rect(mapRect.x, mapRect.y, previewSize, previewSize);

        //            Vector2 localPosition =
        //                MapUtils.GetRemappedRectPosition(worldRect, previewRect, _rects[index].position);

        //            localPosition.y = mapRect.y + previewRect.yMax - scale - localPosition.y;

        //            Rect rect = new Rect(localPosition.x, localPosition.y, scale, scale);

        //            GUI.Box(rect, _screens[index]);
        //        }
        //    }
        //}
        //EndHorizontal();
    }

    private void DrawSquer(Rect rect)
    {
        float y = 0;
        float duration = 0.05f;

        Vector3 point0 = new Vector3(rect.xMin, y, rect.yMin);
        Vector3 point1 = new Vector3(rect.xMin, y, rect.yMax);
        Vector3 point2 = new Vector3(rect.xMax, y, rect.yMax);
        Vector3 point3 = new Vector3(rect.xMax, y, rect.yMin);

        Debug.DrawLine(point0, point1, Color.red, duration, false);
        Debug.DrawLine(point1, point2, Color.red, duration, false);
        Debug.DrawLine(point2, point3, Color.red, duration, false);
        Debug.DrawLine(point3, point0, Color.red, duration, false);
    }
}