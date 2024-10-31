using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarDeathSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<CarDeathEvent>> _filter = default;
        private EcsFilterInject<Inc<CarDeathComp>> _deathFilter = default;

        private EcsPoolInject<CarDeathEvent> _deathEvent = default;
        private EcsPoolInject<CarDeathComp> _deathPool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<LoseEvent> _losePool = default;

        private float _deathTimerBeforeEvent = 5f;

        public void Run(EcsSystems systems)
        {
            HandleStartDeath();
            HandleDeathState();
        }

        private void HandleStartDeath()
        {
            foreach (var entity in _filter.Value)
            {
                ref var deathEventComp = ref _deathEvent.Value.Get(entity);

                if (!_deathPool.Value.Has(deathEventComp.CarEntity) && _state.Value.EnableDeath)
                    _deathPool.Value.Add(deathEventComp.CarEntity);

                Debug.Log("CarDeathSystem");

                ref var carComp = ref _carPool.Value.Get(deathEventComp.CarEntity);
                if (carComp.DeathSmokeParticles != null)
                    foreach (var particle in carComp.DeathSmokeParticles)
                        particle.Play();

                _deathEvent.Value.Del(entity);
            }
        }

        private void HandleDeathState()
        {
            foreach (var entity in _deathFilter.Value)
            {
                ref var deatComp = ref _deathPool.Value.Get(entity);
                deatComp.Timer += Time.deltaTime;
                if (deatComp.Timer > _deathTimerBeforeEvent)
                {
                    ref var loseComp = ref _losePool.Value.Add(_world.Value.NewEntity());
                    loseComp.LoseType = Enums.LoseType.Death;
                }
            }
        }
    }
}