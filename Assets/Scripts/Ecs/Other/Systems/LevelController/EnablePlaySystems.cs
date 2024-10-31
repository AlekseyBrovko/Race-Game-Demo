using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class EnablePlaySystems : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<EnablePlaySystemsEvent>> _filter = default;
        private EcsPoolInject<TimeScaleEvent> _timeScalePool = default;
        private EcsPoolInject<ChangeGlobalStateEvent> _changeStatePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                _state.Value.PlaySystems = true;
                _state.Value.PauseSystems = false;

                _filter.Pools.Inc1.Del(entity);

                ref var timeScaleComp = ref _timeScalePool.Value.Add(_world.Value.NewEntity());
                timeScaleComp.TimeScale = 1f;

                _changeStatePool.Value.Add(_world.Value.NewEntity());
            }
        }
    }
}