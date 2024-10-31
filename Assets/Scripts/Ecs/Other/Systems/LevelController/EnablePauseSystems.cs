using FMODUnity;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class EnablePauseSystems : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<EnablePauseSystemsEvent>> _filter = default;
        private EcsPoolInject<TimeScaleEvent> _timeScalePool = default;
        private EcsPoolInject<ChangeGlobalStateEvent> _changeStatePool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                _state.Value.PlaySystems = false;
                _state.Value.PauseSystems = true;

                _filter.Pools.Inc1.Del(entity);

                ref var timeScaleComp = ref _timeScalePool.Value.Add(_world.Value.NewEntity());
                timeScaleComp.TimeScale = 0;

                _changeStatePool.Value.Add(_world.Value.NewEntity());
            }
        }
    }
}