using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ResetPositionAndRotationOfBodySystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<NpcResetPositionBodyEvent>> _filter = default;
        private EcsPoolInject<NpcResetPositionBodyEvent> _eventPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
                ResetPosition(entity);
        }

        private void ResetPosition(int entity)
        {
            ref var eventComp = ref _eventPool.Value.Get(entity);
            ref var npcComp = ref _npcPool.Value.Get(eventComp.NpcEntity);
            npcComp.TransformOfBody.localPosition = Vector3.zero;
            npcComp.TransformOfBody.localRotation = Quaternion.identity;
            _eventPool.Value.Del(entity);
        }
    }
}