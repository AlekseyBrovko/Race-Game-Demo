using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Client
{
    sealed class LoadSceneSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<LoadSceneEvent>> _filter = default;
        private EcsPoolInject<LoadSceneEvent> _loadScenePool = default;
        private EcsPoolInject<AudioSourceComp> _audioSourcePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var sceneComp = ref _loadScenePool.Value.Get(entity);

                HandleSounds();
                WorkWithFadeCanvas();

                SceneManager.LoadScene(sceneComp.SceneId);
                _loadScenePool.Value.Del(entity);
            }
        }

        private void HandleSounds()
        {
            ref var audioSourceComp = ref _audioSourcePool.Value.Get(_state.Value.SoundsEntity);
            audioSourceComp.InPlayBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        private void WorkWithFadeCanvas()
        {
            FadeCanvas fadeCanvas = GameObject.Instantiate(_state.Value.UiConfig.FadeCanvasPrefab);
            fadeCanvas.Init();
        }
    }
}