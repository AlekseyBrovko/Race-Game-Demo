public class DataForOpenLosePanel : IOpenPanelData
{
    public DataForOpenLosePanel(Enums.LoseType loseType)
    {
        LoseType = loseType;
    }

    public Enums.LoseType LoseType { get; private set; }
}