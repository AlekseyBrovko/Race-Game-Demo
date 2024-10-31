using Client;
using Leopotam.EcsLite;
using UnityEngine;

[CreateAssetMenu(fileName = "Kill Zombie Chapter", menuName = "Configs/Quests/Chapters/Kill Zombie Quest Chapter")]
public class KillZombieQuestChapter : QuestChapter
{
    [SerializeField] private string _nameOfMonster;
    [SerializeField] private int _killAmount;

    private EcsPool<StartKillChapterEvent> _startKillQuestPool;

    public override void Init(GameState state, Quest quest)
    {
        base.Init(state, quest);
        _startKillQuestPool = state.EcsWorld.GetPool<StartKillChapterEvent>();
    }

    public override void EnterChapter()
    {
        ref var startComp = ref _startKillQuestPool.Add(_state.EcsWorld.NewEntity());
        startComp.CurrentQuest = CurrentQuest;
        startComp.KillChapter = this;
        startComp.Amount = _killAmount;
        startComp.NameOfMonster = _nameOfMonster;
    }   
    
    public override void FinishChapter()
    {

    }

    public override void FailChapter()
    {

    }
}