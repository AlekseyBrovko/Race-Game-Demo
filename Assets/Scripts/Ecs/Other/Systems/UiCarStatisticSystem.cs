using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class UiCarStatisticSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CarStatisticPanelComp>> _filter = default;
        private EcsPoolInject<CarStatisticPanelComp> _panelPool = default;
        private EcsPoolInject<CarStatisticComp> _carStatisticPool = default;
        private EcsPoolInject<CarTahometerComp> _tahoPool = default;

        private float _timer = 0;
        private float _duration = 0.1f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var panelComp = ref _panelPool.Value.Get(entity);
                _timer += Time.deltaTime;

                if (_timer > _duration)
                {
                    _timer = 0;

                    ref var statisticComp = ref _carStatisticPool.Value.Get(_state.Value.PlayerCarEntity);
                    ref var tahoComp = ref _tahoPool.Value.Get(_state.Value.PlayerCarEntity);

                    panelComp.CarStatisticPanelMb.ShowSpeed(Mathf.FloorToInt(statisticComp.SpeedKmpH));
                    panelComp.CarStatisticPanelMb.ShowTaho(Mathf.FloorToInt(tahoComp.CurrentValueRpm));
                }
            }
        }
    }
}