public class SettingsGaragePanel : SettingsPanelBehaviour, IPopupPanel
{
    public override void OnBackButtonClick()
    {
        _canvasMb.PlayClickSound();

        _canvasMb.DestroyPanelById(this.Id);
    }
}
