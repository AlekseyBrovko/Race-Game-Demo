public class MoneyQuestReward : IQuestReward 
{
    public MoneyQuestReward(int moneyValue)
    {
        MoneyValue = moneyValue;
    }

    public int MoneyValue { get; private set; }
}