using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class StopKillQuestChapterSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<StopKillChapterEvent>> _filter = default;
        private EcsPoolInject<StopKillChapterEvent> _stopChapterPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;
        private EcsPoolInject<KillQuestChapterComp> _killChapterPool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var stopComp = ref _stopChapterPool.Value.Get(entity);
                stopComp.CurrentQuest.NextChapter();

                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.DestroyPanelById(PanelsIdHolder.MissionCounterPanelId);

                if (_killChapterPool.Value.Has(_state.Value.QuestEntity))
                    _killChapterPool.Value.Del(_state.Value.QuestEntity);


                _stopChapterPool.Value.Del(entity);
            }
        }
    }
}