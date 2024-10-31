using FMODUnity;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitSounds : IEcsInitSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<AudioSourceComp> _audioPool = default;
        private EcsPoolInject<VolumeChangeEvent> _volumeChangePool = default;

        public void Init(EcsSystems systems)
        {
            var entity = _world.Value.NewEntity();
            _state.Value.SoundsEntity = entity;
            ref var audioComp = ref _audioPool.Value.Add(entity);

#if UNITY_EDITOR
            if (SoundsHandlerMb.Instance == null)
            {
                SoundsHandlerMb soundHandler = Object.FindObjectOfType<SoundsHandlerMb>(true);
                soundHandler.gameObject.SetActive(true);
            }
#endif

            var config = _state.Value.SoundFMODConfig;
            audioComp.MasterBus = RuntimeManager.GetBus(config.MasterBusTag);
            audioComp.MusicBus = RuntimeManager.GetBus(config.MusicBusTag);
            audioComp.SoundsBus = RuntimeManager.GetBus(config.SoundsBusTag);
            audioComp.InPlayBus = RuntimeManager.GetBus(config.InPlayBusTag);

            _volumeChangePool.Value.Add(_world.Value.NewEntity());
        }
    }
}