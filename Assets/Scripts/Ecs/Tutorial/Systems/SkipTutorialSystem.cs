using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class SkipTutorialSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<SkipTutorialEvent>> _filter = default;
        private EcsPoolInject<SkipTutorialEvent> _skipPool = default;
        private EcsPoolInject<LoadSceneEvent> _loadScenePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var loadSceneComp = ref _loadScenePool.Value.Add(_world.Value.NewEntity());
                loadSceneComp.SceneId = ScenesIdHolder.StartSceneId;

                _state.Value.StartGameTutorial = false;
                _state.Value.SaveTutorial();

                _skipPool.Value.Del(entity);
            }
        }
    }
}