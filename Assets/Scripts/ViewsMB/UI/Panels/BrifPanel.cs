using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class BrifPanel : PanelBase, IPopupPanel
{
    [SerializeField] private Text _missionNameText;
    [SerializeField] private Text _missionText;
    [SerializeField] private Text _rewardValueText;
    [SerializeField] private Image _avatarImage;

    [SerializeField] private Button _continueButton;

    private BrifMaterials _brifMaterials;

    private EcsPool<LocalizationComp> _localizationPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _localizationPool = state.EcsWorld.GetPool<LocalizationComp>();
        _brifMaterials = (BrifMaterials)openData;
        _continueButton.onClick.AddListener(() => OnBackButtonClick());
        BuildPanel();
    }

    public void BuildPanel()
    {
        if (_brifMaterials.AvatarSprite == null)
            _avatarImage.gameObject.SetActive(false);
        else
            _avatarImage.sprite = _brifMaterials.AvatarSprite;

        _rewardValueText.text = _state.CurrentMission.MoneyReward.ToString();

        GetLocalizedTexts();
    }

    private void GetLocalizedTexts()
    {
        ref var localizationComp = ref _localizationPool.Get(_state.InterfaceEntity);
        _missionNameText.text = localizationComp.MissionNamesCurrentTable[_brifMaterials.LocalizationTag].Value;
        _missionText.text = localizationComp.BrifCurrentTable[_brifMaterials.LocalizationTag].Value;
    }

    public override void OnBackButtonClick()
    {
        _canvasMb.PlayClickSound();
        _canvasMb.DestroyPanelById(this.Id);
        _state.StartPlaySystems();
    }
}