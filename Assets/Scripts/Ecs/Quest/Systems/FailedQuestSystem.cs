using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class FailedQuestSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<QuestFailedEvent>> _filter = default;
        private EcsPoolInject<QuestFailedEvent> _failedPool = default;
        private EcsPoolInject<QuestComp> _questPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;
        private EcsPoolInject<RefreshQuestsEvent> _eventPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var failedComp = ref _failedPool.Value.Get(entity);
                ref var questComp = ref _questPool.Value.Get(_state.Value.QuestEntity);

                if (failedComp.FailedQuest.CanRepeateQuest)
                    failedComp.FailedQuest.QuestState = Enums.QuestState.Available;
                else
                    failedComp.FailedQuest.QuestState = Enums.QuestState.Failed;

                failedComp.FailedQuest.FailQuest();

                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.MessagePanelId, new DataForMessagePanel(Enums.MessageTypeEnum.MissionFailed));

                _eventPool.Value.Add(_world.Value.NewEntity());

                _failedPool.Value.Del(entity);
            }
        }
    }
}