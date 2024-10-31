public class DataForOpenMissionCounterPanel : IOpenPanelData
{
    public DataForOpenMissionCounterPanel(string counterName, int amount)
    {
        CounterName = counterName;
        Amount = amount;
    }
    public string CounterName { get; set; }
    public int Amount { get; set; }
}