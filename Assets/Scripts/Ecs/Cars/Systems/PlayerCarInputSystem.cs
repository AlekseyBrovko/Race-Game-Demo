using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class PlayerCarInputSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<PlayerCarComp>> _filter = default;
        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<InputComp> _inputPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);

                controllComp.IsHandBrake = inputComp.IsSpacePressed;
                controllComp.HorizontalInput = inputComp.AxisKeyBoardValue.x;
                controllComp.VerticalInput = inputComp.AxisKeyBoardValue.y;
            }
        }
    }
}