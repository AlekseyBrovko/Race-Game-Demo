using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class LevelControllerInit : IEcsInitSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<Level> _levelPool = default;

        public void Init(EcsSystems systems)
        {
            var entity = _world.Value.NewEntity();
            _levelPool.Value.Add(entity);
            _state.Value.LevelEntity = entity;
            _state.Value.StartPlaySystems();
        }
    }
}