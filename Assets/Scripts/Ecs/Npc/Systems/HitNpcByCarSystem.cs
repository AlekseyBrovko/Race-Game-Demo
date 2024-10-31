using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class HitNpcByCarSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<HitPedestrianByCarEvent>> _triggerByCarFilter = default;
        private EcsFilterInject<Inc<ColisionPedestrianByCarEvent>> _collisionByCarFilter = default;
        //private EcsFilterInject<Inc<HitByCarDumpEvent>> _carDumpFilter = default;

        private EcsPoolInject<HitPedestrianByCarEvent> _hitEventPool = default;
        private EcsPoolInject<ColisionPedestrianByCarEvent> _collisionEventPool = default;
        //private EcsPoolInject<HitByCarDumpEvent> _hitByDumpPool = default;

        private EcsPoolInject<NpcDamageEvent> _damagePool = default;

        private EcsPoolInject<NpcComp> _npcPool = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        //private EcsPoolInject<NpcIsHurtingByCollisionComp> _hurtingByCarPool = default;

        private EcsPoolInject<NpcStartHurtByCarSystemsEvent> _startHurtPool = default;
        private EcsPoolInject<NpcHurtByCarComp> _hurtPool = default;

        private float _noDamageThreshold = 3f;
        private float _hitToRagdollSpeedThreshold = 20f;
        private float _damageValue = 20f;

        private float _hitImpulseForCar = 3000f;
        //private float _hitImpulseForNpc = 300f;

        public void Run(EcsSystems systems)
        {
            HandleCarTriggerEnter();
            HandleCarCollisionEnter();
            //HandleDumpEvent();
        }

        private void HandleCarTriggerEnter()
        {
            foreach (var entity in _triggerByCarFilter.Value)
            {
                ref var hitComp = ref _hitEventPool.Value.Get(entity);
                ref var npcComp = ref _npcPool.Value.Get(hitComp.PadastrianEntity);
                ref var statisticComp = ref _statisticPool.Value.Get(hitComp.CarEntity);

                if (statisticComp.SpeedKmpH > _hitToRagdollSpeedThreshold)
                {
                    SetDamageToEntity(hitComp.PadastrianEntity, hitComp.CarEntity, float.PositiveInfinity);
                    SetForceOnCar(hitComp.CarEntity, hitComp.PadastrianEntity);
                }
                _hitEventPool.Value.Del(entity);
            }
        }

        private void HandleCarCollisionEnter()
        {
            foreach (var entity in _collisionByCarFilter.Value)
            {
                ref var collisionComp = ref _collisionEventPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(collisionComp.CarEntity);
                ref var carViewComp = ref _viewPool.Value.Get(collisionComp.CarEntity);

                if (statisticComp.PreviousFrameSpeedKmpH > _noDamageThreshold && statisticComp.PreviousFrameSpeedKmpH <= _hitToRagdollSpeedThreshold)
                {
                    if (!_hurtPool.Value.Has(collisionComp.PadastrianEntity))
                    {
                        SetDamageToEntity(collisionComp.PadastrianEntity, collisionComp.CarEntity, _damageValue);
                        int padastrianEntity = collisionComp.PadastrianEntity;
                        
                        //TODO выскочила ошибка, уже назначен
                        ref var startHurtComp = ref _startHurtPool.Value.Add(padastrianEntity);
                        startHurtComp.HitObjectTransform = carViewComp.Transform;
                    }
                }
                _collisionEventPool.Value.Del(entity);
            }
        }

        //private void SetCarHitComponent(int padastrianEntity, int carEntity)
        //{
        //    if (!_tempObjectHitPool.Value.Has(padastrianEntity))
        //        _tempObjectHitPool.Value.Add(padastrianEntity);

        //    ref var hitComp = ref _tempObjectHitPool.Value.Get(padastrianEntity);
        //    ref var carViewComp = ref _viewPool.Value.Get(carEntity);
        //    hitComp.HitObjectTransform = carViewComp.Transform;
        //    SetForceOnCar(carEntity, padastrianEntity);
        //}

        ////TODO
        //private void HandleDumpEvent()
        //{
        //    foreach (var entity in _carDumpFilter.Value)
        //    {
        //        ref var hitComp = ref _hitByDumpPool.Value.Get(entity);
        //        ref var statisticComp = ref _statisticPool.Value.Get(hitComp.CarEntity);

        //        if (statisticComp.SpeedKmpH > _hitToRagdollSpeedThreshold)
        //        {
        //            //SetDamageToEntity(hitComp.PadastrianEntity, float.PositiveInfinity);
        //            //SetForceOnCar(hitComp.CarEntity, hitComp.PadastrianEntity);
        //        }
        //        else
        //        {

        //        }

        //        _hitByDumpPool.Value.Del(entity);
        //    }
        //}

        private void SetDamageToEntity(int damageIntity, int damagerEntity, float value)
        {
            ref var damageComp = ref _damagePool.Value.Add(_world.Value.NewEntity());
            damageComp.DamageType = Enums.DamageType.CarHit;
            damageComp.DamageValue += value;
            damageComp.DamagerEntity = damagerEntity;
            damageComp.DamagedEntity = damageIntity;
        }

        //private void SetForceOnPedestrian(int padastrianEntity)
        //{
        //    ref var padastrianViewComp = ref _viewPool.Value.Get(padastrianEntity);
        //    ref var npcComp = ref _npcPool.Value.Get(padastrianEntity);
        //    npcComp.NpcMb.Rigidbody.AddForce(padastrianViewComp.Transform.up * _hitImpulseForNpc, ForceMode.Impulse);
        //}

        private void SetForceOnCar(int carEntity, int padastrianEntity)
        {
            ref var carComp = ref _carPool.Value.Get(carEntity);
            ref var carViewComp = ref _viewPool.Value.Get(carEntity);
            ref var padastrianViewComp = ref _viewPool.Value.Get(padastrianEntity);

            Vector3 carPos = new Vector3(carViewComp.Transform.position.x, 0f, carViewComp.Transform.position.z);
            Vector3 padastrianPos = new Vector3(padastrianViewComp.Transform.position.x, padastrianViewComp.Transform.position.y + 2f, padastrianViewComp.Transform.position.z);

            Vector3 direction = (carPos - padastrianPos).normalized;
            carComp.CarMb.Rigidbody.AddForce(direction * _hitImpulseForCar, ForceMode.Impulse);
            carComp.CarMb.Rigidbody.AddForce(Vector3.down * _hitImpulseForCar / 2f);
        }
    }
}