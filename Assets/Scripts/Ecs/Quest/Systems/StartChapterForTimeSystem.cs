using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class StartChapterForTimeSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<StartTimeLimitChapterEvent>> _filter = default;
        private EcsPoolInject<StartTimeLimitChapterEvent> _startPool = default;
        private EcsPoolInject<QuestForTimeComp> _timeChapterPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var startComp = ref _startPool.Value.Get(entity);
                ref var timeChapterComp = ref _timeChapterPool.Value.Add(_state.Value.QuestEntity);

                timeChapterComp.CurrentQuest = startComp.CurrentQuest;
                timeChapterComp.CurrentQuestChapter = startComp.CurrentQuestChapter;
                timeChapterComp.TimeDuration = startComp.Time;
                timeChapterComp.Timer = startComp.Time;

                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.MissionTimerPanelId);

                _startPool.Value.Del(entity);
            }
        }
    }
}