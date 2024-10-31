using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class OnStopPlayPromoVideoSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<OnStopInGamePromoVideoEvent>> _filter = default;
        private EcsFilterInject<Inc<PlayPromoVideoComp>> _promoVideoFilter = default;

        private EcsPoolInject<OnStopInGamePromoVideoEvent> _stopPromoPool = default;
        private EcsPoolInject<PlayPromoVideoComp> _promoVideoPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;
        private EcsPoolInject<TimeScaleEvent> _timeScalePool = default;
        private EcsPoolInject<RestoreCarHpEvent> _restoreHpPool = default;
        private EcsPoolInject<RestoreCarFuelEvent> _restoreFuelPool = default;
        private EcsPoolInject<LoadSceneEvent> _loadScenePool = default;
        private EcsPoolInject<AudioSourceComp> _soundsPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                //NormalizeTimeScale();

                switch (GetReasonForPromo())
                {
                    case Enums.PlayPromoVideoReason.RestoreHpAndFuel:
                        HandleRestoreHpAndFuel();
                        HandleSoundsAndMusic();
                        _state.Value.StartPlaySystems();
                        break;

                    case Enums.PlayPromoVideoReason.DoubleMissionMoney:
                        HandleDoubleMissionMoney();
                        break;
                }

                Debug.Log("видео закончилось");
                _stopPromoPool.Value.Del(entity);
            }
        }

        private void HandleSoundsAndMusic()
        {
            if (_state.Value.IsOnMasterVolume)
            {
                ref var soundsComp = ref _soundsPool.Value.Get(_state.Value.SoundsEntity);
                soundsComp.MasterBus.setMute(false);
            }

            SoundsHandlerMb.Instance.ContinueMusic();
        }

        private Enums.PlayPromoVideoReason GetReasonForPromo()
        {
            Enums.PlayPromoVideoReason result = default;
            foreach (var entity in _promoVideoFilter.Value)
            {
                ref var playPromoComp = ref _promoVideoPool.Value.Get(entity);
                result = playPromoComp.Reason;
                _promoVideoPool.Value.Del(entity);
            }
            return result;
        }

        private void NormalizeTimeScale()
        {
            ref var timeScaleComp = ref _timeScalePool.Value.Add(_world.Value.NewEntity());
            timeScaleComp.TimeScale = 1f;
        }

        private void HandleRestoreHpAndFuel()
        {
            _restoreHpPool.Value.Add(_state.Value.PlayerCarEntity);
            _restoreFuelPool.Value.Add(_state.Value.PlayerCarEntity);

            ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
            interfaceComp.CanvasBehaviour.DestroyPanelById(PanelsIdHolder.LosePanelId);
        }

        private void HandleDoubleMissionMoney()
        {
            _state.Value.PlayerMoneyScore += _state.Value.CurrentMission.MoneyReward;
            _state.Value.SaveMoneyScore();

            ref var loadComp = ref _loadScenePool.Value.Add(_world.Value.NewEntity());
            loadComp.SceneId = ScenesIdHolder.LobbySceneId;
        }
    }
}