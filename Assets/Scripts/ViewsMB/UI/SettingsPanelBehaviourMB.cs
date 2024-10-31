using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leopotam.EcsLite;
using Client;
using DG.Tweening;

public class SettingsPanelBehaviourMB : MonoBehaviour
{
    #region ECS
    private EcsWorld _world;
    private GameState _state;
    private EcsPool<UISoundEvent> _soundPool;
    private EcsPool<CreditsEvent> _creditsPool;

    public void Init(GameState state, CanvasBehaviour canvasMB)
    {
        var world = state.EcsWorld;
        _world = world;
        _state = state;
        _canvasMb = canvasMB;
        _soundPool = _world.GetPool<UISoundEvent>();
        _creditsPool = _world.GetPool<CreditsEvent>();
    }
    #endregion

    private CanvasBehaviour _canvasMb;

    [SerializeField] private float _speed;
    private Vector2 _startPos;
    [SerializeField] private Vector2 _activePos;
    [SerializeField] private RectTransform _thisPanel;
    private bool _wasClicked = false;
    [SerializeField] private Image _musicButton;
    [SerializeField] private Image _vibroButton;
    [SerializeField] private Image _soundsButton;
    [SerializeField] private Sprite _musicOnImage;
    [SerializeField] private Sprite _musicOffImage;
    [SerializeField] private Sprite _vibroOnImage;
    [SerializeField] private Sprite _vibroOffImage;
    [SerializeField] private Sprite _soundOnImage;
    [SerializeField] private Sprite _soundOffImage;
    [SerializeField] private AudioBehaviourMB _audioBehaviour;
    [SerializeField] private GameObject _creditsPanel;

    void Awake()
    {
        _startPos = new Vector2(_thisPanel.anchoredPosition.x, _thisPanel.anchoredPosition.y);
    }

    public void OpenCredits()
    {
        _creditsPool.Add(_world.NewEntity());
        PlayClickSound();
    }

    public void OpenFromSystem()
    {
        _creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        _creditsPanel.SetActive(false);
        PlayClickSound();
    }

    public void MoveSettingsPanel()
    {
        PlayClickSound();
        //LightVibration();            
        if (!_wasClicked)
        {
            _thisPanel.DOAnchorPosX(_activePos.x, _speed, false);
            _wasClicked = true;
        }
        else
        {
            _thisPanel.DOAnchorPosX(_startPos.x, _speed, false);
            _wasClicked = false;
        }
    }

    public void ChangeVibration()
    {
        PlayClickSound();
        //LightVibration();
        switch (_state.IsOnVibration)
        {
            case true:
                _state.IsOnVibration = false;
                break;
            case false:
                _state.IsOnVibration = true;
                break;
        }
        _vibroButton.sprite = GetVibroButtonSprite(_state.IsOnVibration);
    }

    public void ChangeMusic()
    {
        PlayClickSound();
        //LightVibration();
        switch (_state.IsOnMusic)
        {
            case true:
                _state.IsOnMusic = false;
                _audioBehaviour.OffMusicVol();
                break;
            case false:
                _state.IsOnMusic = true;
                _audioBehaviour.OnMusicVol();
                break;
        }
        _musicButton.sprite = GetMusicButtonSprite(_state.IsOnMusic);
    }

    public void ChangeSound()
    {
        PlayClickSound();
        //LightVibration();
        switch (_state.IsOnMasterVolume)
        {
            case true:
                _state.IsOnMasterVolume = false;
                _audioBehaviour.OffSoundsVol();
                break;
            case false:
                _state.IsOnMasterVolume = true;
                _audioBehaviour.OnSoundsVol();
                break;
        }
        _soundsButton.sprite = GetSoundsButtonSprite(_state.IsOnMasterVolume);
    }

    public void GetOnStartButtonSprite()
    {
        _vibroButton.sprite = GetVibroButtonSprite(_state.IsOnVibration);
        _soundsButton.sprite = GetSoundsButtonSprite(_state.IsOnMasterVolume);
        _musicButton.sprite = GetMusicButtonSprite(_state.IsOnMusic);
    }
    private Sprite GetSoundsButtonSprite(bool value)
    {
        Sprite sprite = null;
        switch (value)
        {
            case true:
                sprite = _soundOnImage;
                break;
            case false:
                sprite = _soundOffImage;
                break;
        }
        return sprite;
    }

    private Sprite GetMusicButtonSprite(bool value)
    {
        Sprite sprite = null;
        switch (value)
        {
            case true:
                sprite = _musicOnImage;
                break;
            case false:
                sprite = _musicOffImage;
                break;
        }
        return sprite;
    }

    private Sprite GetVibroButtonSprite(bool value)
    {
        Sprite sprite = null;
        switch (value)
        {
            case true:
                sprite = _vibroOnImage;
                break;
            case false:
                sprite = _vibroOffImage;
                break;
        }
        return sprite;
    }
    /*
            private void LightVibration()
            {
                if(!_vibrationEvent.Has(_state.VibrationEntity))
                {
                    ref var vibroComp = ref _vibrationEvent.Add(_state.VibrationEntity);
                    vibroComp.Vibration = VibrationEvent.VibrationType.LightImpack;
                }
            }
    */
    private void PlayClickSound()
    {
        ref var soundComp = ref _soundPool.Add(_world.NewEntity());
        soundComp.Sound = Enums.SoundEnum.ClickSound;
    }
}
