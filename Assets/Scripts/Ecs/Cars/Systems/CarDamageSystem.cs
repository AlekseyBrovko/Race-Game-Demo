using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarDamageSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<CarDamageEvent>> _filter = default;

        private EcsPoolInject<HpComp> _hpPool = default;
        private EcsPoolInject<CarDamageEvent> _damagePool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<VisualHpEvent> _uiHpPool = default;
        private EcsPoolInject<Sound3DEvent> _soundPool = default;
        private EcsPoolInject<CarDeathEvent> _deathEvent = default;

        private float _forceOfHit = 100000f;

        //TODO доделать
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var damageComp = ref _damagePool.Value.Get(entity);
                int damagedEntity = damageComp.DamagedEntity;

                ref var hpComp = ref _hpPool.Value.Get(damagedEntity);
                hpComp.HpValue -= damageComp.DamageValue;

                switch (damageComp.DamageType)
                {
                    case Enums.DamageType.Melee:
                        ref var damagerViewComp = ref _viewPool.Value.Get(damageComp.DamagerEntity);
                        AddForceOnCar(damagedEntity, damagerViewComp.Transform);
                        PlaySound(damagerViewComp.Transform);
                        break;

                    case Enums.DamageType.Explosion:

                        break;

                    case Enums.DamageType.TrowObject:

                        break;
                }

                if (_state.Value.PlayerCarEntity == damagedEntity)
                    _uiHpPool.Value.Add(_world.Value.NewEntity());

                if (hpComp.HpValue <= 0)
                {
                    ref var deathComp = ref _deathEvent.Value.Add(_world.Value.NewEntity());
                    deathComp.CarEntity = damagedEntity;
                }

                _damagePool.Value.Del(entity);
            }
        }

        private void PlaySound(Transform transform)
        {
            ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
            soundComp.Position = transform.position;
            soundComp.SoundType = Enums.SoundEnum.ZombieHitCar;
        }

        private void AddForceOnCar(int carEntity, Transform damagerTransform)
        {
            ref var carViewComp = ref _viewPool.Value.Get(carEntity);
            ref var carComp = ref _carPool.Value.Get(carEntity);

            Vector3 direction = (carViewComp.Transform.position - (damagerTransform.position + Vector3.up)).normalized;
            carComp.Rigidbody.AddForce(direction * _forceOfHit);
        }
    }
}