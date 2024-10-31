using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PlayerCarInit : IEcsInitSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<CarInitEvent> _initPool = default;
        private EcsPoolInject<PlayerSpawnPointsComp> _spawnPointsPool = default;
        private EcsPoolInject<MissionSpawnPointsComp> _missionSpawnPointsPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<RespawnPointsComp> _respawnPointsPool = default;
        private EcsPoolInject<VirtualCameraComp> _virtualCameraPool = default;

        public void Init(EcsSystems systems)
        {
            if (_state.Value.StartGameTutorial)
            {
                ref var respawnPointsPool = ref _respawnPointsPool.Value.Get(_state.Value.RespawnPointsEntity);
                RespawnPointMb randomPoint = respawnPointsPool.RespawnPoints
                    [Random.Range(0, respawnPointsPool.RespawnPoints.Length)];

                GameObject curretPlayerCarPrefab = _state.Value.PlayerCarsPrefabsConfig.TutorialCar.CarPrefab;

                GameObject playerCar = GameObject.Instantiate(
                    curretPlayerCarPrefab,
                    randomPoint.transform.position + Vector3.up,
                    randomPoint.transform.rotation);

                ref var carInitComp = ref _initPool.Value.Add(_world.Value.NewEntity());
                carInitComp.CarGo = playerCar;
            }
            else if (SessionContextHolder.Instance.GameModeType == Enums.GameModType.Story)
            {
                string pointId = _state.Value.CurrentMission.SpawnPointId;
                if (string.IsNullOrEmpty(pointId))
                    SpawnCarOnRandomRespawnPoint();
                else
                    SpawnCarOnMissionPoint(pointId);
            }
            else
            {
                int spawnPointEntity = _state.Value.PlayerSpawnPointsEntity;
                if (_spawnPointsPool.Value.Has(spawnPointEntity))
                {
                    SpawnCarOnRandomRespawnPoint();
                }
                else
                {
                    //for tests
                    GameObject playerCar = GameObject.FindGameObjectWithTag("Car");
                    ref var carInitComp = ref _initPool.Value.Add(_world.Value.NewEntity());
                    carInitComp.CarGo = playerCar;
                }
            }
        }

        private void SpawnCarOnDefaultPoint()
        {
            ref var spawnPointComp = ref _spawnPointsPool.Value.Get(_state.Value.PlayerSpawnPointsEntity);
            Transform startTranform = spawnPointComp.StartSpawnTransform;
            SpawnCar(startTranform);
        }

        private void SpawnCarOnRandomRespawnPoint()
        {
            ref var respawnPointsPool = ref _respawnPointsPool.Value.Get(_state.Value.RespawnPointsEntity);
            var randomPoint = respawnPointsPool.RespawnPoints
                [Random.Range(0, respawnPointsPool.RespawnPoints.Length)];
            SpawnCar(randomPoint.transform);
        }

        private void SpawnCarOnMissionPoint(string pointId)
        {
            ref var spawnPointsComp = ref _missionSpawnPointsPool.Value.Get(_state.Value.RespawnPointsEntity);
            Transform spawnPoint = spawnPointsComp.MissionSpawnPoints[pointId];
            SpawnCar(spawnPoint);
        }

        private void SpawnCar(Transform spawnPoint)
        {
            GameObject curretPlayerCarPrefab = _state.Value.PlayerCarsPrefabsConfig.GetCarById(_state.Value.CurrentPlayerCar);
            GameObject playerCar = GameObject.Instantiate(curretPlayerCarPrefab, spawnPoint.position + Vector3.up, spawnPoint.rotation);
            ref var carInitComp = ref _initPool.Value.Add(_world.Value.NewEntity());
            carInitComp.CarGo = playerCar;

            SetCameraAboveTheCarPosition(playerCar.transform);
        }

        private void SetCameraAboveTheCarPosition(Transform carTransform)
        {
            ref var virtualCameraComp = ref _virtualCameraPool.Value.Get(_state.Value.CameraEntity);
            virtualCameraComp.CinemachineBrain.transform.position 
                = carTransform.position - carTransform.forward * 7f + Vector3.up * 3f;
            virtualCameraComp.CinemachineBrain.transform.LookAt(carTransform.position);

            //TODO попробовать виртуальную камеру подвинуть
            //ref var cameraViewComp = ref _viewPool.Value.Get(_state.Value.CameraEntity);
            //cameraViewComp.Transform.position = carTransform.position - carTransform.forward * 7f + Vector3.up * 3f;
            //cameraViewComp.Transform.LookAt(carTransform.position);
        }
    }
}