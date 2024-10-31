using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class NpcAimSystem : ChangeStateSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<NpcStartAimSystemsEvent>> _startFilter = default;
        private EcsPoolInject<NpcStartAimSystemsEvent> _startAimPool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;

        private EcsFilterInject<Inc<NpcAimComp, NpcRangeComp>, Exc<NpcStartInObjectPoolSystemsEvent, NpcStartDeadSystemsEvent>> _rangeAimFilter = default;
        private EcsPoolInject<NpcAimComp> _aimPool = default;
        private EcsPoolInject<NpcFightComp> _fightPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        private EcsPoolInject<NpcStartRangeAttackSystemsEvent> _startRangeAttackPool = default;
        private EcsPoolInject<NpcStartChasingSystemsEvent> _startChasingPool = default;
        private EcsPoolInject<LayerMaskComp> _layersPool = default;
        private EcsPoolInject<NpcRangeComp> _rangePool = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.Aim;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.FightState;

        private float _aimDuration = 1f;
        private float _speedOfRotation = 5f;

        public override void Run(EcsSystems systems)
        {
            HandleStart();
            HandleRangeAim();
        }

        private void HandleStart()
        {
            foreach (var entity in _startFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);

                if (CheckOnInObjectsPoolTransition(entity))
                {
                    _startAimPool.Value.Del(entity);
                    continue;
                }

                ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
                navmeshComp.Agent.enabled = false;

                ref var aimComp = ref _aimPool.Value.Add(entity);
                aimComp.AimTimer = _aimDuration;

                _startAimPool.Value.Del(entity);
            }
        }

        private void HandleRangeAim()
        {
            foreach (var entity in _rangeAimFilter.Value)
            {
                ref var aimComp = ref _aimPool.Value.Get(entity);
                ref var fightComp = ref _fightPool.Value.Get(entity);
                ref var viewComp = ref _viewPool.Value.Get(entity);
                ref var npcRangeComp = ref _rangePool.Value.Get(entity);

                Vector3 direction = fightComp.Target.position - viewComp.Transform.position;
                direction = new Vector3(direction.x, 0, direction.z);
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                viewComp.Transform.rotation = 
                    Quaternion.Lerp(viewComp.Transform.rotation, 
                    lookRotation, _speedOfRotation * Time.deltaTime);

                aimComp.AimTimer -= Time.deltaTime;
                if (aimComp.AimTimer <= 0)
                {
                    //проверить на дальность и видимость
                    //нужно ещё проверить, есть в руках предмет
                    ref var layersComp = ref _layersPool.Value.Get(_state.Value.LayerMaskEntity);
                    ref var rangeComp = ref _rangePool.Value.Get(entity);
                    float distance = direction.sqrMagnitude;

                    bool throwDistance = distance < rangeComp.RangeMb.RangeOfAttack * rangeComp.RangeMb.RangeOfAttack;
                    bool isVisible = Tools.IsInVisible(viewComp.Transform, fightComp.Target, layersComp.DefaultLayer);

                    if (!throwDistance || !isVisible)
                    {
                        _aimPool.Value.Del(entity);
                        _startChasingPool.Value.Add(entity);
                    }
                    else if (npcRangeComp.RangeMb is IRangeThrowingNpc)
                    {
                        IRangeThrowingNpc throwNpcMb = npcRangeComp.RangeMb as IRangeThrowingNpc;
                        if (throwDistance && isVisible && throwNpcMb.CanThrowObject)
                        {
                            _aimPool.Value.Del(entity);
                            _startRangeAttackPool.Value.Add(entity);
                        }
                        else if (!throwNpcMb.CanThrowObject)
                        {
                            //по факту нужно переходить в подготовку объекта
                            continue;
                        }
                    }
                    else
                    {
                        _aimPool.Value.Del(entity);
                        _startRangeAttackPool.Value.Add(entity);
                    }
                }
            }
        }
    }
}