using Client;
using Leopotam.EcsLite;
using LevelController;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public class MainMenuPanelBehaviour : PanelBase, IMainPanel
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _freeRideButton;
        [SerializeField] private Button _garageButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _controllsButton;
        [SerializeField] private Button _exitButton;

        private bool _isExitPopup;

        private EcsWorld _world;
        private EcsPool<StartGameButtonEvent> _startGamePool;
        private EcsPool<LoadSceneEvent> _loadScenePool;

        public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
        {
            base.Initialize(id, state, canvasMb, openData);

            _world = _state.EcsWorld;
            _startGamePool = _world.GetPool<StartGameButtonEvent>();
            _loadScenePool = _world.GetPool<LoadSceneEvent>();

            InitButtons();
        }

        private void InitButtons()
        {
            _playButton.onClick.AddListener(StartStoryGame);
            _freeRideButton.onClick.AddListener(FreeRideGame);
            _settingsButton.onClick.AddListener(OpenSettingsPanel);
            _garageButton.onClick.AddListener(OpenStorePanel);
            _controllsButton.onClick.AddListener(ShowTutorial);
            _exitButton.onClick.AddListener(OpenExitPopupPanel);

            if (_state.IsWebGl)
                _exitButton.gameObject.SetActive(false);
        }

        private void StartStoryGame() =>
            StartGame(Enums.GameModType.Story);

        private void FreeRideGame() =>
            StartGame(Enums.GameModType.FreeRide);

        private void StartGame(Enums.GameModType gameModType)
        {
            _canvasMb.PlayClickSound();

            ref var startGameComp = ref _startGamePool.Add(_world.NewEntity());
            startGameComp.GameModType = gameModType;
        }

        private void OpenSettingsPanel()
        {
            _canvasMb.PlayClickSound();
            _canvasMb.ShowPanelById(PanelsIdHolder.SettingsPanelId);
        }
           
        private void ShowTutorial()
        {
            if (_state.IsMobilePlatform)
                _canvasMb.ShowPanelById(PanelsIdHolder.TutorialInGameAndroidPanelId);
            else
                _canvasMb.ShowPanelById(PanelsIdHolder.TutorialInGameDesktopPanelId);
        }

        private void OpenExitPopupPanel()
        {
            _canvasMb.PlayClickSound();
            _canvasMb.ShowPanelById(PanelsIdHolder.ExitPopupPanelId);
        }   

        private void OpenStorePanel()
        {
            _canvasMb.PlayClickSound();
            ref var loadComp = ref _loadScenePool.Add(_world.NewEntity());
            loadComp.SceneId = ScenesIdHolder.LobbySceneId;
        }   

        public override void OnBackButtonClick()
        {
            _canvasMb.PlayClickSound();

            if (_isExitPopup)
                _canvasMb.DestroyPanelById(PanelsIdHolder.ExitPopupPanelId);
            else
                _canvasMb.ShowPanelById(PanelsIdHolder.ExitPopupPanelId);
            _isExitPopup = !_isExitPopup;
        }
    }
}