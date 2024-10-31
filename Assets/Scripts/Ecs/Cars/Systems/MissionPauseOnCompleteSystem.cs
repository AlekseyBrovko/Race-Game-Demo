using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class MissionPauseOnCompleteSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<MissionCompleteAfterPauseEvent>> _filter = default;
        private EcsPoolInject<MissionCompleteAfterPauseEvent> _missionCompletePool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;
        private EcsPoolInject<SilentMoneyEvent> _moneyPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {   
                ShowPanel();
                HandleMissionMoney();

                _state.Value.PlayerMoneyScore += _state.Value.CurrentMission.MoneyReward;
                _state.Value.SaveMoneyScore();
                _missionCompletePool.Value.Del(entity);
                Pause();
            }
        }

        private void Pause() =>
            _state.Value.StartPauseSystems();

        private void ShowPanel()
        {
            ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
            interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.MissionCompletePanelId);
        }

        private void HandleMissionMoney()
        {
            ref var moneyEvent = ref _moneyPool.Value.Add(_world.Value.NewEntity());
            moneyEvent.MoneyValue = _state.Value.CurrentMission.MoneyReward;
        }
    }
}