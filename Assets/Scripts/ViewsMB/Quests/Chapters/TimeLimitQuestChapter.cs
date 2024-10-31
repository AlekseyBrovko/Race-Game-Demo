using Client;
using Leopotam.EcsLite;
using UnityEngine;

[CreateAssetMenu(fileName = "Simple CheckPoint Chapter", menuName = "Configs/Quests/Chapters/Time Limit Quest Chapter")]
public class TimeLimitQuestChapter : QuestChapter
{
    [SerializeField] private float _timeForChapter;
    [SerializeField] private Vector3 _finishPosition;

    private QuestCheckpointMb _checkpointMb;

    private EcsPool<StartTimeLimitChapterEvent> _startChapterPool;
    private EcsPool<StopTimeLimitChapterEvent> _stopChapterPool;

    public override void Init(GameState state, Quest quest)
    {
        base.Init(state, quest);

        _checkpointMb = _state.QuestsConfig.SpawnCheckpoint(this, _finishPosition);

        _startChapterPool = _state.EcsWorld.GetPool<StartTimeLimitChapterEvent>();
        _stopChapterPool = _state.EcsWorld.GetPool<StopTimeLimitChapterEvent>();
    }

    public override void EnterChapter()
    {
        ref var startComp = ref _startChapterPool.Add(_state.EcsWorld.NewEntity());
        startComp.CurrentQuest = CurrentQuest;
        startComp.CurrentQuestChapter = this;
        startComp.Time = _timeForChapter;
    }

    public override void FinishChapter()
    {
        Destroy(_checkpointMb.gameObject);

        ref var stopComp = ref _stopChapterPool.Add(_state.EcsWorld.NewEntity());
        stopComp.CurrentQuest = CurrentQuest;
        stopComp.CurrentQuestChapter = this;
    }

    public override void FailChapter()
    {
        Destroy(_checkpointMb.gameObject);
    }
}