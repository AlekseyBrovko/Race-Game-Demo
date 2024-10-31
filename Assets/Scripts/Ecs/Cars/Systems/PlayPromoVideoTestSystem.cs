using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PlayPromoVideoTestSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;

        private EcsFilterInject<Inc<PlayPromoVideoComp>> _filter = default;
        private EcsPoolInject<PlayPromoVideoComp> _playPromoPool = default;
        private EcsPoolInject<OnStopInGamePromoVideoEvent> _stopPromoPool = default;

        private float _durationOfVideo = 5f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var videoComp = ref _playPromoPool.Value.Get(entity);
                videoComp.Timer += Time.unscaledDeltaTime;
                Debug.Log("крутим видео");

                if (videoComp.Timer > _durationOfVideo)
                    _stopPromoPool.Value.Add(_world.Value.NewEntity());
            }
        }
    }
}