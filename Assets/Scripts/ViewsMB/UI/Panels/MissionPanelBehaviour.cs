using Client;
using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

public class MissionPanelBehaviour : PanelBase, ISecondPanel
{
    [SerializeField] private MissionPartIcon _missionPartIconPrefab;
    [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
    [SerializeField] private Transform _missionIconsHolder;
    [SerializeField] private GameObject _progressIcon;
    [SerializeField] private Button _showPanelButton;
    [SerializeField] private Button _hidePanelButton;

    [Space]
    [SerializeField] private LocalizedStringTable _missionPartsTable;
    private StringTable _currentMissionPartsTable;

    private ContentSizeFitter _contentSizeFitter;
    private List<MissionPartIcon> _missionIcons = new List<MissionPartIcon>();

    private EcsWorld _world;
    private EcsPool<MissionPanelComp> _missionPanelPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _contentSizeFitter = _verticalLayoutGroup.gameObject.GetComponent<ContentSizeFitter>();

        _currentMissionPartsTable = _missionPartsTable.GetTable();

        _world = state.EcsWorld;
        _missionPanelPool = _world.GetPool<MissionPanelComp>();
        ref var missionPanelComp = ref _missionPanelPool.Add(_state.InterfaceEntity);
        missionPanelComp.MissionPanelMb = this;

        _showPanelButton.onClick.AddListener(() => ShowProgressIcon());
        _hidePanelButton.onClick.AddListener(() => HideProgressIcon());
    }

    public void ShowProgressIcon()
    {
        _progressIcon.SetActive(true);
        _showPanelButton.gameObject.SetActive(false);
    }

    public void HideProgressIcon()
    {
        _progressIcon.SetActive(false);
        _showPanelButton.gameObject.SetActive(true);
    }

    public void BuildPanel(MissionBase mission)
    {
        for (int i = 0; i < mission.MissionParts.Length; i++)
        {
            MissionPartIcon icon = Instantiate(_missionPartIconPrefab, _missionIconsHolder);
            icon.Init(mission.MissionParts[i].Id);
            _missionIcons.Add(icon);
        }
        StartCoroutine(HandleLayoutGroupCor());
    }

    public void ShowMissionPartProgress(int missionPartId, string text)
    {
        MissionPartIcon icon = _missionIcons.FirstOrDefault(x => x.MissionPartId == missionPartId);
        icon.ShowText(text);
        StartCoroutine(HandleLayoutGroupCor());
    }

    public void ShowMissionPartComplete(int missionPartId)
    {
        MissionPartIcon icon = _missionIcons.FirstOrDefault(x => x.MissionPartId == missionPartId);
        icon.ShowPartCompleteState();
    }

    public void ShowMissionPartFailed(int missionPartId)
    {
        MissionPartIcon icon = _missionIcons.FirstOrDefault(x => x.MissionPartId == missionPartId);
        icon.ShowPartFailState();
    }

    public override void CleanupPanel() =>
        _missionPanelPool.Del(_state.InterfaceEntity);

    private IEnumerator HandleLayoutGroupCor()
    {
        _contentSizeFitter.enabled = false;
        _verticalLayoutGroup.enabled = false;
        yield return null;
        _verticalLayoutGroup.enabled = true;
        _contentSizeFitter.enabled = true;
    }
}
