using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public class NpcShowRagdollSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<NpcShowRagdollEvent>> _filter = default;

        private EcsPoolInject<NpcShowRagdollEvent> _ragdollEventPool = default;
        private EcsPoolInject<ObjectsPoolComp> _objectsPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;
        private EcsPoolInject<RagdollCoolDownComp> _ragdollCoolDownPool = default;
        private EcsPoolInject<Sound3DEvent> _npcDeadSoundEvent = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var npcComp = ref _npcPool.Value.Get(entity);
                ref var poolsComp = ref _objectsPool.Value.Get(_state.Value.ObjectsPoolEntity);

                GameObject ragdollGo = poolsComp.EnemyRagdollsDictionary[npcComp.NpcMb.RagdollId].GetFromPool();
                RagdollMb ragdollMb = ragdollGo.GetComponent<RagdollMb>();

                ragdollMb.OnSpawn();
                ragdollMb.SetTransforms(npcComp.NpcMb.RagdollHelper);
                npcComp.MainCollider.enabled = false;
                _ragdollCoolDownPool.Value.Add(ragdollMb.Entity);

                PlayDeadSound(ragdollMb);
                PlayHitSound(ragdollMb);

                _ragdollEventPool.Value.Del(entity);
            }
        }

        private void PlayDeadSound(RagdollMb ragdollMb)
        {
            ref var deadSoundComp = ref _npcDeadSoundEvent.Value.Add(_world.Value.NewEntity());
            deadSoundComp.SoundType = Enums.SoundEnum.ZombieDead;
            deadSoundComp.MainRigidbody = ragdollMb.MainRigidbody;
            deadSoundComp.Transform = ragdollMb.transform;
        }

        private void PlayHitSound(RagdollMb ragdollMb)
        {
            ref var deadSoundComp = ref _npcDeadSoundEvent.Value.Add(_world.Value.NewEntity());
            deadSoundComp.SoundType = Enums.SoundEnum.CarHitZombie;
            deadSoundComp.Position = ragdollMb.transform.position;
        }
    }
}