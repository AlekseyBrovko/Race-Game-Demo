using Client;
using Leopotam.EcsLite;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public abstract class Quest : ScriptableObject
{
    public QuestChapter CurrentQuestChapter { get; private set; }
    public abstract QuestStartType QuestStartType { get; }
    public abstract IQuestReward QuestReward { get; }
    public bool CanRepeateQuest => _canRepeate;
    public string Id => _id;

    [SerializeField] public QuestState QuestState;
    [SerializeField] protected string _id;
    [SerializeField] protected bool _canRepeate;
    [SerializeField] protected List<QuestChapter> _questChapters = new List<QuestChapter>();

    [SerializeField] protected string[] _questsThatWillBecomeAvailable;

    protected GameState _state;
    protected EcsPool<QuestFinishEvent> _finishPool;

    public virtual void Init(GameState state)
    {
        _state = state;
        _finishPool = _state.EcsWorld.GetPool<QuestFinishEvent>();
    }

    public virtual void CheckAvailable() { }       //к примеру, нужно сколько-то набрать чего-нибудь, периодически проверять

    public virtual void ShowAnailableState() { }

    public virtual void HideAvailableState() { }


    public virtual void StartQuest()
    {
        CurrentQuestChapter = _questChapters[0];
        StartCurrentQuestChapter();
    }

    public virtual void NextChapter()
    {
        CurrentQuestChapter.FinishChapter();
        if (CurrentQuestChapter == _questChapters.Last())
        {
            EndQuest();
        }
        else
        {
            int currentChapterIndex = _questChapters.IndexOf(CurrentQuestChapter);
            CurrentQuestChapter = _questChapters[currentChapterIndex + 1];
            StartCurrentQuestChapter();
        }
    }

    public virtual void EndQuest()
    {
        if (_canRepeate)
            QuestState = QuestState.Available;
        else
            QuestState = QuestState.Complete;

        ref var finishComp = ref _finishPool.Add(_state.EcsWorld.NewEntity());
        finishComp.FinishedQuest = this;
        finishComp.QuestReward = QuestReward;
    }

    public virtual void FailQuest() =>
        CurrentQuestChapter.FailChapter();

    private void StartCurrentQuestChapter()
    {
        CurrentQuestChapter.Init(_state, this);
        CurrentQuestChapter.EnterChapter();
    }

    public virtual void SaveQuestProgress() { }

    public virtual void LoadQuestProgress() { }

    #region Editor
    public virtual void AddNewChapter() { }

    public virtual void RemoveChapter() { }
    #endregion
}
