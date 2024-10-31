using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class ZoneChangeSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CrossZoneEvent>> _filter = default;
        private EcsPoolInject<CrossZoneEvent> _crossZonePool = default;
        private EcsPoolInject<PlayerCarZoneComp> _playerZonePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var zoneComp = ref _crossZonePool.Value.Get(entity);
                ref var playerZoneComp = ref _playerZonePool.Value.Get(_state.Value.PlayerCarEntity);
                playerZoneComp.IslandName = zoneComp.IslandName;
                _crossZonePool.Value.Del(entity);
            }
        }
    }
}