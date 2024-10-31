using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class RagdollCoolDownSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<RagdollCoolDownComp>> _filter = default;
        private EcsPoolInject<RagdollCoolDownComp> _coolDownPool = default;
        private EcsPoolInject<NpcHideRagdollEvent> _hideRagdollPool = default;

        private float _duration = 10f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var coolDownComp = ref _coolDownPool.Value.Get(entity);
                coolDownComp.Timer += Time.deltaTime;

                if (coolDownComp.Timer >= _duration)
                {
                    _hideRagdollPool.Value.Add(entity);
                    _coolDownPool.Value.Del(entity);
                }
            }
        }
    }
}