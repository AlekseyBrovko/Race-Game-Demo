public class SettingsInGamePanel : SettingsPanelBehaviour, IMainPanel
{
    public override void OnBackButtonClick()
    {
        _canvasMb.PlayClickSound();

        if (_state.SceneType == Enums.SceneType.PlayScene)
            _canvasMb.ShowPanelById(PanelsIdHolder.PausePanelId);
        else if (_state.SceneType == Enums.SceneType.StartScene)
            _canvasMb.ShowPanelById(PanelsIdHolder.MainMenuPanelId);
    }
}