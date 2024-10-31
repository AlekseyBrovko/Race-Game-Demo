using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class NpcIdleSystem : ChangeStateSystem
    {
        private EcsWorldInject _world = default;

        private EcsFilterInject<Inc<NpcStartIdleSystemsEvent>> _startFilter = default;
        private EcsPoolInject<NpcStartIdleSystemsEvent> _startIdlePool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<NpcResetPositionBodyEvent> _resetLocalPosPool = default;

        private EcsFilterInject<Inc<NpcIdleComp>, 
            Exc<NpcStartInObjectPoolSystemsEvent, NpcStartDeadSystemsEvent>> _idleFilter = default;

        private EcsPoolInject<NpcIdleComp> _idlePool = default;
        private EcsPoolInject<NpcStartPatrollSystemsEvent> _startPatrollPool = default;
        private EcsPoolInject<NpcPatrollPointsComp> _patrollPointsPool = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.Idle;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.PeaceState;

        //TODO вынести отдельно
        private float _idleDuration = 1.5f;

        public override void Run(EcsSystems systems)
        {
            HandleStartIdle();
            HandleIdlesNpc();
        }

        private void HandleStartIdle()
        {
            foreach (var entity in _startFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);
                
                if (CheckOnInObjectsPoolTransition(entity))
                {
                    _startIdlePool.Value.Del(entity);
                    continue;
                }

                ref var idleComp = ref _idlePool.Value.Add(entity);
                idleComp.IdleTimer = _idleDuration * Random.Range(0.9f, 1.1f);

                ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
                ref var viewComp = ref _viewPool.Value.Get(entity);
                navmeshComp.Agent.enabled = true;

                //TODO ошибка связана со спаунов навмеша, нужно точку навмеша выбирать пустую для спавна
                if (navmeshComp.Agent.isOnNavMesh)
                {
                    navmeshComp.Agent.SetDestination(viewComp.Transform.position);
                }   
                else
                {
                    Debug.LogWarning($"!navmeshComp.Agent.isOnNavMesh name = {viewComp.Transform.name}; " +
                        $"position = {viewComp.Transform.position};");
                }   

                ref var resetPosComp = ref _resetLocalPosPool.Value.Add(_world.Value.NewEntity());
                resetPosComp.NpcEntity = entity;

                _startIdlePool.Value.Del(entity);
            }
        }

        private void HandleIdlesNpc()
        {
            foreach (var entity in _idleFilter.Value)
            {
                ref var idleComp = ref _idlePool.Value.Get(entity);
                idleComp.IdleTimer -= Time.deltaTime;
                if (idleComp.IdleTimer <= 0)
                {
                    if (!_patrollPointsPool.Value.Has(entity))
                        continue;

                    ref var patrollComp = ref _patrollPointsPool.Value.Get(entity);
                    
                    if (patrollComp.PatrollPoints.Count == 0)
                    {
                        idleComp.IdleTimer = _idleDuration;
                    }   
                    else
                    {
                        _startPatrollPool.Value.Add(entity);
                        _idlePool.Value.Del(entity);
                    }   
                }
            }
        }
    }
}