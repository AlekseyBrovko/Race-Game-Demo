using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class StartKillQuestChapterSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<StartKillChapterEvent>> _filter = default;
        private EcsPoolInject<StartKillChapterEvent> _startPool = default;
        private EcsPoolInject<KillQuestChapterComp> _killChapterPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var startComp = ref _startPool.Value.Get(entity);
                ref var killQuestComp = ref _killChapterPool.Value.Add(_state.Value.QuestEntity);

                killQuestComp.CurrentQuest = startComp.CurrentQuest;
                killQuestComp.KillChapter = startComp.KillChapter;
                killQuestComp.NameOfMonster = startComp.NameOfMonster;
                killQuestComp.KillChapter = startComp.KillChapter;
                killQuestComp.Amount = startComp.Amount;

                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.MissionCounterPanelId, 
                    new DataForOpenMissionCounterPanel(startComp.NameOfMonster, killQuestComp.Amount));

                _startPool.Value.Del(entity);
            }
        }
    }
}