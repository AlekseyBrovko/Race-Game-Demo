using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class FuelPanelBehaviour : PanelBase, ISecondPanelWithMainPanel
{
    [SerializeField] private Image _fuelValueImage;

    public string MainPanelId { get; set; }
    private EcsPool<FuelPanelComp> _fuelPanelPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _fuelPanelPool = _state.EcsWorld.GetPool<FuelPanelComp>();
        ref var fuelPanelComp = ref _fuelPanelPool.Add(_state.InterfaceEntity);
        fuelPanelComp.FuelPanelMb = this;
    }

    public void ShowFuel(float index) =>
        _fuelValueImage.fillAmount = index;

    public override void CleanupPanel() =>
        _fuelPanelPool.Del(_state.InterfaceEntity);
}