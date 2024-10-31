using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public class ExitPopupPanel : PanelBase, IPopupPanel
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _exitButton;

        private EcsWorld _world;
        private EcsPool<QuitEvent> _quitPool;

        public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
        {
            base.Initialize(id, state, canvasMb, openData);

            _world = _state.EcsWorld;
            _quitPool = _world.GetPool<QuitEvent>();

            _closeButton.onClick.AddListener(() =>
            {
                _canvasMb.PlayClickSound();
                CloseExitPopup();
            });

            _exitButton.onClick.AddListener(() =>
            {
                _canvasMb.PlayClickSound();
                Exit();
            });
        }

        public void CloseExitPopup() =>
            _canvasMb.PanelsController.DestroyPanelById(Id);

        private void Exit() =>
            _quitPool.Add(_world.NewEntity());
    }
}