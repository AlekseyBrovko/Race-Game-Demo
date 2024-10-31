using UnityEngine;

[CreateAssetMenu(fileName = "Simple Finish Chapter", menuName = "Configs/Quests/Chapters/Simple Finish Quest Chapter")]
public class SimpleFinishChapter : QuestChapter
{
    [SerializeField] private Vector3 _checkpointPosition;
    private QuestFinishPointMb _finishMb;

    public override void EnterChapter() =>
        _finishMb = _state.QuestsConfig.SpawnFinishPoint(this, _checkpointPosition);

    public override void FinishChapter() =>
        Destroy(_finishMb.gameObject);
}