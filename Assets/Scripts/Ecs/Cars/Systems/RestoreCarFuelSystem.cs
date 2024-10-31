using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class RestoreCarFuelSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<RestoreCarFuelEvent>> _filter = default;
        private EcsPoolInject<RestoreCarFuelEvent> _restoreFuelPool = default;
        private EcsPoolInject<CarOutOfFuelComp> _outOfFuelPool = default;
        private EcsPoolInject<CarFuelComp> _carFuelPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                Debug.Log("RestoreCarFuelSystem");
                ref var fuelComp = ref _carFuelPool.Value.Get(entity);
                fuelComp.CurrentValue = fuelComp.FullTankOfGasValue;
                fuelComp.ShowLowFuel = false;

                if (_outOfFuelPool.Value.Has(entity))
                {
                    Debug.Log("_outOfFuelPool.Value.Del(entity);");
                    _outOfFuelPool.Value.Del(entity);
                }

                _restoreFuelPool.Value.Del(entity);
            }
        }
    }
}