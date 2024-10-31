using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class StopChapterForTimeSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<StopTimeLimitChapterEvent>> _filter = default;
        private EcsPoolInject<StopTimeLimitChapterEvent> _eventPool = default;
        private EcsPoolInject<QuestForTimeComp> _questForTimePool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                int questEntity = _state.Value.QuestEntity;
                if (_questForTimePool.Value.Has(questEntity))
                    _questForTimePool.Value.Del(questEntity);

                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.DestroyPanelById(PanelsIdHolder.MissionTimerPanelId);

                _eventPool.Value.Del(entity);
            }
        }
    }
}