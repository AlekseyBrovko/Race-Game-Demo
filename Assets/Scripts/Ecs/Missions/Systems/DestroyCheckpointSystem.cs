using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public sealed class DestroyCheckpointSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<DestroyCheckpointEvent>> _filter = default;

        private EcsPoolInject<DestroyCheckpointEvent> _destroyPool = default;
        private EcsPoolInject<MinimapPanelComp> _minimapPool = default;
        private EcsPoolInject<MissionCheckpointComp> _checkpointPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var checkpointComp = ref _checkpointPool.Value.Get(entity);

                ref var minimapComp = ref _minimapPool.Value.Get(_state.Value.InterfaceEntity);
                minimapComp.MinimapPanelMb.RemovePointFromMinimap(checkpointComp.CheckpointMb.PointId);

                GameObject.Destroy(checkpointComp.CheckpointMb.gameObject);

                _checkpointPool.Value.Del(entity);
                _destroyPool.Value.Del(entity);
            }
        }
    }
}