namespace Client
{
    public struct QuestStartEvent
    {
        public Quest QuestToStart;
    }

    public struct QuestCheckpointEvent
    {
        public Quest CurrentQuest;
    }

    public struct QuestFinishEvent
    {
        public IQuestReward QuestReward;
        public Quest FinishedQuest;
    }

    public struct QuestFailedEvent
    {
        public Quest FailedQuest;
        public QuestChapter QuestChapter;
    }

    public struct StartTimeLimitChapterEvent
    {
        public Quest CurrentQuest;
        public QuestChapter CurrentQuestChapter;
        public float Time;
    }

    public struct StopTimeLimitChapterEvent 
    {
        public Quest CurrentQuest;
        public QuestChapter CurrentQuestChapter;
    }

    public struct QuestForTimeComp 
    {
        public Quest CurrentQuest;
        public QuestChapter CurrentQuestChapter;
        public float TimeDuration;
        public float Timer;
    }

    public struct StartKillChapterEvent
    {
        public Quest CurrentQuest;
        public QuestChapter KillChapter;
        public string NameOfMonster;
        public int Amount;
    }

    public struct StopKillChapterEvent
    {
        public Quest CurrentQuest;
        public QuestChapter KillChapter;
    }

    public struct KillQuestChapterComp
    {
        public Quest CurrentQuest;
        public QuestChapter KillChapter;
        public string NameOfMonster;
        public int Amount;
        public int Counter;
    }
}