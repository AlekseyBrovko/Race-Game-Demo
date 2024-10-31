using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;

namespace Client
{
    sealed class StartQuestSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<QuestStartEvent>> _filter = default;
        private EcsPoolInject<QuestStartEvent> _startQuestPool = default;
        private EcsPoolInject<QuestComp> _questPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var questComp = ref _questPool.Value.Get(_state.Value.QuestEntity);
                ref var startQuestComp = ref _startQuestPool.Value.Get(entity);

                List<Quest> availableQuests = _state.Value.QuestsConfig.GetAvailableFromPointsQuests();
                foreach (var quest in availableQuests)
                    quest.HideAvailableState();

                startQuestComp.QuestToStart.StartQuest();

                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.MessagePanelId, 
                    new DataForMessagePanel(Enums.MessageTypeEnum.MissionStartMessage));

                _startQuestPool.Value.Del(entity);
            }
        }
    }
}