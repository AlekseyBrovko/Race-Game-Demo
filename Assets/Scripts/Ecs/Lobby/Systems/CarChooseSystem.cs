using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class CarChooseSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CarChooseEvent>> _filter = default;
        private EcsPoolInject<CarChooseEvent> _choosePool = default;
        private EcsPoolInject<UISoundEvent> _soundPool = default;
        private EcsPoolInject<CarShopViewEvent> _carShopViewEventPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var chooseComp = ref _choosePool.Value.Get(entity);
                if (chooseComp.CarId != _state.Value.CurrentPlayerCar)
                {
                    _state.Value.CurrentPlayerCar = chooseComp.CarId;
                    _state.Value.SaveCars();
                }
                
                ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
                soundComp.Sound = Enums.SoundEnum.ClickSound;

                _carShopViewEventPool.Value.Add(_world.Value.NewEntity());

                _choosePool.Value.Del(entity);
            }
        }
    }
}