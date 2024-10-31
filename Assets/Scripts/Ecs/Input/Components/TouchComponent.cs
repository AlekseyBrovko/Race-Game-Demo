using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using InputTouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Client {
    struct TouchComponent {
        public Vector2 Direction, Position, InitialPosition;
        public TouchPhase Phase;
    }
}