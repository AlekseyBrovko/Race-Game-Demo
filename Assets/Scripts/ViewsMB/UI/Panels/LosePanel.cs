using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

public class LosePanel : PanelBase, IPopupPanel
{
    [SerializeField] private Button _continueForVideoButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Text _messageText;

    [SerializeField] private LocalizedStringTable _localizedStringTable;
    private StringTable _currentStringTable;
    private LocalizeStringEvent _messageStringEvent;

    private LocalizedString _outOfFuelLocalString;
    private LocalizedString _outOfHealthLocalString;
    private const string _uiTableName = "UI";

    private const string _deathMessage = "Здоровье закончилось";
    private const string _outOfFuelMessage = "Бензин закончился";

    private const string _outOfFuelTag = "outOfFuel";
    private const string _outOfHealthTag = "outOfHealth";

    private EcsWorld _world;
    private EcsPool<PlayInGamePromoVideoEvent> _playVideoEvent;
    private EcsPool<LoadSceneEvent> _loadScenePool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _world = _state.EcsWorld;
        _playVideoEvent = _world.GetPool<PlayInGamePromoVideoEvent>();
        _loadScenePool = _world.GetPool<LoadSceneEvent>();

        //TODO надо тестить
        //_currentStringTable = _localizedStringTable.GetTable();
        _messageStringEvent = _messageText.gameObject.GetComponent<LocalizeStringEvent>();
        _outOfFuelLocalString = new LocalizedString(_uiTableName, _outOfFuelTag);
        _outOfHealthLocalString = new LocalizedString(_uiTableName, _outOfHealthTag);

        DataForOpenLosePanel loseData = openData as DataForOpenLosePanel;
        switch (loseData.LoseType)
        {
            case Enums.LoseType.Death: ShowDeathState(); break;
            case Enums.LoseType.Fuel: ShowOutOfFuelState(); break;
        }

        _continueForVideoButton.onClick.AddListener(() => OnShowVideoButtonClick());
        _cancelButton.onClick.AddListener(() => OnCancelButtonClick());
    }

    public void OnShowVideoButtonClick()
    {
        ref var playVideoComp = ref _playVideoEvent.Add(_world.NewEntity());
        playVideoComp.Reason = Enums.PlayPromoVideoReason.RestoreHpAndFuel;
    }
        
    public void OnCancelButtonClick()
    {
        ref var sceneComp = ref _loadScenePool.Add(_world.NewEntity());
        sceneComp.SceneId = ScenesIdHolder.LobbySceneId;
    }

    public void ShowDeathState()
    {
        //_messageText.text = _deathMessage;
        _messageStringEvent.StringReference = _outOfHealthLocalString;
    }
    
    public void ShowOutOfFuelState()
    {   
        //_messageText.text = _outOfFuelMessage;
        _messageStringEvent.StringReference = _outOfFuelLocalString;
    }   
}