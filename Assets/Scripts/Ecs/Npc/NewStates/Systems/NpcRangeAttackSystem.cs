using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class NpcRangeAttackSystem : ChangeStateSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<NpcStartRangeAttackSystemsEvent>> _startFilter = default;
        private EcsPoolInject<NpcStartRangeAttackSystemsEvent> _startPool = default;

        private EcsFilterInject<Inc<NpcRangeAttackComp>> _rangeAttackFilter = default;
        private EcsPoolInject<NpcRangeAttackComp> _rangeAttackPool = default;
        private EcsPoolInject<NpcRangeComp> _npcRangePool = default;
        private EcsPoolInject<ThrowObjectComp> _throwObjectPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<NpcFightComp> _fightPool = default;
        private EcsPoolInject<NpcStartAttackCoolDownSystemsEvent> _coolDownPool = default;
        private EcsPoolInject<Sound3DEvent> _soundPool = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.RangeAttack;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.FightState;

        private float _maxThrowForce = 200f;

        public override void Run(EcsSystems systems)
        {
            HandleStart();
            HandleAttack();
        }

        private void HandleStart()
        {
            foreach (var entity in _startFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);

                _rangeAttackPool.Value.Add(entity);
                _startPool.Value.Del(entity);
            }
        }

        private void HandleAttack()
        {
            foreach (var entity in _rangeAttackFilter.Value)
            {
                ref var rangeComp = ref _npcRangePool.Value.Get(entity);
                if (rangeComp.RangeMb is IRangeThrowingNpc)
                    HandleThrowingAttack(entity, ref rangeComp);
            }
        }

        private void HandleThrowingAttack(int entity, ref NpcRangeComp rangeComp)
        {
            IRangeThrowingNpc rangeThrowMb = rangeComp.RangeMb as IRangeThrowingNpc;
            IThrowObject throwObject = rangeThrowMb.ThrowObjectInHands;;

            rangeThrowMb.OnRangeAttack();
            throwObject.OnThrowObject();

            ref var viewComp = ref _viewPool.Value.Get(entity);
            ref var fightComp = ref _fightPool.Value.Get(entity);
            ref var throwObjectComp = ref _throwObjectPool.Value.Get(throwObject.Entity);
            
            var targetRb = fightComp.Target.gameObject.GetComponent<Rigidbody>();
            Vector3 targetVelocity = targetRb.velocity;

            ThrowData data = CalculateThrowData(
                fightComp.LookPointOfTarget.position, throwObject.Transform.position);

            data = CalculatePredictedThrowData(data, targetVelocity, fightComp.LookPointOfTarget.position);
            throwObject.Rigidbody.velocity = data.ThrowVelocity;

            ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
            soundComp.Position = viewComp.Transform.position;
            soundComp.SoundType = Enums.SoundEnum.ZombieAttack;

            _rangeAttackPool.Value.Del(entity);
            _coolDownPool.Value.Add(entity);
        }

        private ThrowData CalculatePredictedThrowData(
            ThrowData directionData, Vector3 targetVelocity, Vector3 targetPos)
        {
            Vector3 throwVelocity = directionData.ThrowVelocity;
            throwVelocity.y = 0;

            //float time = directionData.DeltaXZ * _predictedDistanceIndex / throwVelocity.magnitude;
            float time = directionData.DeltaXZ / throwVelocity.magnitude;

            Vector3 playerMovement = targetVelocity;
            //Vector3 playerMovement = _target.transform.forward * _targetMover.Speed * _predictedSpeedIndex;

            Vector3 newTargetPosition = new Vector3(
                targetPos.x + playerMovement.x,
                targetPos.y,
                targetPos.z + playerMovement.z
                );

            Vector3 randomAround = GetRandomVectorInRadius(newTargetPosition, 5f);

            var predictedData = CalculateThrowData(newTargetPosition, directionData.StartPos);
            //var predictedData = CalculateThrowData(randomAround, directionData.StartPos);
            predictedData.ThrowVelocity =
                Vector3.ClampMagnitude(predictedData.ThrowVelocity, _maxThrowForce);
            return predictedData;
        }

        private ThrowData CalculateThrowData(Vector3 targetPos, Vector3 startPos)
        {
            Vector3 displacement = new Vector3(
                targetPos.x,
                startPos.y,
                targetPos.z
                ) - startPos;

            float deltaY = targetPos.y - startPos.y;
            float deltaZX = displacement.magnitude;
            float gravity = Mathf.Abs(Physics.gravity.y);
            float throwStrength = Mathf.Clamp(
                Mathf.Sqrt(gravity * (deltaY + Mathf.Sqrt(Mathf.Pow(deltaY, 2) + Mathf.Pow(deltaZX, 2)))),
                0.01f, _maxThrowForce
                );

            float angle = Mathf.PI / 2f - (0.5f * (Mathf.PI / 2f - (deltaY / deltaZX)));

            Vector3 initialVelocity = Mathf.Cos(angle) * throwStrength * displacement.normalized +
                Mathf.Sin(angle) * throwStrength * Vector3.up;

            return new ThrowData
            {
                ThrowVelocity = initialVelocity,
                Angle = angle,
                DeltaXZ = deltaZX,
                DeltaY = deltaY,
                StartPos = startPos
            };
        }

        private Vector3 GetRandomVectorInRadius(Vector3 pos, float radius)
        {
            return new Vector3(
                Random.Range(pos.x - radius, pos.x + radius),
                pos.y,
                Random.Range(pos.z - radius, pos.z + radius)
                );
        }
    }

    public class ThrowData
    {
        public Vector3 ThrowVelocity { get; set; }
        public float Angle { get; set; }
        public float DeltaXZ { get; set; }
        public float DeltaY { get; set; }
        public Vector3 StartPos { get; set; }
    }
}