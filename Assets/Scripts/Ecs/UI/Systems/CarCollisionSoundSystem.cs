using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using static Enums;

namespace Client
{
    public class CarCollisionSoundSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarCollisionSoundEvent>> _filter = default;
        private EcsPoolInject<CarCollisionSoundEvent> _carCollisionPool = default;
        private EcsPoolInject<Sound3DEvent> _soundPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var collisionComp = ref _carCollisionPool.Value.Get(entity);
                ref var soundComp = ref _soundPool.Value.Add(entity);
                soundComp.Position = collisionComp.Position;

                switch(collisionComp.PhysicalInteractableType)
                {
                    case PhysicalInteractableType.Default:
                        soundComp.SoundType = SoundEnum.CarHitDefault;
                        break;

                    case PhysicalInteractableType.Flash:
                        soundComp.SoundType = SoundEnum.CarHitZombie;
                        break;

                    case PhysicalInteractableType.Metal:
                        soundComp.SoundType = SoundEnum.CarHitMetal;
                        break;

                    case PhysicalInteractableType.Wood:
                        soundComp.SoundType = SoundEnum.CarHitWood;
                        break;
                }

                _carCollisionPool.Value.Del(entity);
            }
        }
    }
}