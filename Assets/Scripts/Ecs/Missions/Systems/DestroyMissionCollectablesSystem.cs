using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DestroyMissionCollectablesSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<DestroyMissionCollectableEvent>> _filter = default;
        private EcsPoolInject<DestroyMissionCollectableEvent> _destroyPool = default;
        private EcsPoolInject<MissionCollectableComp> _collectablePool = default;
        private EcsPoolInject<MinimapPanelComp> _minimapPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var collectableComp = ref _collectablePool.Value.Get(entity);
                
                ref var minimapComp = ref _minimapPool.Value.Get(_state.Value.InterfaceEntity);
                minimapComp.MinimapPanelMb.RemovePointFromMinimap(collectableComp.CollectableMb.PointId);

                GameObject.Destroy(collectableComp.CollectableMb.gameObject);

                _collectablePool.Value.Del(entity);
                _destroyPool.Value.Del(entity);
            }
        }
    }
}