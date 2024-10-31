using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelBehaviour : PanelBase, IMainPanel
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _controlButton;
    [SerializeField] private Button _exitButton;

    [Header("Game Mode")]
    [SerializeField] private Button _goToGarageButton;

    [Header("Tutorial Mode")]
    [SerializeField] private Button _skipTutorialButton;

    private EcsWorld _world;
    private EcsPool<ContinueButtonEvent> _continuePool;
    private EcsPool<LoadSceneEvent> _loadScenePool;
    private EcsPool<SkipTutorialEvent> _skipTutorialPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _world = _state.EcsWorld;
        _continuePool = _world.GetPool<ContinueButtonEvent>();
        _loadScenePool = _world.GetPool<LoadSceneEvent>();
        _skipTutorialPool = _world.GetPool<SkipTutorialEvent>();

        InitButtons();
    }

    private void InitButtons()
    {
        _continueButton.onClick.AddListener(() => OnContinueButtonClick());
        _settingsButton.onClick.AddListener(() => OnSettingButtonClick());
        _controlButton.onClick.AddListener(() => OnControlButtonClick());
        _goToGarageButton.onClick.AddListener(() => OnGarageButtonClick());
        _exitButton.onClick.AddListener(() => OnExitButtonClick());
        _skipTutorialButton.onClick.AddListener(() => OnSkipTutorialButtonClick());

        if (_state.IsWebGl)
            _exitButton.gameObject.SetActive(false);

        if (_state.StartGameTutorial)
        {
            _goToGarageButton.gameObject.SetActive(false);
            _skipTutorialButton.gameObject.SetActive(true);
        }
        else
        {
            _goToGarageButton.gameObject.SetActive(true);
            _skipTutorialButton.gameObject.SetActive(false);
        }
    }

    private void OnSkipTutorialButtonClick()
    {
        _canvasMb.PlayClickSound();
        _skipTutorialPool.Add(_world.NewEntity());
    }

    private void OnContinueButtonClick()
    {
        _canvasMb.PlayClickSound();
        _continuePool.Add(_world.NewEntity());
    }

    private void OnSettingButtonClick()
    {
        _canvasMb.PlayClickSound();
        _canvasMb.ShowPanelById(PanelsIdHolder.SettingsPanelId);
    }

    private void OnControlButtonClick()
    {
        _canvasMb.PlayClickSound();

        TutorialPanelData data = new TutorialPanelData(false);

        //TODO сюда прокинуть данные, что не нужно выходить в пауз системс
        if (_state.IsMobilePlatform)
            _canvasMb.ShowPanelById(PanelsIdHolder.TutorialInGameAndroidPanelId, data);
        else
            _canvasMb.ShowPanelById(PanelsIdHolder.TutorialInGameDesktopPanelId, data);
    }

    private void OnGarageButtonClick()
    {
        _canvasMb.PlayClickSound();

        //TODO здесь дать попап хотите ли уйти с уровн€
        Debug.Log("ѕќ ј«ј“№ ѕќѕ јѕ");
        ref var loadSceneComp = ref _loadScenePool.Add(_world.NewEntity());
        loadSceneComp.SceneId = ScenesIdHolder.LobbySceneId;
    }

    private void OnExitButtonClick()
    {
        _canvasMb.PlayClickSound();
        _canvasMb.ShowPanelById(PanelsIdHolder.ExitPopupPanelId);
    }

    public override void OnBackButtonClick()
    {
        _canvasMb.PlayClickSound();
        _continuePool.Add(_world.NewEntity());
    }
}