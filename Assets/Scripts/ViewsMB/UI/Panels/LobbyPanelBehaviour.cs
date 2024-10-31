using Client;
using Leopotam.EcsLite;
using LevelController;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

public class LobbyPanelBehaviour : PanelBase, IMainPanel, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private CarStatsPanel _carStatsPanel;

    [Space]
    [SerializeField] private Button _nextCarButton;
    [SerializeField] private Button _prevCarButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _playStoryButton;
    [SerializeField] private Button _playFreeRideButton;

    [Space]
    [SerializeField] private GameObject _carPriceIcon;
    [SerializeField] private Text _carPriceText;

    [SerializeField] private LocalizedStringTable _uiStringTable;
    [SerializeField] private LocalizedStringTable _carsStringTable;
    private StringTable _currentStringTable;

    private PlayerCarSo _currentCar;
    private SavedCar _savedCar;

    private EcsWorld _world;
    private EcsPool<LobbyPanelComp> _lobbyPanelPool;
    private EcsPool<NextShopItemEvent> _nextPool;
    private EcsPool<PreviousShopItemEvent> _prevPool;
    private EcsPool<CarRotationLobbyInputComp> _inputPool;
    private EcsPool<CarBuyEvent> _buyPool;
    private EcsPool<CarChooseEvent> _choosePool;
    private EcsPool<LoadSceneEvent> _loadScenePool;
    private EcsPool<VisualMoneyEvent> _visualMoneyPool;
    private EcsPool<StartGameButtonEvent> _startGamePool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _world = state.EcsWorld;
        _nextPool = _world.GetPool<NextShopItemEvent>();
        _prevPool = _world.GetPool<PreviousShopItemEvent>();
        _inputPool = _world.GetPool<CarRotationLobbyInputComp>();
        _buyPool = _world.GetPool<CarBuyEvent>();
        _choosePool = _world.GetPool<CarChooseEvent>();
        _lobbyPanelPool = _world.GetPool<LobbyPanelComp>();
        _loadScenePool = _world.GetPool<LoadSceneEvent>();
        _visualMoneyPool = _world.GetPool<VisualMoneyEvent>();
        _startGamePool = _world.GetPool<StartGameButtonEvent>();

        ref var lobbyPanelComp = ref _lobbyPanelPool.Add(state.InterfaceEntity);
        lobbyPanelComp.LobbyPanelMb = this;

        _canvasMb.ShowPanelById(PanelsIdHolder.MoneyGaragePanelId);
        _visualMoneyPool.Add(_world.NewEntity());

        _carStatsPanel.Init(state, _carsStringTable);

        InitButtons();
    }

    private void InitButtons()
    {
        _nextCarButton.onClick.AddListener(() =>
        {
            _canvasMb.PlayClickSound();
            OnNextButton();
        });

        _prevCarButton.onClick.AddListener(() =>
        {
            _canvasMb.PlayClickSound();
            OnPrevButton();
        });

        _playStoryButton.onClick.AddListener(() =>
        {
            _canvasMb.PlayClickSound();
            StartGame(Enums.GameModType.Story);
        });

        _playFreeRideButton.onClick.AddListener(() =>
        {
            _canvasMb.PlayClickSound();
            StartGame(Enums.GameModType.FreeRide);
        });

        _buyButton.onClick.AddListener(() =>
        {
            _canvasMb.PlayClickSound();
            OnBuyButtonClick();
        });

        _menuButton.onClick.AddListener(() =>
        {
            _canvasMb.PlayClickSound();
            _canvasMb.ShowPanelById(PanelsIdHolder.GarageMenuPanelId);
        });
    }

    public override void OnBackButtonClick()
    {
        Debug.Log("OnBackButtonClick()");
        _canvasMb.ShowPanelById(PanelsIdHolder.GarageMenuPanelId);
    }

    public override void CleanupPanel() =>
        _lobbyPanelPool.Del(_state.InterfaceEntity);

    private bool _carIsBuyed = false;
    public void ShowCarData(PlayerCarSo carSo, SavedCar savedCar = null)
    {
        _currentCar = carSo;
        _savedCar = savedCar;

        _carStatsPanel.ShowStatsIcons(carSo, savedCar);

        _carIsBuyed = savedCar != null;

        if (savedCar == null)
            ShowNotBuyedState();
        else
            ShowBuyedState();
    }

    private IEnumerator GetLocalization()
    {
        var tableLoading = _uiStringTable.GetTable();
        yield return tableLoading;
        _currentStringTable = tableLoading;
        var str = _currentStringTable["buy"].Value;
        Debug.Log(str);
    }

    private void TestGetLocal()
    {
        StringTable tableLoading = _uiStringTable.GetTable();
        _currentStringTable = tableLoading;
        var str = _currentStringTable["buy"].LocalizedValue;
        Debug.Log(str);
    }

    private void OnBuyButtonClick()
    {
        ref var buyComp = ref _buyPool.Add(_state.EcsWorld.NewEntity());
        buyComp.CarId = _currentCar.Id;
    }

    private void StartGame(Enums.GameModType gameModType)
    {
        ref var startGameComp = ref _startGamePool.Add(_world.NewEntity());
        startGameComp.GameModType = gameModType;
    }

    public void ShowBuyedState()
    {
        _buyButton.gameObject.SetActive(false);
        _playStoryButton.gameObject.SetActive(true);
        _playFreeRideButton.gameObject.SetActive(true);
    }

    public void ShowNotBuyedState()
    {
        _buyButton.gameObject.SetActive(true);
        _playStoryButton.gameObject.SetActive(false);
        _playFreeRideButton.gameObject.SetActive(false);
    }

    public void OnStartMoveAnimation()
    {
        _nextCarButton.gameObject.SetActive(false);
        _prevCarButton.gameObject.SetActive(false);
        _menuButton.gameObject.SetActive(false);
        _playStoryButton.gameObject.SetActive(false);
        _playFreeRideButton.gameObject.SetActive(false);
        _buyButton.gameObject.SetActive(false);

        _carStatsPanel.gameObject.SetActive(false);
    }

    public void OnStopMoveAnimation()
    {
        _nextCarButton.gameObject.SetActive(true);
        _prevCarButton.gameObject.SetActive(true);
        _menuButton.gameObject.SetActive(true);
        _playStoryButton.gameObject.SetActive(true);
        _playFreeRideButton.gameObject.SetActive(true);

        _carStatsPanel.gameObject.SetActive(true);
    }

    private void OnNextButton() =>
        _nextPool.Add(_state.EcsWorld.NewEntity());

    private void OnPrevButton() =>
        _prevPool.Add(_state.EcsWorld.NewEntity());

    public void OnBeginDrag(PointerEventData eventData) =>
        _inputPool.Add(_state.InputEntity);

    public void OnEndDrag(PointerEventData eventData) =>
        _inputPool.Del(_state.InputEntity);

    public void OnDrag(PointerEventData eventData) { }
}