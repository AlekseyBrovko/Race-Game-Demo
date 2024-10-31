using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InputFromInputActionSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<InputComp>> _inputFilter = default;
        private EcsPoolInject<InputComp> _inputPool = default;

        public void Run(EcsSystems systems)
        {
            HandleAllSystems();
        }

        private void HandleAllSystems()
        {
            foreach (var entity in _inputFilter.Value)
            {
                ref var inputComp = ref _inputPool.Value.Get(entity);

                inputComp.AxisJoystickValue = inputComp.Controller.Action.Joystick.ReadValue<Vector2>();

                float horizontal = inputComp.Controller.Action.HorizontalAxis.ReadValue<float>();
                float vertical = inputComp.Controller.Action.VerticalAxis.ReadValue<float>();
                inputComp.AxisKeyBoardValue = new Vector2(horizontal, vertical);

                inputComp.IsSpacePressed = inputComp.Controller.Action.HandBrake.IsPressed();

                inputComp.MouseInput = inputComp.Controller.Action.Mouse.ReadValue<Vector2>();
                inputComp.TouchOrMouseInput = inputComp.MouseInput;
            }
        }
    }
}