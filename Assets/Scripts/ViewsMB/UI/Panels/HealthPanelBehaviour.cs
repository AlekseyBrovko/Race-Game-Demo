using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanelBehaviour : PanelBase, ISecondPanelWithMainPanel
{
    public string MainPanelId { get; set; }

    [SerializeField] private Image _healthBar;
    private EcsPool<HealthPanelComp> _healthPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _healthPool = _state.EcsWorld.GetPool<HealthPanelComp>();
        ref var healthComp = ref _healthPool.Add(_state.InterfaceEntity);
        healthComp.HealthPanelMb = this;
    }

    public void ShowHp(float sliderValue) =>
        _healthBar.fillAmount = sliderValue;

    public override void CleanupPanel() =>
        _healthPool.Del(_state.InterfaceEntity);
}