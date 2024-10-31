using FMODUnity;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using static Enums;

namespace Client
{
    sealed class SoundSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<AudioSourceComp> _audioSourcePool = default;

        private EcsFilterInject<Inc<UISoundEvent>> _uiSoundFilter = default;
        private EcsPoolInject<UISoundEvent> _uiSoundPool = default;

        private EcsFilterInject<Inc<Sound3DEvent>> _3dSoundFilter = default;
        private EcsPoolInject<Sound3DEvent> _3dSoundPool = default;

        private EcsPoolInject<ViewComp> _viewPool = default;

        public void Run(EcsSystems systems)
        {
            HandleUiSounds();
            Handle3dSounds();
        }

        private void HandleUiSounds()
        {
            foreach (var entity in _uiSoundFilter.Value)
            {
                ref var audiosourceComp = ref _audioSourcePool.Value.Get(_state.Value.SoundsEntity);
                ref var soundComp = ref _uiSoundPool.Value.Get(entity);
                SoundFMODConfig soundConfig = _state.Value.SoundFMODConfig;

                switch (soundComp.Sound)
                {
                    case SoundEnum.BuySound:
                        RuntimeManager.PlayOneShot(soundConfig.BuySound);
                        break;

                    case SoundEnum.ClickSound:
                        RuntimeManager.PlayOneShot(soundConfig.ClickSound);
                        break;

                    case SoundEnum.WarningSound:
                        RuntimeManager.PlayOneShot(soundConfig.WarningSound);
                        break;

                    case SoundEnum.MoneyIncreaseSound:
                        RuntimeManager.PlayOneShot(soundConfig.MoneyIncreaseSound);
                        break;

                    case SoundEnum.CheckpointMissionSound:
                        HandleCollectInUi();
                        break;

                    case SoundEnum.Pickup:
                        HandleCollectInUi();
                        break;

                    case SoundEnum.CollectMissionSound:
                        HandleCollectInUi();
                        break;

                    case SoundEnum.MissionCompleteSound:
                        RuntimeManager.PlayOneShot(soundConfig.MissionCompleteSound);
                        break;
                }
                _uiSoundPool.Value.Del(entity);
            }
        }

        private void Handle3dSounds()
        {
            foreach (var entity in _3dSoundFilter.Value)
            {
                ref var soundComp = ref _3dSoundPool.Value.Get(entity);
                var config = _state.Value.SoundFMODConfig;
                switch (soundComp.SoundType)
                {
                    //zombies
                    case SoundEnum.ZombieDead:
                        if (Tools.RandomByPersent(60))
                            HandleZombieDead(soundComp);
                        break;

                    case SoundEnum.ZombieAttack:
                        if (Tools.RandomByPersent(70))
                            HandleZombieAttack(soundComp);
                        break;

                    case SoundEnum.ZombieHitCar:
                        HandleCarHitByZombie(soundComp);
                        break;

                    //others
                    case SoundEnum.Explosion:
                        HandleOneShotInPosition(config.ExplosionSound, soundComp.Position);
                        break;

                    //cars
                    case SoundEnum.CarHitZombie:
                        HandleCarHit(config.CarHitZombieSound, soundComp.Position);
                        break;

                    case SoundEnum.CarHitWood:
                        HandleCarHit(config.CarHitWoodSound, soundComp.Position);
                        break;

                    case SoundEnum.CarHitMetal:
                        HandleCarHit(config.CarHitMetalSound, soundComp.Position);
                        break;

                    case SoundEnum.CarHitDefault:
                        HandleCarHit(config.CarHitDefaultSound, soundComp.Position);
                        break;
                }

                _3dSoundPool.Value.Del(entity);
            }
        }

        private void HandleCollectInUi()
        {
            ref var cameraViewComp = ref _viewPool.Value.Get(_state.Value.CameraEntity);
            var soundInstance = RuntimeManager.CreateInstance(_state.Value.SoundFMODConfig.PickupSound);
            RuntimeManager.AttachInstanceToGameObject(soundInstance, cameraViewComp.Transform);
            soundInstance.start();
            soundInstance.release();
        }

        private void HandleCarHit(EventReference eventReference, Vector3 pos)
        {
            var instance = RuntimeManager.CreateInstance(eventReference);
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
            instance.start();
            instance.release();
        }

        private void HandleOneShotInPosition(EventReference eventReference, Vector3 pos) =>
            RuntimeManager.PlayOneShot(eventReference, pos);

        private void HandleZombieAttack(Sound3DEvent soundEvent) =>
            RuntimeManager.PlayOneShot(
                _state.Value.SoundFMODConfig.ZombieAttackSound, soundEvent.Position);

        private void HandleZombieDead(Sound3DEvent soundEvent)
        {
            var soundInstance = RuntimeManager.CreateInstance(_state.Value.SoundFMODConfig.ZombieDeadSound);
            RuntimeManager.AttachInstanceToGameObject(
                soundInstance, soundEvent.Transform, soundEvent.MainRigidbody);

            soundInstance.start();
            soundInstance.release();
        }

        private void HandleCarHitByZombie(Sound3DEvent soundEvent) =>
            RuntimeManager.PlayOneShot(
                _state.Value.SoundFMODConfig.ZombieHitCarSound, soundEvent.Position);
    }
}