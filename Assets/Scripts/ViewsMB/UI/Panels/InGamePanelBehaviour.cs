using Client;
using DG.Tweening;
using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class InGamePanelBehaviour : PanelBase, IMainPanel, IMoneyPanel
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private GameObject _fuelIcon;

    [SerializeField] private Text _moneyText;
    [SerializeField] private Text _speedText;
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private Image _fuelBarImage;

    [SerializeField] private Image _hpItemIcon;
    [SerializeField] private Image _fuelItemIcon;
    [SerializeField] private Image _outOfFuelImage;

    [SerializeField] private GameObject _flipButton;

    private List<Sequence> _sequences = new List<Sequence>();

    private EcsWorld _world;
    private EcsPool<PauseButtonEvent> _pausePool;
    private EcsPool<InGamePanelComp> _inGamePanelPool;
    private EcsPool<MoneyPanelComp> _moneyPanelPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _world = _state.EcsWorld;
        _pausePool = _world.GetPool<PauseButtonEvent>();

        PreparePanelComponents();
        PreparePanelForDevice();
        PreparePanelForGameMode();

        _pauseButton.onClick.AddListener(() => OnPauseButtonClick());
        _canvasMb.ShowPanelById(PanelsIdHolder.MinimapPanelId);
    }

    private void PreparePanelComponents()
    {
        _inGamePanelPool = _world.GetPool<InGamePanelComp>();
        ref var inGamePanelComp = ref _inGamePanelPool.Add(_state.InterfaceEntity);
        inGamePanelComp.InGamePanelMb = this;

        _moneyPanelPool = _world.GetPool<MoneyPanelComp>();
        ref var moneyComp = ref _moneyPanelPool.Add(_state.InterfaceEntity);
        moneyComp.MoneyPanelMb = this;
        ShowMoney();
    }

    private void PreparePanelForDevice()
    {
        _flipButton.gameObject.SetActive(false);
        if (_state.IsMobilePlatform)
        {
            _canvasMb.ShowPanelById(PanelsIdHolder.AndroidInputPanelId);
            _flipButton.gameObject.SetActive(true);
        }
    }

    private void PreparePanelForGameMode()
    {
        if (SessionContextHolder.Instance.GameModeType == GameModType.Story)
            _canvasMb.ShowPanelById(PanelsIdHolder.MissionPanelId);
    }

    public void ShowMoney() =>
        _moneyText.text = _state.PlayerMoneyScore.ToString();

    public void ShowSpeed(float speed) =>
        _speedText.text = speed.ToString();

    public void ShowHealth(float sliderValue, bool showAnimation)
    {
        _healthBarImage.fillAmount = sliderValue;
        if (showAnimation)
            ShowScaleAnimation(_hpItemIcon.transform);
    }   

    public void ShowFuelValue(float sliderValue, bool showAnimation)
    {
        _fuelBarImage.fillAmount = sliderValue;
        HandleFuelLowLevel(sliderValue);

        if (showAnimation)
            ShowLowFuelAnimation(_fuelItemIcon.transform);
    }
        
    private void HandleFuelLowLevel(float sliderValue)
    {
        if (sliderValue < 0.05f)
            ShowOutOfFuel();
        else
            HideOutOfFuel();
    }

    private void ShowOutOfFuel()
    {
        _fuelItemIcon.gameObject.SetActive(false);
        _outOfFuelImage.gameObject.SetActive(true);
    }

    private void HideOutOfFuel()
    {
        _fuelItemIcon.gameObject.SetActive(true);
        _outOfFuelImage.gameObject.SetActive(false);
    }

    private void OnPauseButtonClick()
    {
        _canvasMb.PlayClickSound();
        _pausePool.Add(_world.NewEntity());
    }

    public override void CleanupPanel()
    {
        _inGamePanelPool.Del(_state.InterfaceEntity);
        _moneyPanelPool.Del(_state.InterfaceEntity);
    }

    //TODO можно в мини сервис зарефакторить
    private float _scaleDuration = 0.2f;
    private Vector3 _defaultScale = new Vector3(1f, 1f, 1f);
    private Vector3 _scaleValue = new Vector3(1.2f, 1.2f, 1.2f);
    private void ShowScaleAnimation(Transform transform)
    {
        Sequence sequence = DOTween.Sequence();
        AddNewSequence(sequence);
        sequence.Append(transform.DOScale(_scaleValue, _scaleDuration / 2f));
        sequence.Append(transform.DOScale(_defaultScale, _scaleDuration / 2f));
        sequence.AppendCallback(() => RemoveSequence(sequence));
    }

    private void ShowLowFuelAnimation(Transform transform)
    {
        Sequence sequence = DOTween.Sequence();
        AddNewSequence(sequence);
        sequence.Append(transform.DOScale(_scaleValue, _scaleDuration / 2f));
        sequence.Append(transform.DOScale(_defaultScale, _scaleDuration / 2f));
        sequence.Append(transform.DOScale(_scaleValue, _scaleDuration / 2f));
        sequence.Append(transform.DOScale(_defaultScale, _scaleDuration / 2f));
        sequence.Append(transform.DOScale(_scaleValue, _scaleDuration / 2f));
        sequence.Append(transform.DOScale(_defaultScale, _scaleDuration / 2f));
        sequence.Append(transform.DOScale(_scaleValue, _scaleDuration / 2f));
        sequence.Append(transform.DOScale(_defaultScale, _scaleDuration / 2f));
        sequence.AppendCallback(() => RemoveSequence(sequence));
    }

    private void AddNewSequence(Sequence sequence) =>
        _sequences.Add(sequence);

    private void RemoveSequence(Sequence sequence) =>
        _sequences.Remove(sequence);

    private void OnDestroy()
    {
        foreach (var sequence in _sequences)
            sequence.Kill(false);
    }
}