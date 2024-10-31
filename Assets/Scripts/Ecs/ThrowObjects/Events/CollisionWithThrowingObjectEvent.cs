using UnityEngine;

namespace Client
{
    public struct CollisionWithThrowingObjectEvent
    {
        public INpcMb NpcOwner;
        public IThrowObject ThrowObject;
    }
}