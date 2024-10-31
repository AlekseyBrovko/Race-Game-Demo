using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ThrowObjectsInitSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<ThrowObjectInitEvent>> _filter = default;
        private EcsPoolInject<ThrowObjectInitEvent> _initPool = default;
        private EcsPoolInject<ThrowObjectComp> _throwObjectPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var initComp = ref _initPool.Value.Get(entity);
                int throwEntity = _world.Value.NewEntity();

                ref var throwComp = ref _throwObjectPool.Value.Add(throwEntity);
                throwComp.ThrowMb = initComp.ThrowGo.GetComponent<IThrowObject>();
                throwComp.ThrowMb.Init(_state.Value, throwEntity);
                throwComp.Transform = initComp.ThrowGo.transform;

                ref var viewComp = ref _viewPool.Value.Add(throwEntity);
                viewComp.Transform = initComp.ThrowGo.transform;

                _initPool.Value.Del(entity);
            }
        }
    }
}