using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class QuestChapterForTimeSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<QuestForTimeComp>> _filter = default;
        private EcsPoolInject<QuestForTimeComp> _questForTimePool = default;
        private EcsPoolInject<QuestFailedEvent> _questFailedPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var questForTimeComp = ref _questForTimePool.Value.Get(entity);
                questForTimeComp.Timer -= Time.deltaTime;

                if (questForTimeComp.Timer < 0)
                {
                    Debug.Log("Время Вышло");
                    ref var questFailedComp = ref _questFailedPool.Value.Add(entity);
                    questFailedComp.FailedQuest = questForTimeComp.CurrentQuest;
                    questFailedComp.QuestChapter = questForTimeComp.CurrentQuestChapter;

                    ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                    interfaceComp.CanvasBehaviour.DestroyPanelById(PanelsIdHolder.MissionTimerPanelId);

                    _questForTimePool.Value.Del(entity);
                }
            }
        }
    }
}