using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPanelBehaviour : PanelBase, ISecondPanelWithMainPanel, IMoneyPanel
{
    [SerializeField] protected Text _moneyCounterText;

    private EcsPool<MoneyPanelComp> _moneyPanelPool;

    public string MainPanelId { get; set; }

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _moneyPanelPool = _state.EcsWorld.GetPool<MoneyPanelComp>();
        ref var moneyPanelComp = ref _moneyPanelPool.Add(_state.InterfaceEntity);
        moneyPanelComp.MoneyPanelMb = this;
    }

    public virtual void ShowMoney() =>
        _moneyCounterText.text = _state.PlayerMoneyScore.ToString();

    public override void CleanupPanel() =>
        _moneyPanelPool.Del(_state.InterfaceEntity);
}