using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class SimpleMoveTransformSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<TransformMoveComp>> _linearFilter = default;
        private EcsPoolInject<TransformMoveComp> _linearMovePool = default;

        public void Run(EcsSystems systems)
        {
            HandleCurveMove();
        }

        private void HandleCurveMove()
        {
            foreach(var entity in _linearFilter.Value)
            {
                ref var moveComp = ref _linearMovePool.Value.Get(entity);
                AnimationCurve animCurve = _state.Value.SettingsConfig.GetCurveByAnimationType(moveComp.MovingType);

                moveComp.Timer -= Time.deltaTime;
                float timeIndex = (moveComp.Duration - moveComp.Timer) / moveComp.Duration;
                float curveIndex = animCurve.Evaluate(timeIndex);
                Vector3 pos = Vector3.Lerp(moveComp.StartPosition, moveComp.FinishPosition, curveIndex);

                if (moveComp.IsLocal)
                    moveComp.Transform.localPosition = pos;
                else
                    moveComp.Transform.position = pos;

                if (moveComp.Timer < 0)
                {
                    if (moveComp.IsLocal)
                        moveComp.Transform.localPosition = moveComp.FinishPosition;
                    else
                        moveComp.Transform.position = moveComp.FinishPosition;

                    moveComp.Callback?.Invoke();
                    _linearMovePool.Value.Del(entity);
                }
            }
        }
    }
}