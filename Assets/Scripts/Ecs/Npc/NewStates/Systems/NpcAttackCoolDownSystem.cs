using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class NpcAttackCoolDownSystem : ChangeStateSystem
    {
        private EcsFilterInject<Inc<NpcStartAttackCoolDownSystemsEvent>> _startFilter = default;
        private EcsPoolInject<NpcStartAttackCoolDownSystemsEvent> _startPool = default;

        private EcsFilterInject<
            Inc<NpcAttackCoolDownComp, NpcMeleeComp>,
            Exc<NpcStartInObjectPoolSystemsEvent, NpcStartDeadSystemsEvent>> _meleeCoolDownFilter = default;

        private EcsFilterInject<
            Inc<NpcAttackCoolDownComp, NpcRangeComp>,
            Exc<NpcStartInObjectPoolSystemsEvent, NpcStartDeadSystemsEvent>> _rangeCoolDownFilter = default;

        private EcsPoolInject<NpcAttackCoolDownComp> _coolDownPool = default;
        private EcsPoolInject<NpcStartChasingSystemsEvent> _startChasingPool = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.AttackCoolDown;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.FightState;

        private float _meleeCoolDown = 0.2f;
        private float _rangeCoolDown = 2f;

        public override void Run(EcsSystems systems)
        {
            HandleStart();

            HandleMeleeCoolDown();
            HandleRangeCoolDown();
        }

        private void HandleStart()
        {
            foreach (var entity in _startFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);

                _coolDownPool.Value.Add(entity);
                _startPool.Value.Del(entity);
            }
        }

        private void HandleMeleeCoolDown()
        {
            foreach (var entity in _meleeCoolDownFilter.Value)
            {
                ref var coolDownComp = ref _coolDownPool.Value.Get(entity);
                coolDownComp.Timer += Time.deltaTime;
                if (coolDownComp.Timer > _meleeCoolDown)
                    _startChasingPool.Value.Add(entity);
            }
        }

        private void HandleRangeCoolDown()
        {
            foreach (var entity in _rangeCoolDownFilter.Value)
            {
                ref var coolDownComp = ref _coolDownPool.Value.Get(entity);
                coolDownComp.Timer += Time.deltaTime;
                if (coolDownComp.Timer > _rangeCoolDown)
                    _startChasingPool.Value.Add(entity);
            }
        }
    }
}