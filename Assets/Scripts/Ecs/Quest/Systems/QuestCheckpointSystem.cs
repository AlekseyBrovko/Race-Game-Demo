using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class QuestCheckpointSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<QuestCheckpointEvent>> _filter = default;
        private EcsPoolInject<QuestCheckpointEvent> _checkpointPool = default;
        private EcsPoolInject<QuestComp> _questsPool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var questComp = ref _questsPool.Value.Get(_state.Value.QuestEntity);
                ref var checkpointComp = ref _checkpointPool.Value.Get(entity);

                checkpointComp.CurrentQuest.NextChapter();

                _checkpointPool.Value.Del(entity);
            }
        }
    }
}