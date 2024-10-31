using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class MissionCompleteSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<MissionCompleteEvent>> _completeEventFilter = default;
        private EcsFilterInject<Inc<MissionCompleteComp>> _completeCompFilter = default;

        private EcsPoolInject<MissionCompleteEvent> _completeEventPool = default;
        private EcsPoolInject<MissionCompleteComp> _completeCompPool = default;
        private EcsPoolInject<MissionCompleteAfterPauseEvent> _missionCompletePool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;
        private EcsPoolInject<UISoundEvent> _soundPool = default;

        private float _timerBeforeShowUI = 6f;

        public void Run(EcsSystems systems)
        {
            HandleCompleteStart();
            HandleCompleteTimer();
        }

        private void HandleCompleteStart()
        {
            foreach (var entity in _completeEventFilter.Value)
            {
                //TODO здесь крутануть звук прохождения миссии
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.DestroyPanelById(PanelsIdHolder.MissionPanelId);
                interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.MissionCompletePopupPanelId);

                PlayMissionCompleteSound();

                _completeCompPool.Value.Add(entity);
                _completeEventPool.Value.Del(entity);
            }
        }

        private void PlayMissionCompleteSound()
        {
            ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
            soundComp.Sound = Enums.SoundEnum.MissionCompleteSound;
        }

        private void HandleCompleteTimer()
        {
            foreach (var entity in _completeCompFilter.Value)
            {
                ref var completeComp = ref _completeCompPool.Value.Get(entity);
                completeComp.Timer += Time.deltaTime;

                if (completeComp.Timer > _timerBeforeShowUI)
                {
                    //по этому событию уже показывается панель с видосом и удвоение денег
                    _missionCompletePool.Value.Add(entity);
                    _completeCompPool.Value.Del(entity);
                }
            }
        }
    }
}