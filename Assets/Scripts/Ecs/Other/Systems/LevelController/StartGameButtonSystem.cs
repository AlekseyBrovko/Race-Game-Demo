using Client;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace LevelController
{
    sealed class StartGameButtonSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<StartGameButtonEvent>> _filter = default;
        private EcsPoolInject<StartGameButtonEvent> _startGamePool = default;
        private EcsPoolInject<LoadSceneEvent> _loadScenePool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var startGameComp = ref _startGamePool.Value.Get(entity);
                SessionContextHolder.Instance.GameModeType = startGameComp.GameModType;

                ref var loadComp = ref _loadScenePool.Value.Add(_world.Value.NewEntity());
                loadComp.SceneId = ScenesIdHolder.CityLevelSceneId;

                _startGamePool.Value.Del(entity);
            }
        }
    }
}
