using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class NpcHurtByCarSystem : ChangeStateSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<NpcStartHurtByCarSystemsEvent>> _startFilter = default;
        private EcsPoolInject<NpcStartHurtByCarSystemsEvent> _startPool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;
        private EcsPoolInject<AnimatorComp> _animatorPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;
        private EcsPoolInject<NpcResetPositionBodyEvent> _resetPosPool = default;

        private EcsFilterInject<Inc<NpcHurtByCarComp>, Exc<NpcStartInObjectPoolSystemsEvent, NpcStartDeadSystemsEvent>> _hurtFilter = default;
        private EcsPoolInject<NpcHurtByCarComp> _hurtByCarPool = default;

        private EcsPoolInject<NpcStartChasingSystemsEvent> _startChasingPool = default;

        private EcsFilterInject<Inc<NpcCarHitAnimationStopEvent, NpcHurtByCarComp>> _stopFilter = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.HurtByCar;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.FightState;

        private float _durationOfHurt = 1f;
        private float _distanceOfMove = 2f;

        public override void Run(EcsSystems systems)
        {
            HandleStart();
            HandleHurt();
            HandleStop();
        }

        private void HandleStart()
        {
            foreach (var entity in _startFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);

                ref var startHurtComp = ref _startPool.Value.Get(entity);
                ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
                navmeshComp.Agent.enabled = false;

                ref var npcComp = ref _npcPool.Value.Get(entity);
                npcComp.NpcMb.OnStartHurt();

                ref var hurtComp = ref _hurtByCarPool.Value.Add(entity);
                hurtComp.TargetTransform = npcComp.MainTransform;
                hurtComp.Duration = _durationOfHurt;
                hurtComp.Timer = _durationOfHurt;
                hurtComp.StartPosition = npcComp.MainTransform.position;

                Vector3 directionOfHit = npcComp.MainTransform.position - startHurtComp.HitObjectTransform.position;
                directionOfHit = new Vector3(directionOfHit.x, 0, directionOfHit.y).normalized;
                hurtComp.EndPosition = npcComp.MainTransform.position + directionOfHit * _distanceOfMove;

                ref var animatorComp = ref _animatorPool.Value.Get(entity);
                animatorComp.Animator.SetTrigger("CarHitTrigger");

                _startPool.Value.Del(entity);
            }
        }

        private void HandleHurt()
        {
            foreach (var entity in _hurtFilter.Value)
            {
                ref var npcComp = ref _npcPool.Value.Get(entity);
                ref var animatorComp = ref _animatorPool.Value.Get(entity);

                Transform characterTransform = npcComp.MainTransform;
                Transform animatorTransform = animatorComp.Animator.gameObject.transform;

                ResetPositionOfAnimatedObject(animatorTransform);
                GoBackByCurve(entity);
            }
        }

        private void ResetPositionOfAnimatedObject(Transform animatorTransform)
        {
            animatorTransform.localPosition = Vector3.zero;
            animatorTransform.localEulerAngles = Vector3.zero;
        }

        private void GoBackByCurve(int entity)
        {
            //TODO в идеале нужно из аниматора брать длину анимации
            AnimationCurve animCurve = _state.Value.SettingsConfig.GetCurveByAnimationType(Enums.MovingType.VeryFastOnStart);

            ref var hurtComp = ref _hurtByCarPool.Value.Get(entity);
            hurtComp.Timer -= Time.deltaTime;

            float timeIndex = (hurtComp.Duration - hurtComp.Timer) / hurtComp.Duration;
            float curveIndex = animCurve.Evaluate(timeIndex);
            Vector3 pos = Vector3.Lerp(hurtComp.StartPosition, hurtComp.EndPosition, curveIndex);
            hurtComp.TargetTransform.position = pos;
        }

        private void HandleStop()
        {
            foreach (var entity in _stopFilter.Value)
            {
                ref var npcComp = ref _npcPool.Value.Get(entity);
                npcComp.NpcMb.OnStopHurt();

                _hurtByCarPool.Value.Del(entity);
                
                ref var resetComp = ref _resetPosPool.Value.Add(_world.Value.NewEntity());
                resetComp.NpcEntity = entity;

                _startChasingPool.Value.Add(entity);
            }
        }
    }
}