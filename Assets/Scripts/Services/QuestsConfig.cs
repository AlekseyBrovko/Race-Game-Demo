using Client;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestsConfig", menuName = "Configs/QuestsConfig")]
public class QuestsConfig : ScriptableObject
{
    [SerializeField] private Quest[] _quests;
    [SerializeField] private QuestPointsPrefabs _questPointsPrefabs;

    private GameState _state;

    public void InitQuests(GameState state)
    {
        _state = state;
        foreach (var quest in _quests)
            quest.Init(state);
    }

    public Quest GetQuestById(string id)
    {
        Quest result = _quests.FirstOrDefault(x => x.Id == id);
        return result;
    }

    public List<Quest> GetAvailableFromPointsQuests()
    {
        List<Quest> result = new List<Quest>();
        foreach (Quest quest in _quests)
        {
            if (quest.QuestStartType == Enums.QuestStartType.FromPoint &&
                quest.QuestState == Enums.QuestState.Available)
                result.Add(quest);
        }
        return result;
    }

    public QuestStartPointMb SpawnStartPoint(Quest quest, Vector3 position)
    {
        QuestStartPointMb pointMb = Instantiate(_questPointsPrefabs.StartPointPrefab, position, Quaternion.identity);
        pointMb.Init(_state, quest);
        return pointMb;
    }

    public QuestCheckpointMb SpawnCheckpoint(QuestChapter questChapter, Vector3 position)
    {
        QuestCheckpointMb pointMb = Instantiate(_questPointsPrefabs.CheckpointPrefab, position, Quaternion.identity);
        pointMb.Init(_state, questChapter);
        return pointMb;
    }

    public QuestFinishPointMb SpawnFinishPoint(QuestChapter questChapter, Vector3 position)
    {
        QuestFinishPointMb pointMb = Instantiate(_questPointsPrefabs.FinishPointPrefab, position, Quaternion.identity);
        pointMb.Init(_state, questChapter);
        return pointMb;
    }


    [Serializable]
    private class QuestPointsPrefabs
    {
        [SerializeField] public QuestStartPointMb StartPointPrefab;
        [SerializeField] public QuestCheckpointMb CheckpointPrefab;
        [SerializeField] public QuestFinishPointMb FinishPointPrefab;
    }
}