using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Client
{
    sealed class InputInit : IEcsInitSystem, IEcsDestroySystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<InputComp> _inputPool = default;
        private EcsPoolInject<CarFlipEvent> _flipPool = default;
        private EcsPoolInject<EscButtonEvent> _escButtonPool = default;
        private ControllerIA _controller;

        private float _upAndDownDeadTouchZone = 0.3f;
        private float _sideDeadTouchZone = 0.3f;

        public void Init(EcsSystems systems)
        {
            if (_state.Value.IsMobilePlatform)
                EnhancedTouchSupport.Enable();

            int newInputEntity = _world.Value.NewEntity();
            _state.Value.InputEntity = newInputEntity;

            ref var newInputComp = ref _inputPool.Value.Add(newInputEntity);
            _controller = new ControllerIA();
            _controller.Enable();
            newInputComp.Controller = _controller;

            _controller.Action.Esc.performed += EscButtonPerformed;
            _controller.Action.Flip.performed += FlipButtonPerformed;

            float screenHeight = Screen.height;
            float screenWidth = Screen.width;

            Debug.Log($"InputInit; screenHeight = {screenHeight}; screenWidth = {screenWidth};");

            float downDeadPoint = screenHeight * _upAndDownDeadTouchZone;
            float upDeadPoint = screenHeight - downDeadPoint;
            float leftDeadPoint = screenWidth * _sideDeadTouchZone;
            float rightDeadPoint = screenWidth - leftDeadPoint;

            newInputComp.LeftDownBoundPoint = new Vector2(leftDeadPoint, downDeadPoint);
            newInputComp.RightUpBoundPoint = new Vector2(rightDeadPoint, upDeadPoint);
        }

        private void EscButtonPerformed(InputAction.CallbackContext context) =>
            _escButtonPool.Value.Add(_world.Value.NewEntity());

        private void FlipButtonPerformed(InputAction.CallbackContext context)
        {
            if (_state.Value.PlaySystems)
                _flipPool.Value.Add(_state.Value.PlayerCarEntity);
        }

        public void Destroy(EcsSystems systems)
        {
            _controller.Action.Esc.performed -= EscButtonPerformed;
            _controller.Action.Flip.performed -= FlipButtonPerformed;
        }
    }
}