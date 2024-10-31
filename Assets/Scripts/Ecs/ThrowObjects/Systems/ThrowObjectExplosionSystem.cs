using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client
{
    sealed class ThrowObjectExplosionSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<ExplosionThrowingObjectEvent>> _filter = default;
        private EcsPoolInject<ExplosionThrowingObjectEvent> _explosionEventPool = default;
        private EcsPoolInject<NpcDamageEvent> _npcDamagePool = default;
        private EcsPoolInject<CarDamageEvent> _carDamagePool = default;
        private EcsPoolInject<Sound3DEvent> _soundPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var exlosionComp = ref _explosionEventPool.Value.Get(entity);
                Vector3 pos = exlosionComp.ThrowObject.Transform.position;
                GameObject.Destroy(exlosionComp.ThrowObject.Transform.gameObject);

                PlayParticles(pos);
                PlayAudio(pos);
                Explosion(pos, exlosionComp);

                _explosionEventPool.Value.Del(entity);
            }
        }

        private void Explosion(Vector3 pos, ExplosionThrowingObjectEvent explosionComp)
        {   
            List<IEcsEntityMb> ecsEntityMbs = new List<IEcsEntityMb>();

            var settingsConfig = _state.Value.SettingsConfig;
            Collider[] colliders = Physics.OverlapSphere(pos, settingsConfig.ExplosionRadiusOfDamage);
            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent(out IEcsEntityMb entityMb))
                    ecsEntityMbs.Add(entityMb);
            }

            List<IEcsEntityMb> noDupesEcsEntityMbs = ecsEntityMbs.Distinct().ToList();

            foreach (IEcsEntityMb entityMb in noDupesEcsEntityMbs)
            {
                switch (entityMb.EntityType)
                {
                    case Enums.EntityType.Car:
                        HandleCarExplosion(entityMb.Entity, explosionComp);
                        break;

                    case Enums.EntityType.Npc:
                        HandleNpcExplosion(entityMb.Entity, explosionComp);
                        break;
                }
            }
        }

        private void PlayParticles(Vector3 pos)
        {
            ParticlesConfig particleConfig = _state.Value.ParticlesConfig;
            GameObject particle = GameObject.Instantiate(particleConfig.ExplosionParticle);
            particle.transform.position = pos;
            IParticles particleMb = particle.GetComponent<IParticles>();
            particleMb.PlayParticles();
        }

        private void PlayAudio(Vector3 pos)
        {
            ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
            soundComp.Position = pos;
            soundComp.SoundType = Enums.SoundEnum.Explosion;
        }

        private void HandleCarExplosion(int entity, ExplosionThrowingObjectEvent explosionComp)
        {
            var settingsConfig = _state.Value.SettingsConfig;
            ref var damageExplosionComp = ref _carDamagePool.Value.Add(_world.Value.NewEntity());
            damageExplosionComp.DamageValue = settingsConfig.RangeExplosionDamage;
            damageExplosionComp.DamageType = Enums.DamageType.Explosion;
            damageExplosionComp.DamagedEntity = entity;
            damageExplosionComp.DamagerEntity = explosionComp.ThrowObject.Owner.Entity;
            damageExplosionComp.PointOfForce = explosionComp.ThrowObject.Transform.position;
        }

        private void HandleNpcExplosion(int entity, ExplosionThrowingObjectEvent explosionComp)
        {
            ref var damageExplosionComp = ref _npcDamagePool.Value.Add(_world.Value.NewEntity());
            damageExplosionComp.DamageValue = float.PositiveInfinity;
            damageExplosionComp.DamageType = Enums.DamageType.Explosion;
            damageExplosionComp.DamagedEntity = entity;
            damageExplosionComp.DamagerEntity = explosionComp.ThrowObject.Owner.Entity;
            damageExplosionComp.PointOfForce = explosionComp.ThrowObject.Transform.position;
        }
    }
}