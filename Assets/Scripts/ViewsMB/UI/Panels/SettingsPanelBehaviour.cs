using Client;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsPanelBehaviour : PanelBase
{
    [SerializeField] private Button _backButton;

    [SerializeField] private Toggle _volumeToggle;
    [SerializeField] private Toggle _soundsToggle;
    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private Toggle _vibroToggle;

    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _soundsSlider;
    [SerializeField] private Slider _musicSlider;

    [SerializeField] private GameObject _vibrationIcon;

    [SerializeField] private Dropdown _languageDropDown;

    private EcsWorld _world;
    private EcsPool<VolumeChangeEvent> _changeAudioPool;
    private EcsPool<LocalizationComp> _localizationPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _world = _state.EcsWorld;
        _changeAudioPool = _world.GetPool<VolumeChangeEvent>();
        _localizationPool = _world.GetPool<LocalizationComp>();

        if (!state.IsMobilePlatform)
            _vibrationIcon.gameObject.SetActive(false);

        _backButton.onClick.AddListener(() => OnBackButtonClick());

        BuildPanel();
        InitTogglesAndSliders();
        InitLangDropDown();
    }

    public override void CleanupPanel() =>
        _state.SaveSettings();

    private void InitLangDropDown()
    {
        _languageDropDown.ClearOptions();
        ref var localComp = ref _localizationPool.Get(_state.InterfaceEntity);

        for (int i = 0; i < localComp.LocalesCodes.Count; i++)
            _languageDropDown.options.Add(new Dropdown.OptionData(localComp.LocalesCodes[i]));

        _languageDropDown.value = localComp.LocalesCodes.IndexOf(
            LocalizationSettings.SelectedLocale.Identifier.Code);

        _languageDropDown.onValueChanged.AddListener(OnLangDropDownChange);
    }

    private void OnLangDropDownChange(int dropDownValue) =>
        StartCoroutine(SetLocale(dropDownValue));

    private IEnumerator SetLocale(int localeId)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
    }

    private void BuildPanel()
    {
        if (_state.IsOnMasterVolume)
        {
            _volumeToggle.isOn = true;
            _volumeSlider.interactable = true;
            _volumeSlider.value = _state.MasterVolumeValue;
        }
        else
        {
            _volumeToggle.isOn = false;
            _volumeSlider.interactable = false;
            _volumeSlider.value = 0;
        }

        if (_state.IsOnSounds)
        {
            _soundsToggle.isOn = true;
            _soundsSlider.enabled = true;
            _soundsSlider.value = _state.SoundsVolumeValue;
        }
        else
        {
            _soundsToggle.isOn = false;
            _soundsSlider.enabled = false;
            _soundsSlider.value = 0;
        }

        if (_state.IsOnMusic)
        {
            _musicToggle.isOn = true;
            _musicSlider.enabled = true;
            _musicSlider.value = _state.MusicVolumeValue;
        }
        else
        {
            _musicToggle.isOn = false;
            _musicSlider.enabled = false;
            _musicSlider.value = 0;
        }

        if (_state.IsOnVibration)
            _vibroToggle.isOn = true;
        else
            _vibroToggle.isOn = false;
    }

    private void InitTogglesAndSliders()
    {
        _volumeToggle.onValueChanged.AddListener(delegate
        {
            _canvasMb.PlayClickSound();
            OnVolumeToggleChange(_volumeToggle);
        });

        _soundsToggle.onValueChanged.AddListener(delegate
        {
            _canvasMb.PlayClickSound();
            OnSoundsToggleChange(_soundsToggle);
        });

        _musicToggle.onValueChanged.AddListener(delegate
        {
            _canvasMb.PlayClickSound();
            OnMusicToggleChange(_musicToggle);
        });

        _vibroToggle.onValueChanged.AddListener(delegate
        {
            _canvasMb.PlayClickSound();
            OnVibroToggleChange(_vibroToggle);
        });

        _volumeSlider.onValueChanged.AddListener(delegate
        {
            OnVolumeSliderChange(_volumeSlider);
        });

        _soundsSlider.onValueChanged.AddListener(delegate
        {
            OnSoundsSliderChange(_soundsSlider);
        });

        _musicSlider.onValueChanged.AddListener(delegate
        {
            OnMusicSliderChange(_musicSlider);
        });
    }

    private void OnVolumeToggleChange(Toggle volumeToggle)
    {
        _state.IsOnMasterVolume = volumeToggle.isOn;
        _volumeSlider.enabled = volumeToggle.isOn;

        if (volumeToggle.isOn)
            _volumeSlider.value = _state.MasterVolumeValue;
        else
            _volumeSlider.value = 0;

        _changeAudioPool.Add(_world.NewEntity());
    }

    private void OnSoundsToggleChange(Toggle soundsToggle)
    {
        _state.IsOnSounds = soundsToggle.isOn;
        _soundsSlider.enabled = soundsToggle.isOn;

        if (soundsToggle.isOn)
            _soundsSlider.value = _state.SoundsVolumeValue;
        else
            _soundsSlider.value = 0;

        _changeAudioPool.Add(_world.NewEntity());
    }

    private void OnMusicToggleChange(Toggle musicToggle)
    {
        _state.IsOnMusic = musicToggle.isOn;
        _musicSlider.enabled = musicToggle.isOn;

        if (musicToggle.isOn)
            _musicSlider.value = _state.MusicVolumeValue;
        else
            _musicSlider.value = 0;

        _changeAudioPool.Add(_world.NewEntity());
    }

    private void OnVibroToggleChange(Toggle vibroToggle) =>
        _state.IsOnVibration = vibroToggle.isOn;

    private void OnVolumeSliderChange(Slider slider)
    {
        if (_volumeToggle.isOn)
        {
            _state.MasterVolumeValue = slider.value;
            _changeAudioPool.Add(_world.NewEntity());
        }
    }

    private void OnSoundsSliderChange(Slider soundsSlider)
    {
        if (_soundsToggle.isOn)
        {
            _state.SoundsVolumeValue = soundsSlider.value;
            _changeAudioPool.Add(_world.NewEntity());
        }
    }

    private void OnMusicSliderChange(Slider slider)
    {
        if (_musicToggle.isOn)
        {
            _state.MusicVolumeValue = slider.value;
            _changeAudioPool.Add(_world.NewEntity());
        }
    }
}