using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

public class CarStatsPanel : MonoBehaviour
{
    [SerializeField] private Text _carNameText;

    [SerializeField] private GameObject _maxSpeedIcon;
    [SerializeField] private GameObject _horcePowerIcon;
    [SerializeField] private GameObject _hpLevelIcon;
    [SerializeField] private GameObject _fuelLevelIcon;

    [SerializeField] private CarStatIcon _maxSpeedIconMb;
    [SerializeField] private CarStatIcon _horcePowerIconMb;
    [SerializeField] private CarStatIcon _hpLevelIconMb;
    [SerializeField] private CarStatIcon _fuelLevelIconMb;

    [SerializeField] private Text _maxSpeedPriceText;
    [SerializeField] private Text _horcePowerPriceText;
    [SerializeField] private Text _hpPriceText;
    [SerializeField] private Text _fuelPriceText;

    [SerializeField] private Button _upgradeMaxSpeedButton;
    [SerializeField] private Button _upgradeHorcePowerButton;
    [SerializeField] private Button _upgradeHpButton;
    [SerializeField] private Button _upgradeFuelButton;

    private LocalizedStringTable _carsTable;
    private StringTable _currentLocalizeTable;
    private PlayerCarSo _carSo;
    private SavedCar _savedCar;

    private EcsWorld _world;
    private GameState _state;
    private EcsPool<CarUpgradeBuyEvent> _upgradePool;
    private EcsPool<LocalizationComp> _localizationPool;
    public void Init(GameState state, LocalizedStringTable carsTable)
    {
        _state = state;
        _world = _state.EcsWorld;
        _upgradePool = _world.GetPool<CarUpgradeBuyEvent>();
        _localizationPool = _world.GetPool<LocalizationComp>();
        _carsTable = carsTable;
        _currentLocalizeTable = _carsTable.GetTable();

        InitButtons();
    }

    public void ShowStatsIcons(PlayerCarSo carSo, SavedCar savedCar)
    {
        _carSo = carSo;
        _savedCar = savedCar;

        _carNameText.text = GetCarName(carSo);

        if (savedCar != null)
            ShowBuyedState(carSo, savedCar);
        else
            ShowNotBuyedState();
    }

    private string GetCarName(PlayerCarSo carSo) =>
        _currentLocalizeTable[carSo.Id].LocalizedValue;

    private void ShowNotBuyedState()
    {
        _maxSpeedIcon.SetActive(false);
        _horcePowerIcon.SetActive(false);
        _hpLevelIcon.SetActive(false);
        _fuelLevelIcon.SetActive(false);
    }

    private void ShowBuyedState(PlayerCarSo carSo, SavedCar savedCar)
    {
        _maxSpeedIcon.SetActive(true);
        _horcePowerIcon.SetActive(true);
        _hpLevelIcon.SetActive(true);
        _fuelLevelIcon.SetActive(true);

        ShowLevels(savedCar);
        ShowPrices();
    }

    private void ShowLevels(SavedCar savedCar)
    {
        _maxSpeedIconMb.ShowLevelValue(savedCar.MaxSpeedLevel);
        _horcePowerIconMb.ShowLevelValue(savedCar.HorcePowerLevel);
        _hpLevelIconMb.ShowLevelValue(savedCar.HpLevel);
        _fuelLevelIconMb.ShowLevelValue(savedCar.FuelLevel);
    }

    private void ShowPrices()
    {
        int currentMaxSpeedLevel = _savedCar.MaxSpeedLevel;
        if (currentMaxSpeedLevel < _carSo.MaxSpeedLevels.Length - 1)
        {
            _upgradeMaxSpeedButton.interactable = true;
            int upgradeSpeedLevelPrice = _carSo.MaxSpeedLevels[_savedCar.MaxSpeedLevel + 1].Price;
            _maxSpeedPriceText.text = upgradeSpeedLevelPrice.ToString();
        }
        else
        {
            _upgradeMaxSpeedButton.interactable = false;
            //_maxSpeedPriceText.text = "Full";
            _maxSpeedPriceText.text = GetFullText();
        }

        int currentMotorPowerLevel = _savedCar.HorcePowerLevel;
        if (currentMotorPowerLevel < _carSo.HorcePowerLevels.Length - 1)
        {
            _upgradeHorcePowerButton.interactable = true;
            int upgradeMotorPowerPrice = _carSo.HorcePowerLevels[_savedCar.HorcePowerLevel + 1].Price;
            _horcePowerPriceText.text = upgradeMotorPowerPrice.ToString();
        }
        else
        {
            _upgradeHorcePowerButton.interactable = false;
            //_horcePowerPriceText.text = "Full";
            _horcePowerPriceText.text = GetFullText();
        }


        int currentHpLevel = _savedCar.HpLevel;
        if (currentHpLevel < _carSo.HpLevels.Length - 1)
        {
            _upgradeHpButton.interactable = true;
            int upgradeHpPrice = _carSo.HpLevels[_savedCar.HpLevel + 1].Price;
            _hpPriceText.text = upgradeHpPrice.ToString();
        }
        else
        {
            _upgradeHpButton.interactable = false;
            //_hpPriceText.text = "Full";
            _hpPriceText.text = GetFullText();
        }


        int currentFuelLevel = _savedCar.FuelLevel;
        if (currentFuelLevel < _carSo.FuelLevels.Length - 1)
        {
            _upgradeFuelButton.interactable = true;
            int upgradeFuelPrice = _carSo.FuelLevels[currentFuelLevel + 1].Price;
            _fuelPriceText.text = upgradeFuelPrice.ToString();
        }
        else
        {
            _upgradeFuelButton.interactable = false;
            //_fuelPriceText.text = "Full";
            _fuelPriceText.text = GetFullText();
        }
    }

    private void InitButtons()
    {
        _upgradeMaxSpeedButton.onClick.AddListener(() => OnUpdateMaxSpeedButtonClick());
        _upgradeHorcePowerButton.onClick.AddListener(() => OnUpdateHorcePowerButtonClick());
        _upgradeHpButton.onClick.AddListener(() => OnUpdateHpButtonClick());
        _upgradeFuelButton.onClick.AddListener(() => OnUpdateFuelButtonClick());
    }

    private void OnUpdateMaxSpeedButtonClick() =>
        UpgradeEvent(Enums.CarUpgradeType.MaxSpeed);

    private void OnUpdateHorcePowerButtonClick() =>
        UpgradeEvent(Enums.CarUpgradeType.MotorPower);

    private void OnUpdateHpButtonClick() =>
        UpgradeEvent(Enums.CarUpgradeType.Hp);

    private void OnUpdateFuelButtonClick() =>
        UpgradeEvent(Enums.CarUpgradeType.Fuel);

    private void UpgradeEvent(Enums.CarUpgradeType upgradeType)
    {
        ref var upgradeComp = ref _upgradePool.Add(_world.NewEntity());
        upgradeComp.UpgradeType = upgradeType;
    }

    private const string _fullLocalizationTag = "fullCarUpgrade";
    private string GetFullText()
    {
        ref var localizationComp = ref _localizationPool.Get(_state.InterfaceEntity);
        return localizationComp.UiCurrentTable[_fullLocalizationTag].Value;
    }
}