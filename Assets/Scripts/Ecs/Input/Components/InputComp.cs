using UnityEngine;

namespace Client
{
    struct InputComp
    {
        public ControllerIA Controller;
        public Vector2 AxisJoystickValue;
        public Vector2 AxisKeyBoardValue;
        public bool IsSpacePressed;

        public Vector2 MouseInput;
        public Vector2 TouchInput;
        public Vector2 TouchOrMouseInput;

        public Vector2 LeftDownBoundPoint;
        public Vector2 RightUpBoundPoint;
    }
}