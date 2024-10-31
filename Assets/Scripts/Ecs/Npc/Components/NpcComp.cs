using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    struct NpcComp
    {
        public INpcMb NpcMb;

        public Transform MainTransform;
        public Transform TransformOfBody;
        public LayerMask LayerMaskToSearchEnemies;

        public float RadiusOfLook;
        public float SqrRadiusOfFeel;
        public float AngleOfLook;

        public Collider MainCollider;
        public GameObject GameObject;
        public NavMeshAgent NavMeshAgent;
        public Animator Animator;
    }
}