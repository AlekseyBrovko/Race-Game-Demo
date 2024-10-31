using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class MissionCompletePanel : PanelBase, IPopupPanel
{
    [SerializeField] private Button _showVideoAndGetDoubleMoneyButton;
    [SerializeField] private Button _goToGarageButton;
    [SerializeField] private Text _rewardValueText;

    private EcsWorld _world;
    private EcsPool<PlayInGamePromoVideoEvent> _playVideoEvent;
    private EcsPool<LoadSceneEvent> _loadScenePool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _world = _state.EcsWorld;
        _playVideoEvent = _world.GetPool<PlayInGamePromoVideoEvent>();
        _loadScenePool = _world.GetPool<LoadSceneEvent>();

        _showVideoAndGetDoubleMoneyButton.onClick.AddListener(() => OnShowVideoButtonClick());
        _goToGarageButton.onClick.AddListener(() => OnGoToGarageButtonClick());

        _rewardValueText.text = _state.CurrentMission.MoneyReward.ToString();
    }

    public void OnShowVideoButtonClick()
    {
        ref var playVideoComp = ref _playVideoEvent.Add(_world.NewEntity());
        playVideoComp.Reason = Enums.PlayPromoVideoReason.DoubleMissionMoney;
    }

    public void OnGoToGarageButtonClick()
    {
        ref var sceneComp = ref _loadScenePool.Add(_world.NewEntity());
        sceneComp.SceneId = ScenesIdHolder.LobbySceneId;
    }
}