using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class NpcInitPatrollPointsSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<NpcInitPatrollPointsEvent>> _filter = default;
        private EcsPoolInject<NpcInitPatrollPointsEvent> _eventPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<LayerMaskComp> _layersPool = default;
        private EcsPoolInject<NpcPatrollPointsComp> _patrollPointsPool = default;

        private float _radiusOfSearch = 50f;
        private int _patrollPointsCount = 4;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var viewComp = ref _viewPool.Value.Get(entity);
                ref var layersComp = ref _layersPool.Value.Get(_state.Value.LayerMaskEntity);

                Vector3 pos = viewComp.Transform.position;
                Collider[] colliders = Physics.OverlapSphere(pos, _radiusOfSearch, layersComp.PatrollPointsMask, QueryTriggerInteraction.Collide);
                
                if (colliders.Length < 0)
                {
                    _eventPool.Value.Del(entity);
                    Debug.Log("МОБ НЕ НАШЁЛ КОНТРОЛЬНЫЕ ТОЧКИ");
                    return;
                }

                List<Transform> tempTransforms = new List<Transform>();
                foreach (var collider in colliders)
                    tempTransforms.Add(collider.transform);

                if (!_patrollPointsPool.Value.Has(entity))
                    _patrollPointsPool.Value.Add(entity);
                ref var patrollPointsComp = ref _patrollPointsPool.Value.Get(entity);
                patrollPointsComp.PatrollPoints = Tools.GetSeveralNearestTransforms(pos, tempTransforms, _patrollPointsCount);

                _eventPool.Value.Del(entity);
            }
        }
    }
}