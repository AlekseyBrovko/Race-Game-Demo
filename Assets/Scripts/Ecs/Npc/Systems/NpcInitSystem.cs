using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    sealed class NpcInitSystem : IEcsRunSystem
    {
        public NpcInitSystem(bool isTestScene)
        {
            _isTestScene = isTestScene;
        }

        private bool _isTestScene;

        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<NpcInitEvent>> _filter = default;

        private EcsPoolInject<NpcInitEvent> _initPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<HpComp> _hpPool = default;
        private EcsPoolInject<NpcInitFsmEvent> _fsmPool = default;

        private EcsPoolInject<EnemyComp> _enemyPool = default;
        private EcsPoolInject<FriendlyComp> _friendlyPool = default;

        private EcsPoolInject<NpcLookAroundComp> _lookPool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;
        private EcsPoolInject<AnimatorComp> _animatorPool = default;
        private EcsPoolInject<NpcFightComp> _fightPool = default;
        private EcsPoolInject<NpcCheckObstacleComp> _checkObstaclePool = default;
        private EcsPoolInject<NpcDestinationComp> _destinationPool = default;

        private EcsPoolInject<NpcRangeComp> _npcRangePool = default;
        private EcsPoolInject<NpcMeleeComp> _npcMeleePool = default;
        private EcsPoolInject<NpcStateComp> _statePool = default;

        private EcsPoolInject<NpcDebugCanvasComp> _debugCanvasPool = default;
        private EcsPoolInject<ObjectsPoolComp> _objectsPool = default;
        private EcsPoolInject<LayerMaskComp> _layersPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var initComp = ref _initPool.Value.Get(entity);

                ref var layersComp = ref _layersPool.Value.Get(_state.Value.LayerMaskEntity);
                var settingsConfig = _state.Value.SettingsConfig;

                ref var viewComp = ref _viewPool.Value.Add(entity);
                viewComp.Transform = initComp.NpcGo.transform;

                INpcMb npcMb = initComp.NpcMb;
                npcMb.SetRunSpeed();

                ref var npcComp = ref _npcPool.Value.Add(entity);
                npcComp.NpcMb = npcMb;
                npcComp.MainTransform = initComp.NpcGo.transform;
                npcComp.TransformOfBody = npcMb.Animator.transform;
                npcComp.MainCollider = npcMb.MainCollider;
                npcComp.GameObject = npcMb.GameObject;
                npcComp.NavMeshAgent = npcMb.Agent;
                npcComp.Animator = npcMb.Animator;

                ref var hpComp = ref _hpPool.Value.Add(entity);
                hpComp.HpValue = npcMb.Hp;
                hpComp.FullHpValue = npcMb.Hp;

                if (npcMb is IEnemy)
                {
                    ref var enemyComp = ref _enemyPool.Value.Add(entity);
                    enemyComp.EnemyMb = npcMb as IEnemy;
                    npcComp.LayerMaskToSearchEnemies = layersComp.FriendlyMask;
                }
                else if (npcMb is IFriendly)
                {
                    _friendlyPool.Value.Add(entity);
                    npcComp.LayerMaskToSearchEnemies = layersComp.EnemyMask;
                }

                ref var lookComp = ref _lookPool.Value.Add(entity);
                lookComp.TimerOfCheck = 0.1f;
                lookComp.Targets = new List<Transform>();

                ref var navmeshComp = ref _navmeshPool.Value.Add(entity);
                navmeshComp.Agent = npcMb.Agent;
                navmeshComp.Agent.avoidancePriority = Random.Range(0, 200);

                ref var animatorComp = ref _animatorPool.Value.Add(entity);
                animatorComp.Animator = npcMb.Animator;

                if (npcMb is IMeleeNpc)
                {
                    InitMeleeComp(entity);
                    npcComp.RadiusOfLook = settingsConfig.RadiusOfMeleeLook;
                    npcComp.SqrRadiusOfFeel = settingsConfig.SqrRadiusOfMeleeFeel;
                    npcComp.AngleOfLook = settingsConfig.AngleOfMeleeLook;
                }   
                else if (npcMb is IRangeNpc)
                {
                    InitRangeComp(entity, initComp.NpcGo);
                    npcComp.RadiusOfLook = settingsConfig.RadiusOfRangeLook;
                    npcComp.SqrRadiusOfFeel = settingsConfig.SqrRadiusOfRangeFeel;
                    npcComp.AngleOfLook = settingsConfig.AngleOfRangeLook;
                }   
                else           //by default
                {
                    InitMeleeComp(entity);
                    npcComp.RadiusOfLook = settingsConfig.RadiusOfMeleeLook;
                    npcComp.SqrRadiusOfFeel = settingsConfig.SqrRadiusOfMeleeFeel;
                    npcComp.AngleOfLook = settingsConfig.AngleOfMeleeLook;
                }
                
                _checkObstaclePool.Value.Add(entity);
                _fightPool.Value.Add(entity);
                _fsmPool.Value.Add(entity);
                _destinationPool.Value.Add(entity);

                ref var stateComp = ref _statePool.Value.Add(entity);
                stateComp.States = new List<string>();

                _initPool.Value.Del(entity);

                if (_isTestScene)
                    InitDebugCanvas(entity, viewComp.Transform);
            }
        }

        private void InitRangeComp(int entity, GameObject npcGo)
        {
            ref var rangeComp = ref _npcRangePool.Value.Add(entity);
            rangeComp.RangeMb = npcGo.GetComponent<IRangeNpc>();
            if (rangeComp.RangeMb is IRangeThrowingNpc)
            {
                IRangeThrowingNpc throwingNpc = (IRangeThrowingNpc)rangeComp.RangeMb;
                ref var poolComp = ref _objectsPool.Value.Get(_state.Value.ObjectsPoolEntity);
                throwingNpc.PoolOfThrowObjects = poolComp.ThrowingObjectsDictionary[throwingNpc.ThrowObjectId];
            }
        }

        private void InitMeleeComp(int entity)
        {
            ref var meleeComp = ref _npcMeleePool.Value.Add(entity);
        }

        private void InitDebugCanvas(int entity, Transform npcTransform)
        {
            Vector3 localPos = new Vector3(0f, 4.4f, 0f);

            GameObject canvasGo = GameObject.Instantiate(_state.Value.EnemyPrefabsConfig.NpcDebugCanvas);
            canvasGo.transform.SetParent(npcTransform);
            canvasGo.transform.localPosition = localPos;

            NpcDebugCanvasMb canvasMb = canvasGo.GetComponent<NpcDebugCanvasMb>();
            ref var debugCanvasComp = ref _debugCanvasPool.Value.Add(entity);
            debugCanvasComp.CanvasMb = canvasMb;
        }
    }
}