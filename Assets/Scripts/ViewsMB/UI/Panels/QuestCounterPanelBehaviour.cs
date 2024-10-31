using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class QuestCounterPanelBehaviour : PanelBase, ISecondPanelWithMainPanel
{
    public string MainPanelId { get; set; }

    [SerializeField] private Text _counterText;

    private string _counterName;
    private string _amountValue;

    private EcsPool<QuestCounterPanelComp> _counterPanelPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        DataForOpenMissionCounterPanel data = openData as DataForOpenMissionCounterPanel;
        _counterName = data.CounterName;
        _amountValue = data.Amount.ToString();

        _counterPanelPool = state.EcsWorld.GetPool<QuestCounterPanelComp>();
        ref var counterPanelComp = ref _counterPanelPool.Add(_state.InterfaceEntity);
        counterPanelComp.MissionCounterPanel = this;

        ShowCounter(0);
    }

    public void ShowCounter(int counter) =>
        _counterText.text = _counterName + " " + counter.ToString() + "/" + _amountValue;

    public override void CleanupPanel() =>
        _counterPanelPool.Del(_state.InterfaceEntity);
}