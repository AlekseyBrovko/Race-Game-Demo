using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ZonesSeparatorsInit : IEcsInitSystem
    {
        public ZonesSeparatorsInit(SceneIniterMb sceneIniter) =>
            _sceneIniter = sceneIniter;

        private SceneIniterMb _sceneIniter;
        private EcsCustomInject<GameState> _state = default;

        public void Init(EcsSystems systems)
        {
            foreach (Transform child in _sceneIniter.ZoneSeparators)
                if (child.TryGetComponent(out ZoneSeparatorMb zoneMb))
                    zoneMb.Init(_state.Value);
        }
    }
}