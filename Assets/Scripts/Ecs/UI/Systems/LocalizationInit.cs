using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

namespace Client
{
    sealed class LocalizationInit : IEcsInitSystem, IEcsDestroySystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsPoolInject<LocalizationComp> _localizationPool = default;
        private EcsPoolInject<LocalizationChangeEvent> _localizationChangePool = default;

        public void Init(EcsSystems systems)
        {
            LocalizationSettings.Instance.OnSelectedLocaleChanged += OnChangeLang;

            ref var localizationComp = ref _localizationPool.Value.Add(_state.Value.InterfaceEntity);
            localizationComp.LocalesDictionary = new Dictionary<string, int>();
            localizationComp.LocalesCodes = new List<string>();

            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                string code = LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code;
                localizationComp.LocalesDictionary.Add(code, i);
                localizationComp.LocalesCodes.Add(code);
            }

            //localizationComp.UiLocalizedTable = _uiTable;
            _localizationChangePool.Value.Add(_world.Value.NewEntity());
        }

        private void OnChangeLang(Locale locale)
        {
            _localizationChangePool.Value.Add(_world.Value.NewEntity());
            Debug.Log("LocalizationInit OnChangeLang");
        }

        public void Destroy(EcsSystems systems)
        {
            LocalizationSettings.Instance.OnSelectedLocaleChanged -= OnChangeLang;
            Debug.Log("LocalizationInit OnDestroy");
        }
    }
}