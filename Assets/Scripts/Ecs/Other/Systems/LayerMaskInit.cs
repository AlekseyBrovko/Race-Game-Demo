using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class LayerMaskInit : IEcsInitSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<LayerMaskComp> _layersPool = default;

        public void Init(EcsSystems systems)
        {
            int layerMaskEntity = _world.Value.NewEntity();
            _state.Value.LayerMaskEntity = layerMaskEntity;

            ref var layersComp = ref _layersPool.Value.Add(layerMaskEntity);
            layersComp.DefaultLayer = 1 << 0;

            layersComp.EnemyNpcMask = 1 << 6;
            layersComp.FriendlyNpcMask = 1 << 7;

            layersComp.EnemyCarMask = 1 << 9;
            layersComp.FriendlyCarMask = 1 << 10;

            layersComp.EnemyMask = 1 << 6 | 1 << 9;
            layersComp.FriendlyMask = 1 << 7 | 1 << 10;

            layersComp.PatrollPointsMask = 1 << 11;
        }
    }
}