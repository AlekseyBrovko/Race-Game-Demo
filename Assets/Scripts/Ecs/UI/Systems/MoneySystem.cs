using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class MoneySystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<SilentMoneyEvent>> _silentFilter = default;
        private EcsFilterInject<Inc<MoneySpendEvent>> _spendfilter = default;
        private EcsFilterInject<Inc<MoneyIncreaseEvent>> _increasefilter = default;

        private EcsPoolInject<SilentMoneyEvent> _silentMoneyPool = default;
        private EcsPoolInject<MoneySpendEvent> _spendMoneyPool = default;
        private EcsPoolInject<MoneyIncreaseEvent> _increaseMoneyPool = default;
        private EcsPoolInject<UISoundEvent> _soundPool = default;
        private EcsPoolInject<VisualMoneyEvent> _visualMoneyPool = default;

        public void Run(EcsSystems systems)
        {
            HandleSilentMoneyEvents();
            HandleSpendMoneyEvents();
            HandleIncreaseMoneyEvents();
        }

        private void HandleSilentMoneyEvents()
        {
            foreach (var entity in _silentFilter.Value)
            {
                ref var moneyComp = ref _silentMoneyPool.Value.Get(entity);
                _state.Value.PlayerMoneyScore += moneyComp.MoneyValue;
                
                _visualMoneyPool.Value.Add(_world.Value.NewEntity());
                _silentMoneyPool.Value.Del(entity);
            }
        }

        private void HandleSpendMoneyEvents()
        {
            foreach (var entity in _spendfilter.Value)
            {
                ref var moneyComp = ref _spendMoneyPool.Value.Get(entity);
                _state.Value.PlayerMoneyScore -= moneyComp.MoneyValue;

                ref var soundComp = ref _soundPool.Value.Add(entity);
                soundComp.Sound = Enums.SoundEnum.BuySound;

                _state.Value.SaveMoneyScore();
                _visualMoneyPool.Value.Add(_world.Value.NewEntity());
                _spendMoneyPool.Value.Del(entity);
            }
        }

        private void HandleIncreaseMoneyEvents()
        {
            foreach (var entity in _increasefilter.Value)
            {
                ref var moneyComp = ref _increaseMoneyPool.Value.Get(entity);
                _state.Value.PlayerMoneyScore += moneyComp.MoneyValue;

                ref var soundComp = ref _soundPool.Value.Add(entity);
                soundComp.Sound = Enums.SoundEnum.MoneyIncreaseSound;

                _visualMoneyPool.Value.Add(_world.Value.NewEntity());
                _increaseMoneyPool.Value.Del(entity);
            }
        }
    }

    sealed class UiMoneySystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<VisualMoneyEvent>> _filter = default;
        private EcsFilterInject<Inc<MoneyPanelComp>> _moneyPanelFilter = default;

        private EcsPoolInject<VisualMoneyEvent> _uiMoneyPool = default;
        private EcsPoolInject<MoneyPanelComp> _moneyPanelPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var eventEntity in _filter.Value)
            {
                foreach (var uiEntity in _moneyPanelFilter.Value)
                {
                    ref var moneyPanelComp = ref _moneyPanelPool.Value.Get(uiEntity);
                    moneyPanelComp.MoneyPanelMb.ShowMoney();
                }
                _uiMoneyPool.Value.Del(eventEntity);
            }
        }
    }
}