using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarOutOfFuelSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<CarOutOfFuelComp>> _filter = default;
        private EcsPoolInject<CarOutOfFuelComp> _outOfFuelPool = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<LoseEvent> _losePool = default;

        private float _outOfFuelTimerDuration = 5f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var carOutFuelComp = ref _outOfFuelPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);

                carOutFuelComp.Timer += Time.deltaTime;

                if (carOutFuelComp.Timer > _outOfFuelTimerDuration)
                    HandleOutOfFuel();
                else if (statisticComp.SpeedKmpH < 2f)
                    HandleOutOfFuel();
            }
        }

        private void HandleOutOfFuel()
        {
            Debug.Log("out of fuel");
            ref var loseComp = ref _losePool.Value.Add(_world.Value.NewEntity());
            loseComp.LoseType = Enums.LoseType.Fuel;
        }
    }
}