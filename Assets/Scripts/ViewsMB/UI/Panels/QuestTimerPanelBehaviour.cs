using Client;
using Leopotam.EcsLite;
using System;
using UnityEngine;
using UnityEngine.UI;

public class QuestTimerPanelBehaviour : PanelBase, ISecondPanelWithMainPanel
{
    public string MainPanelId { get; set; }

    [SerializeField] private Text _timerText;

    private EcsPool<QuestTimerPanelComp> _missionPanelPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _missionPanelPool = state.EcsWorld.GetPool<QuestTimerPanelComp>();
        ref var panelComp = ref _missionPanelPool.Add(_state.InterfaceEntity);
        panelComp.MissionTimerPanel = this;
    }

    public void ShowTimer(float timerValue) =>
        _timerText.text = TimeSpan.FromSeconds(timerValue).ToString("mm\\:ss");

    public override void CleanupPanel() =>
        _missionPanelPool.Del(_state.InterfaceEntity);
}