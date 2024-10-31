using UnityEngine;

namespace Client
{
    struct LayerMaskComp
    {   
        public LayerMask DefaultLayer;
        public LayerMask EnemyNpcMask;
        public LayerMask FriendlyNpcMask;
        public LayerMask EnemyCarMask;
        public LayerMask FriendlyCarMask;
        public LayerMask EnemyMask;
        public LayerMask FriendlyMask;
        public LayerMask PatrollPointsMask;
    }
}