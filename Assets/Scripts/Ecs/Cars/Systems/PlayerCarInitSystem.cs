using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PlayerCarInitSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<PlayerCarInitEvent>> _filter = default;
        private EcsPoolInject<PlayerCarInitEvent> _eventPool = default;
        private EcsPoolInject<ChangeCameraTargetEvent> _cameraChangePool = default;

        private EcsFilterInject<Inc<PlayerCarComp>> _playerFilter = default;
        private EcsPoolInject<PlayerCarComp> _playerCarPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<CarWheelsControllComp> _wheelControllPool = default;
        private EcsPoolInject<CarFuelComp> _carFuelPool = default;
        private EcsPoolInject<CarComp> _carPool = default;

        private EcsPoolInject<CameraChangePositionEvent> _cameraChangePositionEvent = default;

        private EcsPoolInject<PlayerCarZoneComp> _playerCarZonePool = default;
        private EcsPoolInject<RespawnPointsComp> _respawnPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                //если нет машин, то не перемещаем,
                //если есть машины, то берем позицию
                Vector3? tempPosition = null;
                Quaternion? tempRotation = null;

                if (_playerFilter.Value.GetEntitiesCount() > 0)
                    RemovePlayerCarOnScene(ref tempPosition, ref tempRotation);

                _state.Value.PlayerCarEntity = entity;
                _playerCarPool.Value.Add(entity);

                ref var viewPlayerComp = ref _viewPool.Value.Get(entity);
                ref var changeCameraComp = ref _cameraChangePool.Value.Add(entity);
                changeCameraComp.TargetGo = viewPlayerComp.Transform.gameObject;

                if (tempPosition != null)
                {
                    viewPlayerComp.Transform.position = tempPosition.Value;
                    viewPlayerComp.Transform.rotation = tempRotation.Value;
                }

                if (_state.Value.WithFuel)
                {
                    ref var carComp = ref _carPool.Value.Get(entity);

                    ref var fuelComp = ref _carFuelPool.Value.Add(entity);
                    fuelComp.FullTankOfGasValue = carComp.CarCharacteristic.FuelLevel;
                    fuelComp.CurrentValue = carComp.CarCharacteristic.FuelLevel;
                }

                _cameraChangePositionEvent.Value.Add(_world.Value.NewEntity());

                InitZoneComp(entity);

                _eventPool.Value.Del(entity);
            }
        }

        private void RemovePlayerCarOnScene(ref Vector3? tempPosition, ref Quaternion? tempRotation)
        {
            foreach (var playerCarEntity in _playerFilter.Value)
            {
                ref var wheelControllComp = ref _wheelControllPool.Value.Get(playerCarEntity);
                foreach (var wheelEntity in wheelControllComp.WheelsEntities)
                    _world.Value.DelEntity(wheelEntity);

                ref var viewComp = ref _viewPool.Value.Get(playerCarEntity);
                GameObject carGO = viewComp.Transform.gameObject;
                tempPosition = viewComp.Transform.position;
                tempRotation = viewComp.Transform.rotation;
                GameObject.DestroyImmediate(carGO);
                _world.Value.DelEntity(playerCarEntity);
            }
        }

        private void InitZoneComp(int entity)
        {
            ref var respawnPointsComp = ref _respawnPool.Value.Get(_state.Value.RespawnPointsEntity);
            ref var playerZoneComp = ref _playerCarZonePool.Value.Add(entity);
            ref var carComp = ref _carPool.Value.Get(entity);

            float distance = float.MaxValue;
            RespawnPointMb nearestPoint = null;
            Vector3 carPosition = carComp.CarTransform.position;

            foreach (RespawnPointMb pointMb in respawnPointsComp.RespawnPoints)
            {
                Vector3 diretion = pointMb.transform.position - carPosition;
                float tempDistance = diretion.sqrMagnitude;
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    nearestPoint = pointMb;
                }
            }

            playerZoneComp.IslandName = nearestPoint.IslandName;
        }
    }
}