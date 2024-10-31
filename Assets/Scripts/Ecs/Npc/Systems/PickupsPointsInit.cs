using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PickupsPointsInit : IEcsInitSystem
    {
        public PickupsPointsInit(SceneIniterMb sceneIniter) =>
            _sceneIniter = sceneIniter;

        private SceneIniterMb _sceneIniter;

        private EcsWorldInject _world = default;
        private EcsPoolInject<SpawnPickupEvent> _spawnPool = default;
        private EcsPoolInject<PickupSpawnPointComp> _spawnerPool = default;

        public void Init(EcsSystems systems)
        {
            foreach (Transform child in _sceneIniter.PickupsPointsHolder)
            {
                if (child.TryGetComponent(out PickupPointMb pickupPointMb))
                {
                    int spawnerEntity = _world.Value.NewEntity();
                    ref var spawnerComp = ref _spawnerPool.Value.Add(spawnerEntity);
                    spawnerComp.PickupPointMb = pickupPointMb;

                    ref var spawnComp = ref _spawnPool.Value.Add(spawnerEntity);
                    spawnComp.Position = child.position + Vector3.up;
                }
            }
        }
    }
}