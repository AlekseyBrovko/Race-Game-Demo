using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarSpawnSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarSpawnEvent>> _filter = default;
        private EcsPoolInject<CarSpawnEvent> _eventPool = default;
        private EcsPoolInject<CarInitEvent> _carInitPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var carSpawnComp = ref _eventPool.Value.Get(entity);
                Vector3 pos = Vector3.zero;
                Quaternion rot = Quaternion.identity;
                if (carSpawnComp.Position != null)
                {
                    pos = carSpawnComp.Position.Value;
                    rot = carSpawnComp.Rotation.Value;
                }

                ref var carInitComp = ref _carInitPool.Value.Add(entity);
                carInitComp.CarGo = GameObject.Instantiate(carSpawnComp.CarSo.CarPrefab, pos, rot);

                _eventPool.Value.Del(entity);
            }
        }
    }
}