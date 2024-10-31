using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class NpcLookAroundInPeaceSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<NpcLookAroundComp, NpcInPeaceComp>> _filter = default;

        private EcsPoolInject<NpcLookAroundComp> _lookPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;
        private EcsPoolInject<LayerMaskComp> _layersPool = default;
        private EcsPoolInject<NpcStartChasingSystemsEvent> _startChasingPool = default;

        private float _checkCoolDown = 0.3f;
        List<Transform> _tempTargets = new List<Transform>();

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var lookComp = ref _lookPool.Value.Get(entity);
                lookComp.TimerOfCheck -= Time.deltaTime;

                if (lookComp.TimerOfCheck > 0)
                    continue;

                lookComp.TimerOfCheck = _checkCoolDown * Random.Range(0.9f, 1.1f);
                ref var npcComp = ref _npcPool.Value.Get(entity);
                ref var layersComp = ref _layersPool.Value.Get(_state.Value.LayerMaskEntity);

                lookComp.Targets.Clear();
                lookComp.NearestTarget = null;

                _tempTargets.Clear();

                if (Physics.CheckSphere(npcComp.MainTransform.position, npcComp.RadiusOfLook,
                    npcComp.LayerMaskToSearchEnemies, QueryTriggerInteraction.Ignore))
                {
                    Collider[] colliders = Physics.OverlapSphere(npcComp.MainTransform.position,
                        npcComp.RadiusOfLook, npcComp.LayerMaskToSearchEnemies, QueryTriggerInteraction.Ignore);
                    if (colliders.Length > 0)
                    {
                        foreach (var collider in colliders)
                        {
                            Vector3 directionToEnemy = collider.gameObject.transform.position - npcComp.MainTransform.position;
                            float angle = Vector3.Angle(npcComp.MainTransform.forward, directionToEnemy);

                            IVisible lookTarget = collider.gameObject.GetComponent<IVisible>();

                            if (Tools.IsInVisible(npcComp.MainTransform, lookTarget.LookPoint, layersComp.DefaultLayer))
                            {
                                if (npcComp.SqrRadiusOfFeel > Vector3.SqrMagnitude(directionToEnemy) || angle < npcComp.AngleOfLook)
                                {
                                    _tempTargets.Add(collider.transform);
                                }
                            }
                        }
                    }
                }

                if (_tempTargets.Count > 0)
                {
                    lookComp.Targets = _tempTargets;
                    lookComp.NearestTarget = Tools.GetNearestTransform(npcComp.MainTransform.position, lookComp.Targets);
                    _startChasingPool.Value.Add(entity);
                }
            }
        }
    }
}