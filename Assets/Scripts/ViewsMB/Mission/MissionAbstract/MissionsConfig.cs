using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "MissionsConfig", menuName = "Configs/MissionsConfig", order = 2)]
public class MissionsConfig : ScriptableObject
{
    [field: SerializeField] public MissionBase[] Missions;

    [field: SerializeField] public MissionBase[] MissionsOnTopGame;

    [field: SerializeField] public GameObject MissionPoint;
    [field: SerializeField] public GameObject FinishMissionPoint;

    public MissionBase GetFirstMission() =>
        Missions[0];

    public MissionBase GetMissionById(int id)
    {
        MissionBase result = null;
        result = Missions.FirstOrDefault(x => x.Id == id);
        if (result == null)
            result = MissionsOnTopGame.FirstOrDefault(x => x.Id == id);
        return result;
    }
        

    public bool IsLastMission(int id)
    {
        var mission = GetMissionById(id);
        int index = Array.IndexOf(Missions, mission);
        if (index == Missions.Length - 1)
        {
            Debug.Log("IsLastMission = true");
            return true;
        }   
        
        Debug.Log("IsLastMission = false");
        return false;
    }

    public MissionBase GetNextMission(int id)
    {
        var mission = GetMissionById(id);
        int index = Array.IndexOf(Missions, mission);
        index++;

        if (index >= Missions.Length)
        {
            Debug.LogWarning("Миссии кончились взяли из списка топ");
            return mission = MissionsOnTopGame[Random.Range(0, MissionsOnTopGame.Length)];
        }

        return Missions[index];
    }

    public MissionBase GetMissionOnAllComplete(int previousMissionId)
    {
        if (MissionsOnTopGame.Length > 1)
        {
            List<MissionBase> tempMissions = new List<MissionBase>(MissionsOnTopGame);
            MissionBase prevMission = GetMissionById(previousMissionId);
            tempMissions.Remove(prevMission);
            return tempMissions[Random.Range(0, tempMissions.Count)];
        }
        return MissionsOnTopGame[0];
    }
}