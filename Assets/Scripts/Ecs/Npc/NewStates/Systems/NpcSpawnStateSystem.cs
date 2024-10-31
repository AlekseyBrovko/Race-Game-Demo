using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class NpcSpawnStateSystem : ChangeStateSystem
    {
        private EcsWorldInject _world = default;

        private EcsFilterInject<Inc<NpcStartSpawnSystemEvent, NpcMeleeComp>> _startSpawnMeleeFilter = default;
        private EcsFilterInject<Inc<NpcStartSpawnSystemEvent, NpcRangeComp>> _startSpawnRangeFilter = default;

        private EcsPoolInject<NpcStartSpawnSystemEvent> _spawnPool = default;
        private EcsPoolInject<NpcStartIdleSystemsEvent> _startIdlePool = default;

        private EcsPoolInject<RestoreFullHpEvent> _restoreHpPool = default;
        private EcsPoolInject<NpcResetPositionBodyEvent> _resetPositionPool = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.Spawn;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.InactiveState;

        public override void Run(EcsSystems systems)
        {
            HandleMeleeNpc();
            HandleRangeNpc();
        }

        private void HandleMeleeNpc()
        {
            foreach (var entity in _startSpawnMeleeFilter.Value)
                HandleChangeState(entity);
        }

        private void HandleRangeNpc()
        {
            foreach (var entity in _startSpawnRangeFilter.Value)
            {
                HandleChangeState(entity);

            }
        }

        private void HandleChangeState(int entity)
        {
            ResetState(entity);
            ChangeState(entity);

            ref var resetPosComp = ref _resetPositionPool.Value.Add(_world.Value.NewEntity());
            resetPosComp.NpcEntity = entity;

            _restoreHpPool.Value.Add(entity);
            _spawnPool.Value.Del(entity);
            _startIdlePool.Value.Add(entity);
        }
    }
}