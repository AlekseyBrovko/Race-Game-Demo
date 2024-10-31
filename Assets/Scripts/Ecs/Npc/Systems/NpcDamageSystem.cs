using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class NpcDamageSystem : NpcChangeStateSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<NpcDamageEvent>> _damageFilter = default;

        private EcsPoolInject<NpcDamageEvent> _damagePool = default;
        private EcsPoolInject<HpComp> _hpPool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;
        private EcsPoolInject<NpcStartDeadSystemsEvent> _startDeadPool = default;
        private EcsPoolInject<NpcShowRagdollEvent> _npcRagdollPool = default;

        private EcsPoolInject<KillZombieEvent> _killEvent = default;
        private EcsPoolInject<NpcComp> _npcPool = default;

        public override void Run(EcsSystems systems)
        {
            HandlDamageSystem();
        }

        private void HandlDamageSystem()
        {
            foreach (var entity in _damageFilter.Value)
            {
                ref var damageComp = ref _damagePool.Value.Get(entity);
                int damagedEntity = damageComp.DamagedEntity;

                if (!_hpPool.Value.Has(damagedEntity))
                {
                    //костыль, была бага
                    _damagePool.Value.Del(entity);
                    continue;
                }

                ref var hpComp = ref _hpPool.Value.Get(damagedEntity);
                hpComp.HpValue -= damageComp.DamageValue;

                if (hpComp.HpValue <= 0)
                {
                    hpComp.HpValue = 0;

                    if (!_npcRagdollPool.Value.Has(damagedEntity))
                    {
                        _npcRagdollPool.Value.Add(damagedEntity);

                        ref var navmeshComp = ref _navmeshPool.Value.Get(damagedEntity);
                        navmeshComp.Agent.enabled = false;

                        DeathData deathData = new DeathData(damageComp.DamagerEntity, damageComp.DamageType);
                        switch (damageComp.DamageType)
                        {
                            case Enums.DamageType.Explosion:
                                //TODO к регдолу силу бахнуть
                                break;

                            default:
                                ref var npcComp = ref _npcPool.Value.Get(damagedEntity);
                                ref var killEventComp = ref _killEvent.Value.Add(_world.Value.NewEntity());
                                killEventComp.ZombieName = npcComp.NpcMb.Id;
                                killEventComp.ZoneName = npcComp.NpcMb.ZoneName;
                                break;
                        }

                        ref var deadEventComp = ref _startDeadPool.Value.Add(damagedEntity);
                        deadEventComp.DeathData = deathData;
                    }   
                }

                _damagePool.Value.Del(entity);
            }
        }
    }
}