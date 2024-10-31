using Client;
using UnityEngine;

public abstract class QuestChapter : ScriptableObject
{
    public Quest CurrentQuest { get; private set; }
    protected GameState _state;

    public virtual void Init(GameState state, Quest quest)
    {
        _state = state;
        CurrentQuest = quest;
    }

    public virtual void EnterChapter() { }
    public virtual void FinishChapter() { }
    public virtual void FailChapter() { }
}