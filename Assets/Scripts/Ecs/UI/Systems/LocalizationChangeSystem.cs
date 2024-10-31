using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class LocalizationChangeSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<LocalizationChangeEvent>> _filter = default;
        private EcsPoolInject<LocalizationChangeEvent> _localizationChangePool = default;
        private EcsPoolInject<LocalizationComp> _localizationPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                var localizationConfig = _state.Value.LocalizationConfig;
                ref var localizationComp = ref _localizationPool.Value.Get(_state.Value.InterfaceEntity);

                localizationComp.UiCurrentTable = localizationConfig.UiTable.GetTable();
                localizationComp.CarShopCurrentTable = localizationConfig.CarShopTable.GetTable();
                localizationComp.BrifCurrentTable = localizationConfig.MissionBrifsTable.GetTable();
                localizationComp.MissionNamesCurrentTable = localizationConfig.MissionNamesTable.GetTable();
                localizationComp.MissionPartsCurrentTable = localizationConfig.MissionPartsTable.GetTable();
                localizationComp.TutorialCurrentTable = localizationConfig.TutorialTable.GetTable();

                _localizationChangePool.Value.Del(entity);
            }
        }
    }
}