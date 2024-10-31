using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ThrowObjectDeleteSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ThrowObjectDelayBeforeDeleteComp>> _filter = default;

        private EcsPoolInject<ThrowObjectDelayBeforeDeleteComp> _deletePool = default;
        private EcsPoolInject<ThrowObjectComp> _throwObjectPool = default;

        private float _timeForDelete = 20f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var deleteComp = ref _deletePool.Value.Get(entity);
                deleteComp.Timer += Time.fixedDeltaTime;
                if (deleteComp.Timer > _timeForDelete)
                    DeleteThrowObject(entity);
            }
        }

        private void DeleteThrowObject(int entity)
        {
            ref var throwComp = ref _throwObjectPool.Value.Get(entity);
            throwComp.ThrowMb.DestroyImmediately();
        }
    }
}