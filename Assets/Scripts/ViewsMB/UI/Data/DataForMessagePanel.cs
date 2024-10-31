public class DataForMessagePanel : IOpenPanelData
{
    public DataForMessagePanel(Enums.MessageTypeEnum messageType)
    {
        MessageType = messageType;
    }

    public DataForMessagePanel(Enums.MessageTypeEnum messageType, string message)
    {
        MessageType = messageType;
        MessageText = message;
    }

    public Enums.MessageTypeEnum MessageType;
    public string MessageText;
}