using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class MoveShopCarSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<MoveShopCarComp>> _filter = default;
        private EcsPoolInject<MoveShopCarComp> _movePool = default;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var moveComp = ref _movePool.Value.Get(entity);
                moveComp.Timer -= Time.deltaTime;

                float indexOfTime = 1f - moveComp.Timer / moveComp.Duration;
                moveComp.Transform.position = Vector3.Lerp(moveComp.StartPosition, moveComp.EndPosition, indexOfTime);

                if (moveComp.Timer < 0)
                {
                    moveComp.Transform.position = moveComp.EndPosition;
                    _movePool.Value.Del(entity);
                }
            }
        }
    }
}