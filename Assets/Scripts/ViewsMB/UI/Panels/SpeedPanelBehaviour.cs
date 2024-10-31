using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class SpeedPanelBehaviour : PanelBase, ISecondPanelWithMainPanel
{
    public string MainPanelId { get; set; }

    [SerializeField] private Text _carSpeedText;
    private EcsPool<SpeedPanelComp> _speedPanelPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _speedPanelPool = state.EcsWorld.GetPool<SpeedPanelComp>();
        ref var speedPanelComp = ref _speedPanelPool.Add(state.InterfaceEntity);
        speedPanelComp.SpeedPanelMb = this;
    }

    public void ShowSpeed(float speed) =>
        _carSpeedText.text = speed.ToString();

    public override void CleanupPanel() =>
        _speedPanelPool.Del(_state.InterfaceEntity);
}