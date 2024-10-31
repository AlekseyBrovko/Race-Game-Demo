using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class UnfadeCanvasInit : IEcsInitSystem
    {
        private EcsCustomInject<GameState> _state = default;

        public void Init(EcsSystems systems)
        {
            if (_state.Value.SceneType == Enums.SceneType.StartScene)
                if (FadeCanvas.Instance != null)
                    FadeCanvas.Instance.DestroyFadeCanvas();

            if (FadeCanvas.Instance != null)
                FadeCanvas.Instance.Unfade();
        }
    }
}