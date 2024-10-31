using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class VolumeChangeSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<VolumeChangeEvent>> _filter = default;
        private EcsPoolInject<VolumeChangeEvent> _volumePool = default;
        private EcsPoolInject<AudioSourceComp> _audioPool = default;

        private EcsFilterInject<Inc<ChangeGlobalStateEvent>> _changeGlobalStateFilter = default;

        public void Run(EcsSystems systems)
        {
            HandleSoundsChange();
            HandlePlayAndPauseTransitions();
        }

        private void HandleSoundsChange()
        {
            foreach (var entity in _filter.Value)
            {
                ref var audioComp = ref _audioPool.Value.Get(_state.Value.SoundsEntity);
                if (_state.Value.IsOnMasterVolume)
                {
                    audioComp.MasterBus.setMute(false);
                    audioComp.MasterBus.setVolume(_state.Value.MasterVolumeValue);
                }
                else
                {   
                    audioComp.MasterBus.setMute(true);
                }

                if (_state.Value.IsOnSounds)
                {
                    audioComp.SoundsBus.setMute(false);
                    audioComp.SoundsBus.setVolume(_state.Value.SoundsVolumeValue);
                }
                else
                {
                    audioComp.SoundsBus.setMute(true);
                }

                if (_state.Value.IsOnMusic)
                {
                    Debug.Log("VolumeChangeSystem MusicBus IsOnMusic setVolume = " + _state.Value.MusicVolumeValue);
                    audioComp.MusicBus.setMute(false);
                    audioComp.MusicBus.setVolume(_state.Value.MusicVolumeValue);
                }
                else
                {
                    Debug.Log("VolumeChangeSystem MusicBus !IsOnMusic");
                    audioComp.MusicBus.setMute(true);
                }

                _volumePool.Value.Del(entity);
            }
        }

        private void HandlePlayAndPauseTransitions()
        {
            foreach (var entity in _changeGlobalStateFilter.Value)
            {
                ref var audioComp = ref _audioPool.Value.Get(_state.Value.SoundsEntity);
                audioComp.InPlayBus.setMute(_state.Value.PauseSystems);
            }
        }
    }
}