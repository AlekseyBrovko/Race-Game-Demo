using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Client
{
    sealed class TouchInputSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<InputComp, TouchComp>> _touchFilter = default;

        private EcsPoolInject<InputComp> _inputPool = default;
        private EcsPoolInject<TouchComp> _touchPool = default;

        public void Run(EcsSystems systems)
        {
            HandleStartAndEndOfTouch();
            HandleTouch();
        }

        private void HandleTouch()
        {
            foreach (var entity in _touchFilter.Value)
            {
                ref var inputComp = ref _inputPool.Value.Get(entity);

                if (Touch.activeTouches.Count > 0)
                {
                    var delta = Touch.activeTouches[0].delta;
                    inputComp.TouchInput = delta;
                    inputComp.TouchOrMouseInput = inputComp.TouchInput;
                }
            }
        }

        private void HandleStartAndEndOfTouch()
        {
            if (Touch.activeFingers.Count > 0 && Touch.activeFingers[0].currentTouch.phase 
                == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (TouchIsNotInDeadZone(Touch.activeFingers[0].currentTouch))
                {
                    if (!_touchPool.Value.Has(_state.Value.InputEntity))
                        _touchPool.Value.Add(_state.Value.InputEntity);
                }
            }

            if (Touch.activeFingers.Count == 0 && _touchPool.Value.Has(_state.Value.InputEntity))
                _touchPool.Value.Del(_state.Value.InputEntity);
        }

        private bool TouchIsNotInDeadZone(Touch touch)
        {
            ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);
            Vector2 touchPos = Touch.activeTouches[0].screenPosition;

            if (touchPos.x < inputComp.LeftDownBoundPoint.x || touchPos.x > inputComp.RightUpBoundPoint.x)
                return false;

            if (touchPos.y < inputComp.LeftDownBoundPoint.y || touchPos.y > inputComp.RightUpBoundPoint.y)
                return false;

            return true;
        }
    }
}