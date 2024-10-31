using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;
using UnityEngine;

namespace Client
{
    sealed class PickupsSpawnSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<SpawnPickupEvent>> _spawnFilter = default;
        private EcsPoolInject<SpawnPickupEvent> _spawnPool = default;

        private EcsFilterInject<Inc<SpawnPickupCoolDownComp>> _coolDownFilter = default;
        private EcsPoolInject<SpawnPickupCoolDownComp> _coolDownPool = default;
        private EcsPoolInject<PickupSpawnPointComp> _spawnerPool = default;

        private float _coolDown = 60f;

        public void Run(EcsSystems systems)
        {
            HandleCoolDown();
            HandleSpawn();
        }

        private void HandleSpawn()
        {
            foreach (var entity in _spawnFilter.Value)
            {
                ref var spawnComp = ref _spawnPool.Value.Get(entity);
                ref var spawnerComp = ref _spawnerPool.Value.Get(entity);

                GameObject prefab = GetPrefabFromStore(spawnerComp.PickupPointMb.PickupType);
                GameObject pickupGo = GameObject.Instantiate(prefab, spawnComp.Position, Quaternion.identity);
                PickupMb pickupMb = pickupGo.GetComponent<PickupMb>();
                pickupMb.SetSpawnerEntity(entity);

                _spawnPool.Value.Del(entity);
            }
        }

        private void HandleCoolDown()
        {
            foreach (var entity in _coolDownFilter.Value)
            {
                ref var coolDownComp = ref _coolDownPool.Value.Get(entity);
                coolDownComp.Timer += Time.deltaTime;

                if (coolDownComp.Timer > _coolDown)
                {
                    ref var spawnComp = ref _spawnPool.Value.Add(entity);
                    spawnComp.Position = coolDownComp.Position;
                    _coolDownPool.Value.Del(entity);
                }
            }
        }

        private GameObject GetPrefabFromStore(Enums.PickupType pickupType)
        {
            if (pickupType == Enums.PickupType.Random)
                return GetPickupByWeight();
            else
                return _state.Value.PickupsConfig.Pickups.FirstOrDefault(x => x.PickupType == pickupType).PickupPrefab;
        }

        private GameObject GetPickupByWeight()
        {
            float allWeight = 0;
            foreach (var item in _state.Value.PickupsConfig.Pickups)
                allWeight += item.Weight;

            float random = Random.Range(0, allWeight);

            float tempWeight = 0;

            for (int i = 0; i < _state.Value.PickupsConfig.Pickups.Length; i++)
            {
                tempWeight += _state.Value.PickupsConfig.Pickups[i].Weight;
                if (random <= tempWeight)
                    return _state.Value.PickupsConfig.Pickups[i].PickupPrefab;
            }

            Debug.LogWarning("Ошибка в алгоритме подбора");
            return _state.Value.PickupsConfig.Pickups[0].PickupPrefab;
        }
    }
}