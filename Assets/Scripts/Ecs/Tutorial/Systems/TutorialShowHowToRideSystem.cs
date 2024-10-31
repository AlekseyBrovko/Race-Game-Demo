using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TutorialShowHowToRideSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<ShowHowToRideComp>, Exc<TutorialTimerComp>> _stageFilter = default;
        private EcsPoolInject<ShowHowToRideComp> _howToRidePool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _stageFilter.Value)
            {
                _state.Value.StartPauseSystems();

                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour
                    .ShowPanelById(PanelsIdHolder.TutorialLevelAndroidPanel, 
                        new TutorialLevelPanelData(Enums.TutorialPartType.ShowHowToRide));

                Debug.Log("TutorialShowHowToRideSystem");

                //TODO тормозим время, показываем панельку

                _howToRidePool.Value.Del(entity);
            }
        }
    }
}