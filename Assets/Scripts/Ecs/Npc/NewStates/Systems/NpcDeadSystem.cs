using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class NpcDeadSystem : ChangeStateSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<NpcStartDeadSystemsEvent, NpcMeleeComp>> _startDeadMeleeFilter = default;
        private EcsFilterInject<Inc<NpcStartDeadSystemsEvent, NpcRangeComp>> _startDeadRangeFilter = default;

        private EcsPoolInject<NpcStartDeadSystemsEvent> _startPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;

        private EcsPoolInject<SpawnerSpawnCoolDownComp> _spawnCoolDownPool = default;
        private EcsPoolInject<NpcSpawnerComp> _spawnerPool = default;

        private EcsPoolInject<SilentMoneyEvent> _moneyPool = default;
        private EcsPoolInject<PlayerCarComp> _playerCarPool = default;
        private EcsPoolInject<PhysicalObjectComp> _physicalObjectPool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;

        private EcsFilterInject<Inc<NpcDeadComp>> _deadFilter = default;
        private EcsPoolInject<NpcDeadComp> _deadPool = default;
        private EcsPoolInject<NpcStartInObjectPoolSystemsEvent> _startPoolsObjectPool = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.Dead;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.InactiveState;

        private int _defaultMoneyReward = 10;

        public override void Run(EcsSystems systems)
        {
            HandleMeleeStart();
            HandleRangeStart();
            HandleDead();
        }

        private void HandleMeleeStart()
        {
            foreach (var entity in _startDeadMeleeFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);
                HandleStartDead(entity);

                _deadPool.Value.Add(entity);
                _startPool.Value.Del(entity);
            }
        }

        private void HandleRangeStart()
        {
            foreach (var entity in _startDeadRangeFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);
                HandleStartDead(entity);

                _deadPool.Value.Add(entity);
                _startPool.Value.Del(entity);
            }
        }

        private void HandleDead()
        {
            foreach (var entity in _deadFilter.Value)
            {
                _startPoolsObjectPool.Value.Add(entity);
            }
        }

        private void GetMoney(int value)
        {
            ref var moneyComp = ref _moneyPool.Value.Add(_world.Value.NewEntity());
            moneyComp.MoneyValue = value;
        }

        private void HandleStartDead(int entity)
        {
            ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
            navmeshComp.Agent.enabled = false;

            ref var npcComp = ref _npcPool.Value.Get(entity);
            npcComp.NpcMb.OnDeathEvent();

            IPoolObjectMb poolObjectMb = npcComp.NpcMb as IPoolObjectMb;
            poolObjectMb.ReturnObjectInPool();

            ISpawned spawnedMb = npcComp.NpcMb as ISpawned;

            //TODO дальше здесь рефакторим вероятно стоит перенести в другую систему
            //обрабатываем спаунера
            int spawnerEntity = spawnedMb.SpawnerEntity;
            ref var spawnerCoolDownComp = ref _spawnCoolDownPool.Value.Get(spawnerEntity);
            spawnerCoolDownComp.Timers.Add(0f);

            ref var spawnerComp = ref _spawnerPool.Value.Get(spawnerEntity);
            spawnerComp.SpawnedMbs.Remove(spawnedMb);

            ref var deathEventComp = ref _startPool.Value.Get(entity);
            if (deathEventComp.DeathData != null)
            {
                //Если будут добавляться ещё машины или дружественные npc, это нужно будет рефакторить
                int killerEntity = deathEventComp.DeathData.KillerEntity;
                if (_playerCarPool.Value.Has(killerEntity))
                    GetMoney(_defaultMoneyReward);
                else if (_physicalObjectPool.Value.Has(killerEntity))
                    GetMoney(_defaultMoneyReward);
            }
        }
    }
}