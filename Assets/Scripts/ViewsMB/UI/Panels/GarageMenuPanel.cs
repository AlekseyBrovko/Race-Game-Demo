using Client;
using UnityEngine;
using UnityEngine.UI;

public class GarageMenuPanel : PanelBase, IPopupPanel
{
    [SerializeField] private Button _backToGarageButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _backToGarageButton.onClick.AddListener(() => OnBackToGarageButtonClick());
        _settingsButton.onClick.AddListener(() => OnSettingsButtonClick());
        _quitButton.onClick.AddListener(() => OnQuitButtonClick());

        if (_state.IsWebGl)
            _quitButton.gameObject.SetActive(false);
    }

    private void OnBackToGarageButtonClick()
    {
        _canvasMb.PlayClickSound();
        _canvasMb.DestroyPanelById(this.Id);
    }
    
    private void OnSettingsButtonClick()
    {
        _canvasMb.PlayClickSound();
        _canvasMb.ShowPanelById(PanelsIdHolder.SettingsGaragePanelId);
    }
        
    private void OnQuitButtonClick()
    {
        _canvasMb.PlayClickSound();
        _canvasMb.ShowPanelById(PanelsIdHolder.ExitPopupPanelId);
    }   
}