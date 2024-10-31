using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class StartPlayPromoVideoSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        
        private EcsFilterInject<Inc<PlayInGamePromoVideoEvent>> _filter = default;
        private EcsPoolInject<PlayInGamePromoVideoEvent> _startVideoPool = default;
        private EcsPoolInject<PlayPromoVideoComp> _playPromoPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var startPlayComp = ref _startVideoPool.Value.Get(entity);
                ref var playPromoComp = ref _playPromoPool.Value.Add(_world.Value.NewEntity());
                playPromoComp.Reason = startPlayComp.Reason;

                SoundsHandlerMb.Instance.PauseMusic();

                _startVideoPool.Value.Del(entity);
                Debug.Log("включаем видос");
            }
        }
    }
}