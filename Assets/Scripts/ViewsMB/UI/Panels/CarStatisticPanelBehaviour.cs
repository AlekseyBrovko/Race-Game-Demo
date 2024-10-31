using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class CarStatisticPanelBehaviour : PanelBase, ISecondPanelWithMainPanel
{
    public string MainPanelId { get; set; }

    [SerializeField] private Text _speedText;
    [SerializeField] private Text _tahoText;

    private EcsPool<CarStatisticPanelComp> _panelPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);
        _panelPool = _state.EcsWorld.GetPool<CarStatisticPanelComp>();
        ref var panelComp = ref _panelPool.Add(_state.InterfaceEntity);
        panelComp.CarStatisticPanelMb = this;
    }

    public override void CleanupPanel() =>
        _panelPool.Del(_state.InterfaceEntity);

    public void ShowSpeed(float value) =>
        _speedText.text = value.ToString();
 
    public void ShowTaho(float value) =>
        _tahoText.text = value.ToString();
}