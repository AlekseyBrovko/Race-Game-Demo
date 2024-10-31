using Client;
using Leopotam.EcsLite;

public class QuestCheckpointMb : QuestCheckpoint
{
    private EcsPool<QuestCheckpointEvent> _checkpointPool;
    
    public override void Init(GameState state, QuestChapter questChapter)
    {
        base.Init(state, questChapter);

        _checkpointPool = _state.EcsWorld.GetPool<QuestCheckpointEvent>();
    }

    protected override void OnTriggerExecute()
    {
        ref var checkpointComp = ref _checkpointPool.Add(_state.EcsWorld.NewEntity());
        checkpointComp.CurrentQuest = _questChapter.CurrentQuest;
    }
}