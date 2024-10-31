using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class RefreshQuestsSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<RefreshQuestsEvent>> _filter = default;
        private EcsPoolInject<RefreshQuestsEvent> _eventPool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {   
                QuestsConfig config = _state.Value.QuestsConfig;
                List<Quest> availableQuests = config.GetAvailableFromPointsQuests();
                foreach (var quest in availableQuests)
                    quest.ShowAnailableState();

                _eventPool.Value.Del(entity);
            }
        }
    }
}