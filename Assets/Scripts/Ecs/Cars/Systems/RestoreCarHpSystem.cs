using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class RestoreCarHpSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<RestoreCarHpEvent>> _filter = default;
        private EcsPoolInject<RestoreCarHpEvent> _restoreHpPool = default;

        private EcsPoolInject<HpComp> _hpPool = default;
        private EcsPoolInject<VisualHpEvent> _uiHpPool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<CarDeathComp> _deathPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                int playerCarEntity = _state.Value.PlayerCarEntity;
                ref var hpComp = ref _hpPool.Value.Get(playerCarEntity);
                hpComp.HpValue = hpComp.FullHpValue;
                _uiHpPool.Value.Add(_world.Value.NewEntity());

                ref var carComp = ref _carPool.Value.Get(playerCarEntity);
                if (carComp.DeathSmokeParticles != null)
                    foreach (var particle in carComp.DeathSmokeParticles)
                        particle.Stop();

                if (_deathPool.Value.Has(playerCarEntity))
                    _deathPool.Value.Del(playerCarEntity);

                _restoreHpPool.Value.Del(entity);
            }
        }
    }
}