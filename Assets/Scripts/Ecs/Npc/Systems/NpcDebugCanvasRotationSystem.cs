using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class NpcDebugCanvasRotationSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<NpcDebugCanvasComp>> _filter = default;
        private EcsPoolInject<NpcDebugCanvasComp> _canvasPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        public void Run(EcsSystems systems)
        {
            ref var cameraViewComp = ref _viewPool.Value.Get(_state.Value.CameraEntity);
            foreach (var entity in _filter.Value)
            {
                ref var canvasComp = ref _canvasPool.Value.Get(entity);
                canvasComp.CanvasMb.transform.rotation = cameraViewComp.Transform.rotation;
            }
        }
    }
}