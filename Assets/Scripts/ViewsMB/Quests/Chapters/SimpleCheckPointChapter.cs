using UnityEngine;

[CreateAssetMenu (fileName = "Simple CheckPoint Chapter", menuName = "Configs/Quests/Chapters/Simple Checkpoint Quest Chapter")]
public class SimpleCheckPointChapter : QuestChapter
{
    [SerializeField] private Vector3 _checkpointPosition;
    private QuestCheckpointMb _checkpointMb;

    public override void EnterChapter() =>
        _checkpointMb = _state.QuestsConfig.SpawnCheckpoint(this, _checkpointPosition);
    
    public override void FinishChapter() =>
        Destroy(_checkpointMb.gameObject);
}
