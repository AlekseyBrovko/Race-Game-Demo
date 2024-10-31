using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using static Enums;

namespace Client
{
    sealed class CarPickupsSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<PickupEvent>> _filter = default;
        private EcsPoolInject<PickupEvent> _pickupPool = default;
        private EcsPoolInject<SpawnPickupCoolDownComp> _spawnCoolDown = default;
        private EcsPoolInject<SilentMoneyEvent> _moneyPool = default;
        private EcsPoolInject<UISoundEvent> _soundPool = default;
        private EcsPoolInject<RestoreCarHpEvent> _restoreHpPool = default;
        private EcsPoolInject<RestoreCarFuelEvent> _restoreFuelPool = default;

        private int _moneyIncreaseValue = 300;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var pickupComp = ref _pickupPool.Value.Get(entity);
                ref var coolDownComp = ref _spawnCoolDown.Value.Add(entity);
                coolDownComp.Position = pickupComp.PositionOfPickup;

                switch (pickupComp.Type)
                {
                    case PickupType.Health:
                        _restoreHpPool.Value.Add(_state.Value.PlayerCarEntity);
                        break;

                    case PickupType.Time:
                        _restoreFuelPool.Value.Add(_state.Value.PlayerCarEntity);
                        break;

                    case PickupType.Money:
                        ref var moneyComp = ref _moneyPool.Value.Add(_world.Value.NewEntity());
                        moneyComp.MoneyValue = _moneyIncreaseValue;
                        break;
                }

                ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
                soundComp.Sound = SoundEnum.Pickup;
                _pickupPool.Value.Del(entity);
            }
        }
    }
}