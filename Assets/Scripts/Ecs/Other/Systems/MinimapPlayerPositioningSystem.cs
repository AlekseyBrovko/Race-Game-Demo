using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class MinimapPlayerPositioningSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<MinimapPanelComp>> _minimapFilter;
        private EcsFilterInject<Inc<PlayerCarComp>> _playerFilter;

        private EcsPoolInject<MinimapPanelComp> _minimapPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _minimapFilter.Value)
            {
                foreach (var playerEntity in _playerFilter.Value)
                {
                    ref var minimapComp = ref _minimapPool.Value.Get(entity);
                    ref var playerViewComp = ref _viewPool.Value.Get(playerEntity);

                    minimapComp.MinimapPanelMb.SetRotationForMinimap(playerViewComp.Transform.rotation.eulerAngles.y);

                    minimapComp.MinimapPanelMb.SetPositionForMinimap(playerViewComp.Transform.position);
                }
            }
        }
    }
}