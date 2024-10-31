using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class TimerOnQuestUISystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<QuestTimerPanelComp>> _filter = default;
        private EcsPoolInject<QuestTimerPanelComp> _timerPanelPool = default;
        private EcsPoolInject<QuestForTimeComp> _questForTimePool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var timerPanelComp = ref _timerPanelPool.Value.Get(entity);
                ref var questComp = ref _questForTimePool.Value.Get(_state.Value.QuestEntity);

                timerPanelComp.MissionTimerPanel.ShowTimer(questComp.Timer);
            }
        }
    }
}