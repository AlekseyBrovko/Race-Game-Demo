using System;
using UnityEngine;

namespace Client
{
    struct TransformMoveComp
    {
        public Enums.MovingType MovingType;
        public bool IsLocal;
        public Transform Transform;
        public Vector3 StartPosition;
        public Vector3 FinishPosition;
        public float Timer;
        public float Duration;
        public Action Callback;
    }
}