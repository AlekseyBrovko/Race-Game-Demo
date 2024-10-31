using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class LoseSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<LoseEvent>> _filter = default;
        private EcsPoolInject<LoseEvent> _losePool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                Debug.Log("LoseSystem");
                ref var loseComp = ref _losePool.Value.Get(entity);
                ShowPanel(loseComp.LoseType);
                _losePool.Value.Del(entity);
                Pause();
            }
        }

        private void Pause() =>
            _state.Value.StartPauseSystems();

        private void ShowPanel(Enums.LoseType loseType)
        {
            ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
            DataForOpenLosePanel dataForPanel = new DataForOpenLosePanel(loseType);
            interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.LosePanelId, dataForPanel);
        }
    }
}