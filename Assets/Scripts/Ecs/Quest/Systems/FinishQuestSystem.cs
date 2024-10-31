using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class FinishQuestSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<QuestFinishEvent>> _filter = default;
        private EcsPoolInject<QuestFinishEvent> _finishPool = default;
        private EcsPoolInject<QuestComp> _questPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var finishComp = ref _finishPool.Value.Get(entity);
                ref var questComp = ref _questPool.Value.Get(_state.Value.QuestEntity);

                List<Quest> availableQuests = _state.Value.QuestsConfig.GetAvailableFromPointsQuests();
                foreach (var quest in availableQuests)
                    quest.ShowAnailableState();

                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.MessagePanelId, 
                    new DataForMessagePanel(Enums.MessageTypeEnum.MissionComplete));

                switch(finishComp.QuestReward)
                {
                    case MoneyQuestReward:
                        MoneyQuestReward moneyReward = finishComp.QuestReward as MoneyQuestReward;
                        Debug.Log("moneyReward.MoneyValue = " + moneyReward.MoneyValue);
                        break;
                }

                _finishPool.Value.Del(entity);
            }
        }
    }
}