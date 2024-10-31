using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;

namespace Client
{
    sealed class QuestsInit : IEcsInitSystem
    {
        private EcsWorldInject _world;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<QuestComp> _questPool = default;
        private EcsPoolInject<RefreshQuestsEvent> _eventPool = default;

        public void Init(EcsSystems systems)
        {
            int questEntity = _world.Value.NewEntity();
            _state.Value.QuestEntity = questEntity;

            _state.Value.QuestsConfig.InitQuests(_state.Value);

            ref var questComp = ref _questPool.Value.Add(questEntity);
            questComp.Quests = new List<Quest>();

            _eventPool.Value.Add(_world.Value.NewEntity());
        }
    }
}