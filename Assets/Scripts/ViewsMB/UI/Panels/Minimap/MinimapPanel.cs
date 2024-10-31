using Client;
using Leopotam.EcsLite;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MinimapPanel : PanelBase, ISecondPanelWithMainPanel
{
    public string MainPanelId { get; set; }

    [Header("References")]
    [SerializeField] private RawImage _squareImagePrefab;
    [SerializeField] private MinimapItem _itemPrefab;
    [SerializeField] private MinimapItem _zoneItemPrefab;
    [SerializeField] private MinimapItemArrow _arrowPrefab;
    [SerializeField] private Image _mapImage;
    [SerializeField] private Image _maskImage;
    [SerializeField] private RectTransform _minimapRect;

    [Header("Settings")]
    [SerializeField] private float _squareSize = 100f;
    [SerializeField] private Color _checkpointColor;
    [SerializeField] private Color _lastPointColor;
    [SerializeField] private Color _killZoneColor;
    [SerializeField] private Color _collectablesOnMissionColor;

    private GridLayoutGroup _mapGrid;
    private float _indexOfScale;
    private Vector2 _offset;
    private float _sqrHalfIconWidth;
    private float _halfIconWidth;

    private List<MinimapItem> _itemsOnScreen = new List<MinimapItem>();
    private Dictionary<MinimapItem, MinimapItemArrow> _arrowsDict = new Dictionary<MinimapItem, MinimapItemArrow>();

    private EcsWorld _world;
    private EcsPool<MinimapPanelComp> _minimapPool;
    private MinimapDataSo _minimapData;

    private Vector2 _playerPosOnMinimap;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _world = state.EcsWorld;
        _minimapPool = _world.GetPool<MinimapPanelComp>();

        BuildMinimap();

        ref var minimapComp = ref _minimapPool.Add(state.InterfaceEntity);
        minimapComp.MinimapPanelMb = this;

        _halfIconWidth = _maskImage.rectTransform.sizeDelta.x / 2f;
        _sqrHalfIconWidth = _halfIconWidth * _halfIconWidth;

        if (!_state.IsMobilePlatform)
            SetMinimapPositionForPc();
    }

    private void BuildMinimap()
    {
        _minimapData = _state.MinimapData;
        _mapGrid = _mapImage.GetComponent<GridLayoutGroup>();
        _mapGrid.constraintCount = _minimapData.HorizontalSquareCount;
        _mapGrid.cellSize = new Vector2(_squareSize, _squareSize);

        for (int i = 0; i < _minimapData.PartsOfMinimap.Length; i++)
        {
            RawImage tempSquare = Instantiate(_squareImagePrefab, _mapImage.transform);
            tempSquare.texture = _minimapData.PartsOfMinimap[i];
        }

        float minimapWidth = _squareSize * _minimapData.HorizontalSquareCount;
        float worldMapWidth = _minimapData.HorizontalSquareCount * _minimapData.SquareSide;

        _indexOfScale = minimapWidth / worldMapWidth;
        _offset = new Vector2(-_indexOfScale * _minimapData.HorizontalOffset, -_indexOfScale * _minimapData.VerticalOffset);
    }

    public void SetRotationForMinimap(float value) =>
        _maskImage.rectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, value));

    public void SetPositionForMinimap(Vector3 playerPosition)
    {
        Vector2 positionForMinimap = new Vector2(-playerPosition.x, -playerPosition.z);
        _mapImage.rectTransform.localPosition = positionForMinimap * _indexOfScale - _offset;

        Vector2 playerPos = new Vector2(playerPosition.x, playerPosition.z);
        _playerPosOnMinimap = playerPos * _indexOfScale + _offset;

        HandleMinimapItems();
    }

    public override void CleanupPanel() =>
        _minimapPool.Del(_state.InterfaceEntity);

    public void ShowCheckpoint(Vector3 itemPosition, int id) =>
        InstantiateItem(itemPosition, id, _checkpointColor);

    public void ShowLastPoint(Vector3 itemPosition, int id) =>
        InstantiateItem(itemPosition, id, _lastPointColor);

    public void ShowCollectable(Vector3 itemPosition, int id) =>
        InstantiateItem(itemPosition, id, _collectablesOnMissionColor);

    public void ShowZone(Vector3 zoneCenterPosition, int id)
    {
        Color fadeZoneColor = _killZoneColor;
        fadeZoneColor.a = 0.2f;

        //TODO нужно контролить рект сайз
        //TODO хранить данные по отступу в стрелке
        var item = InstantiateItemOnMinimap(_zoneItemPrefab, zoneCenterPosition, id, fadeZoneColor);
        var arrow = InstantiateArrowOnMinimap(id, _killZoneColor);
        _arrowsDict.Add(item, arrow);
    }

    private void InstantiateItem(Vector3 itemPosition, int id, Color color)
    {
        var item = InstantiateItemOnMinimap(_itemPrefab, itemPosition, id, color);
        var arrow = InstantiateArrowOnMinimap(id, color);
        _arrowsDict.Add(item, arrow);
    }

    private MinimapItem InstantiateItemOnMinimap(MinimapItem itemPrefab, Vector3 itemPosition, int id, Color color)
    {
        MinimapItem icon = Instantiate(itemPrefab, _mapImage.transform);
        icon.Init(id, color);
        icon.transform.SetAsLastSibling();
        Vector2 position = new Vector2(itemPosition.x, itemPosition.z) * _indexOfScale + _offset;
        icon.RectTransform.localPosition = position;
        _itemsOnScreen.Add(icon);
        return icon;
    }

    private MinimapItemArrow InstantiateArrowOnMinimap(int id, Color color)
    {
        MinimapItemArrow arrow = Instantiate(_arrowPrefab, _mapImage.transform);
        arrow.Init(id, color);
        arrow.transform.SetAsLastSibling();
        arrow.gameObject.SetActive(false);
        return arrow;
    }

    public void RemovePointFromMinimap(int id)
    {
        Debug.Log("RemovePointFromMinimap id = " + id);

        MinimapItem item = _itemsOnScreen.FirstOrDefault(x => x.Id == id);

        if (item == null)
        {
            Debug.LogWarning("RemovePointFromMinimap item == null");
            return;
        }

        //TODO ошибка тут вылезла
        MinimapItemArrow arrow = _arrowsDict[item];

        _arrowsDict.Remove(item);
        _itemsOnScreen.Remove(item);

        Destroy(item.gameObject);
        Destroy(arrow.gameObject);
    }

    private void HandleMinimapItems()
    {
        foreach (MinimapItem item in _itemsOnScreen)
        {
            MinimapItemArrow arrow = _arrowsDict[item];
            Vector2 direction = item.RectTransform.anchoredPosition - _playerPosOnMinimap;
            float sqrDistanse = Vector2.SqrMagnitude(direction);

            if (sqrDistanse > _sqrHalfIconWidth)
            {
                arrow.RectTransform.anchoredPosition = _playerPosOnMinimap;
                arrow.gameObject.SetActive(true);

                Vector3 heading = item.transform.position - arrow.transform.position;
                float angle = Vector3.SignedAngle(arrow.transform.up, heading, arrow.transform.forward);
                arrow.transform.Rotate(arrow.transform.forward, angle);
                arrow.RectTransform.anchoredPosition = _playerPosOnMinimap + direction.normalized * _halfIconWidth * 0.9f;
            }
            else
            {
                arrow.gameObject.SetActive(false);
            }
        }
    }

    //private void SetMinimapPositionForAndroid()
    //{
    //    float screenHeight = Screen.height;
    //    Debug.Log("SetMinimapPositionForAndroid screenHeight = " + screenHeight);
    //    _minimapRect.anchoredPosition = new Vector2(_minimapRect.anchoredPosition.x, screenHeight / 2f);
    //}

    private void SetMinimapPositionForPc()
    {
        float screenHeight = Screen.height;
        float iconPcOffset = Mathf.Abs(_minimapRect.anchoredPosition.x);

        float y = -screenHeight + iconPcOffset;
        _minimapRect.anchoredPosition = new Vector2(_minimapRect.anchoredPosition.x, y);
    }
}