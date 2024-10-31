using UnityEngine;
using Leopotam.EcsLite;
using Client;

public class CanvasBehaviour : MonoBehaviour
{
    public PanelsController PanelsController { get; private set; }

    private EcsWorld _world;
    private GameState _state;
    private EcsPool<UISoundEvent> _soundPool;

    public void Init(GameState state)
    {
        var world = state.EcsWorld;
        _world = world;
        _state = state;
        _soundPool = _world.GetPool<UISoundEvent>();
        PanelsController = new PanelsController(_state, this);
    }

    public void ShowStartSceneStartPanel() =>
        PanelsController.ShowPanelById(PanelsIdHolder.MainMenuPanelId);

    public void ShowLobbySceneStartPanel() =>
        PanelsController.ShowPanelById(PanelsIdHolder.LobbyStartPanelId);

    public void ShowPlaySceneStartPanel() =>
        PanelsController.ShowPanelById(PanelsIdHolder.InGamePanelId);

    public void OnBackButtonClick() =>
        PanelsController.MainPanel.OnBackButtonClick();

    public void ShowPanelById(string id, IOpenPanelData data = null) =>
        PanelsController.ShowPanelById(id, data);

    public void DestroyPanelById(string id) =>
        PanelsController.DestroyPanelById(id);

    public void PlayClickSound()
    {
        ref var soundComp = ref _soundPool.Add(_world.NewEntity());
        soundComp.Sound = Enums.SoundEnum.ClickSound;
    }

    public void PlayWarningSound()
    {
        ref var soundComp = ref _soundPool.Add(_world.NewEntity());
        soundComp.Sound = Enums.SoundEnum.WarningSound;
    }

    public void PlayBuySound()
    {
        ref var soundComp = ref _soundPool.Add(_world.NewEntity());
        soundComp.Sound = Enums.SoundEnum.BuySound;
    }

    public void PlayMoneyIncreaseSound()
    {
        ref var soundComp = ref _soundPool.Add(_world.NewEntity());
        soundComp.Sound = Enums.SoundEnum.MoneyIncreaseSound;
    }
}