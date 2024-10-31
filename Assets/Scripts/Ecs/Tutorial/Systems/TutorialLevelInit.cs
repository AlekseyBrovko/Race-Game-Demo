using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class TutorialLevelInit : IEcsInitSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<TutorialComp> _tutorialPool = default;

        private EcsPoolInject<ShowHowToRideComp> _howToRidePool = default;
        private EcsPoolInject<TutorialTimerComp> _tutorialTimerPool = default;

        public void Init(EcsSystems systems)
        {
            if (_state.Value.StartGameTutorial)
            {
                int tutorialEntity = _state.Value.TutorialEntity = _world.Value.NewEntity();
                _tutorialPool.Value.Add(tutorialEntity);

                ref var tutorialTimerComp = ref _tutorialTimerPool.Value.Add(_state.Value.TutorialEntity);
                tutorialTimerComp.Timer = TutorialTimers.ShowHowToRide;

                _howToRidePool.Value.Add(_state.Value.TutorialEntity);
            }
        }
    }
}