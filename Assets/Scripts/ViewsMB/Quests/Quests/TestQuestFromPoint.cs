using UnityEngine;
using static Enums;

[CreateAssetMenu (fileName = "Quest From Point", menuName = "Configs/Quests/Test Quest From Point")]
public class TestQuestFromPoint : Quest
{
    public override QuestStartType QuestStartType => QuestStartType.FromPoint;
    public override IQuestReward QuestReward => new MoneyQuestReward(_moneyReward);

    [SerializeField] private int _moneyReward;
    [SerializeField] private Vector3 _startPosition;

    private QuestStartPointMb _startPointMb;

    public override void ShowAnailableState() =>
        _startPointMb = _state.QuestsConfig.SpawnStartPoint(this, _startPosition);

    public override void HideAvailableState() =>
        Destroy(_startPointMb.gameObject);

    public override void StartQuest()
    {
        HideAvailableState();
        base.StartQuest();
    }
}