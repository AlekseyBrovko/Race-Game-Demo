using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TimerOnMissionSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<MissionPartOnTimeComp>> _filter = default;
        private EcsPoolInject<MissionPartOnTimeComp> _missionPartOnTimePool = default;
        private EcsPoolInject<MissionPartProgressUiEvent> _uiEventPool = default;
        private EcsPoolInject<MissionFailedEvent> _failedPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var missionComp = ref _missionPartOnTimePool.Value.Get(entity);
                missionComp.Timer -= Time.deltaTime;

                if (missionComp.MissionPartSaveData.Complete)
                {
                    _missionPartOnTimePool.Value.Del(entity);
                    continue;
                }

                int roundedTimer = (int)missionComp.Timer;
                if (roundedTimer != missionComp.PreviousUITimerValue)
                {
                    missionComp.PreviousUITimerValue = roundedTimer;
                    ref var uiComp = ref _uiEventPool.Value.Add(_world.Value.NewEntity());
                    uiComp.MissionPartId = missionComp.MissionPart.Id;
                    missionComp.MissionPartSaveData.Timer = roundedTimer;
                }

                if (missionComp.Timer < 0)
                {
                    _failedPool.Value.Add(entity);
                }
            }
        }
    }
}