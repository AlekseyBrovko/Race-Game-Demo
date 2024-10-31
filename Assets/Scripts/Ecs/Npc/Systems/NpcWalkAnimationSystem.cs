using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class NpcWalkAnimationSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<NpcComp, AnimatorComp, NpcMeleeComp>> _meleeFilter = default;
        private EcsFilterInject<Inc<NpcComp, AnimatorComp, NpcRangeComp>> _rangeFilter = default;

        private EcsPoolInject<NavmeshComp> _navmeshPool = default;
        private EcsPoolInject<AnimatorComp> _animatorPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;
        private EcsPoolInject<NpcRangeComp> _rangePool = default;

        private int _animatorWalkSpeed = Animator.StringToHash("WalkSpeed");
        private float _smoothTime = 0.2f;

        public void Run(EcsSystems systems)
        {
            HandleMeleeNpc();
            HandleRangeNpc();
        }

        private void HandleMeleeNpc()
        {
            foreach (var entity in _meleeFilter.Value)
            {
                ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
                ref var animatorComp = ref _animatorPool.Value.Get(entity);
                ref var npcComp = ref _npcPool.Value.Get(entity);

                float velocity = navmeshComp.Agent.velocity.magnitude / npcComp.NpcMb.RunSpeed;

                float smoothVelocity = Mathf.SmoothDamp(animatorComp.WalkValue, 
                    velocity, ref animatorComp.SmoothVelocity, _smoothTime);
                animatorComp.Animator.SetFloat(_animatorWalkSpeed, smoothVelocity);
                animatorComp.WalkValue = smoothVelocity;
            }
        }

        private void HandleRangeNpc()
        {
            foreach (var entity in _rangeFilter.Value)
            {
                ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
                ref var animatorComp = ref _animatorPool.Value.Get(entity);
                ref var npcComp = ref _npcPool.Value.Get(entity);
                ref var rangeComp = ref _rangePool.Value.Get(entity);

                float velocity = navmeshComp.Agent.velocity.magnitude / rangeComp.RangeMb.DefaultRunSpeed;

                float smoothVelocity = Mathf.SmoothDamp(animatorComp.WalkValue, 
                    velocity, ref animatorComp.SmoothVelocity, _smoothTime);
                animatorComp.Animator.SetFloat(_animatorWalkSpeed, smoothVelocity);
                animatorComp.WalkValue = smoothVelocity;
            }
        }
    }
}