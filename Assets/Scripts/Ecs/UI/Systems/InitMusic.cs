using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class InitMusic : IEcsInitSystem
    {
        private EcsCustomInject<GameState> _state = default;

        public void Init(EcsSystems systems)
        {
            switch (_state.Value.SceneType)
            {
                case Enums.SceneType.PlayScene:
                    SoundsHandlerMb.Instance.PlayInGameMusic();
                    break;

                case Enums.SceneType.StartScene:
                    SoundsHandlerMb.Instance.PlayMenuMusic();
                    break;

                case Enums.SceneType.LobbyScene:
                    SoundsHandlerMb.Instance.PlayMenuMusic();
                    break;
            }
        }
    }
}