using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class UiSpeedSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<InGamePanelComp>> _filter = default;
        private EcsFilterInject<Inc<PlayerCarComp>> _playerCarFilter = default;

        private EcsPoolInject<InGamePanelComp> _panelPool = default;
        private EcsPoolInject<CarStatisticComp> _carStatisticPool = default;

        private float _timer = 0;
        private float _duration = 0.1f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                foreach (var playerEntity in _playerCarFilter.Value)
                {
                    _timer += Time.deltaTime;
                    if (_timer > _duration)
                    {
                        _timer = 0;
                        ref var panelComp = ref _panelPool.Value.Get(entity);
                        ref var statisticComp = ref _carStatisticPool.Value.Get(playerEntity);
                        panelComp.InGamePanelMb.ShowSpeed(Mathf.FloorToInt(statisticComp.SpeedKmpH));
                    }
                }
            }
        }
    }
}