using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class MoveCarsCoolDownSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<MoveCarsCoolDownComp>> _filter = default;
        private EcsPoolInject<MoveCarsCoolDownComp> _coolDownPool = default;
        private EcsPoolInject<LobbyPanelComp> _lobbyPanelPool = default;
        private EcsPoolInject<CarShopViewEvent> _carShopViewEventPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var collDownComp = ref _coolDownPool.Value.Get(entity);
                collDownComp.Timer -= Time.deltaTime;
                if (collDownComp.Timer < 0)
                {
                    ref var lobbyPanelComp = ref _lobbyPanelPool.Value.Get(entity);
                    lobbyPanelComp.LobbyPanelMb.OnStopMoveAnimation();
                    _coolDownPool.Value.Del(entity);
                    _carShopViewEventPool.Value.Add(_world.Value.NewEntity());
                }
            }
        }
    }
}