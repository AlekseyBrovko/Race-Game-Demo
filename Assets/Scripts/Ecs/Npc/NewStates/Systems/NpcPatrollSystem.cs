using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class NpcPatrollSystem : ChangeStateSystem
    {
        private EcsWorldInject _world = default;

        private EcsFilterInject<Inc<NpcStartPatrollSystemsEvent>> _startFilter = default;
        private EcsPoolInject<NpcStartPatrollSystemsEvent> _startPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<NpcStartIdleSystemsEvent> _startIdlePool = default;
        private EcsPoolInject<NpcDestinationComp> _destinationPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;
        private EcsPoolInject<NpcResetPositionBodyEvent> _resetPosisionPool = default;
        private EcsPoolInject<NpcInitPatrollPointsEvent> _initPatrollPointsPool = default;

        private EcsFilterInject<Inc<NpcPatrollComp>, 
            Exc<NpcStartInObjectPoolSystemsEvent, NpcStartDeadSystemsEvent>> _patrollFilter = default;

        private EcsPoolInject<NpcPatrollComp> _patrollPool = default;
        private EcsPoolInject<NpcPatrollPointsComp> _patrollPointsPool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.Patroll;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.PeaceState;

        private List<Transform> _tempTransforms = new List<Transform>();

        private float _durationToCheckDistance = 0.5f;
        private float _sqrDestinationThreshold = 4f;

        public override void Run(EcsSystems systems)
        {
            HandleStart();
            HandlePatroll();
        }

        private void HandleStart()
        {
            foreach (var entity in _startFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);

                if (CheckOnInObjectsPoolTransition(entity))
                {   
                    _startPool.Value.Del(entity);
                    continue;
                }

                _tempTransforms.Clear();

                //TODO отсюда ошибка прилетела
                if (!_patrollPointsPool.Value.Has(entity))
                {
                    if (!_initPatrollPointsPool.Value.Has(entity))
                        _initPatrollPointsPool.Value.Add(entity);

                    continue;
                }

                ref var patrollPointsComp = ref _patrollPointsPool.Value.Get(entity);
                ref var destinationComp = ref _destinationPool.Value.Get(entity);
                ref var patrollComp = ref _patrollPool.Value.Add(entity);
                ref var npcComp = ref _npcPool.Value.Get(entity);
                ref var navMeshComp = ref _navmeshPool.Value.Get(entity);
                navMeshComp.Agent.enabled = true;
                navMeshComp.Agent.speed = npcComp.NpcMb.WalkSpeed;

                //выбераем патрульную точку
                if (patrollPointsComp.PatrollPoints.Count == 0)
                {   
                    _patrollPool.Value.Del(entity);
                    _startIdlePool.Value.Add(entity);
                    continue;
                }
                else if (patrollPointsComp.PatrollPoints.Count == 1)
                {
                    destinationComp.DestinationTransform = patrollPointsComp.PatrollPoints[0];
                }
                else if (patrollPointsComp.PatrollPoints.Count == 2)
                {   
                    if (patrollPointsComp.CurrentPoint != null)
                    {
                        _tempTransforms.AddRange(patrollPointsComp.PatrollPoints);
                        patrollPointsComp.PreviousPoint = patrollPointsComp.CurrentPoint;
                        _tempTransforms.Remove(patrollPointsComp.CurrentPoint);
                        patrollPointsComp.CurrentPoint = _tempTransforms[0];
                        destinationComp.DestinationTransform = _tempTransforms[0];
                    }
                    else
                    {
                        patrollPointsComp.CurrentPoint = patrollPointsComp.PatrollPoints[
                            Random.Range(0, patrollPointsComp.PatrollPoints.Count)];
                        destinationComp.DestinationTransform = patrollPointsComp.CurrentPoint;
                    }
                }
                else
                {
                    _tempTransforms.AddRange(patrollPointsComp.PatrollPoints);

                    if (patrollPointsComp.PreviousPoint != null)
                    {
                        _tempTransforms.Remove(patrollPointsComp.PreviousPoint);
                    }

                    if (patrollPointsComp.CurrentPoint != null)
                    {
                        _tempTransforms.Remove(patrollPointsComp.CurrentPoint);
                        patrollPointsComp.PreviousPoint = patrollPointsComp.CurrentPoint;
                    }

                    patrollPointsComp.CurrentPoint = _tempTransforms[Random.Range(0, _tempTransforms.Count)];
                    destinationComp.DestinationTransform = patrollPointsComp.CurrentPoint;
                }

                ref var resetComp = ref _resetPosisionPool.Value.Add(_world.Value.NewEntity());
                resetComp.NpcEntity = entity;

                _startPool.Value.Del(entity);
            }
        }

        private void HandlePatroll()
        {
            foreach (var entity in _patrollFilter.Value)
            {
                ref var patrollComp = ref _patrollPool.Value.Get(entity);
                patrollComp.TimerToCheckDistance -= Time.deltaTime;
                if (patrollComp.TimerToCheckDistance > 0)
                    continue;

                patrollComp.TimerToCheckDistance = _durationToCheckDistance * Random.Range(0.9f, 1.1f);

                ref var viewComp = ref _viewPool.Value.Get(entity);
                ref var destinationComp = ref _destinationPool.Value.Get(entity);
                ref var navmeshComp = ref _navmeshPool.Value.Get(entity);

                float sqrDistance = Vector3.SqrMagnitude(destinationComp.DestinationTransform.position - viewComp.Transform.position);

                if (sqrDistance < _sqrDestinationThreshold)
                {
                    if (navmeshComp.Agent.isOnNavMesh)
                        navmeshComp.Agent.SetDestination(viewComp.Transform.position);
                    else
                        Debug.LogWarning($"!navmeshComp.Agent.isOnNavMesh; name = {viewComp.Transform.name};" +
                            $"position = {viewComp.Transform.position};");
                    _patrollPool.Value.Del(entity);
                    _startIdlePool.Value.Add(entity);
                }
                else
                {
                    if (navmeshComp.Agent.isOnNavMesh)
                        navmeshComp.Agent.SetDestination(destinationComp.DestinationTransform.position);
                    else
                    {
                        Debug.LogWarning($"!navmeshComp.Agent.isOnNavMesh; name = {viewComp.Transform.name};" +
                            $"position = {viewComp.Transform.position};");
                    }   
                }
            }
        }
    }
}