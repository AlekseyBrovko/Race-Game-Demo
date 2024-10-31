using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class NpcInObjectPoolSystem : ChangeStateSystem
    {
        private EcsFilterInject<Inc<NpcStartInObjectPoolSystemsEvent>> _startFilter = default;
        private EcsPoolInject<NpcStartInObjectPoolSystemsEvent> _startPool = default;
        private EcsPoolInject<NpcInObjectPoolComp> _objectsPool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;

        private EcsPoolInject<NpcStartIdleSystemsEvent> _startIdlePool = default;
        private EcsPoolInject<NpcStartPatrollSystemsEvent> _startPatrollPool = default;
        private EcsPoolInject<NpcStartChasingSystemsEvent> _startChasingPool = default;
        private EcsPoolInject<NpcStartAimSystemsEvent> _startAimPool = default;
        private EcsPoolInject<NpcStartHurtByCarSystemsEvent> _startHurtPool = default;
        private EcsPoolInject<NpcStartRangeAttackSystemsEvent> _startRangeAttackPool = default;
        private EcsPoolInject<NpcStartMeleeAttackSystemsEvent> _startAttackPool = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.InPool;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.InactiveState;

        public override void Run(EcsSystems systems)
        {
            HandleStart();
        }

        private void HandleStart()
        {
            foreach (var entity in _startFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);
                //ResetTransitions(entity);

                //TODO проверить ещё выключать аниматор
                ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
                navmeshComp.Agent.enabled = false;

                _objectsPool.Value.Add(entity);
                _startPool.Value.Del(entity);
            }
        }

        //не работает
        private void ResetTransitions(int entity)
        {
            if (_startIdlePool.Value.Has(entity))
                _startIdlePool.Value.Del(entity);

            if (_startPatrollPool.Value.Has(entity))
                _startPatrollPool.Value.Del(entity);

            if (_startChasingPool.Value.Has(entity))
                _startChasingPool.Value.Del(entity);

            if (_startAimPool.Value.Has(entity))
                _startAimPool.Value.Del(entity);

            if (_startHurtPool.Value.Has(entity))
                _startHurtPool.Value.Del(entity);

            if (_startRangeAttackPool.Value.Has(entity))
                _startRangeAttackPool.Value.Del(entity);

            if (_startAttackPool.Value.Has(entity))
                _startAttackPool.Value.Del(entity);
        }
    }
}