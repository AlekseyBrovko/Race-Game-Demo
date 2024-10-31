using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MissionSaveData
{
    public MissionSaveData(MissionBase mission, bool allMissionsComplete)
    {
        MissionId = mission.Id;
        AllMissionComplete = allMissionsComplete;

        foreach (var missionPart in mission.MissionParts)
        {
            switch (missionPart.MissionType)
            {
                case Enums.MissionType.CollectOneByOne:
                    CollectPartSaveData collectData = new CollectPartSaveData(missionPart.Id);
                    CollectPartSaveDatas.Add(collectData);
                    AllMissionsParts.Add(collectData);
                    break;

                case Enums.MissionType.CollectAllOnMap:
                    CollectPartSaveData collectDataAll = new CollectPartSaveData(missionPart.Id);
                    CollectPartSaveDatas.Add(collectDataAll);
                    AllMissionsParts.Add(collectDataAll);
                    break;

                case Enums.MissionType.RideByCheckpointsOneByOne:
                    RidePartSaveData rideData = new RidePartSaveData(missionPart.Id);
                    RidePartSaveDatas.Add(rideData);
                    AllMissionsParts.Add(rideData);
                    break;

                case Enums.MissionType.RideByCheckpointsAllOnMap:
                    RidePartSaveData rideAllData = new RidePartSaveData(missionPart.Id);
                    RidePartSaveDatas.Add(rideAllData);
                    AllMissionsParts.Add(rideAllData);
                    break;

                case Enums.MissionType.Kill:
                    KillPartSaveData killData = new KillPartSaveData(missionPart.Id);
                    KillPartSaveDatas.Add(killData);
                    AllMissionsParts.Add(killData);
                    break;

                case Enums.MissionType.Custom:
                    CustomPartSaveData customData = new CustomPartSaveData(missionPart.Id);
                    CustomPartSaveDatas.Add(customData);
                    AllMissionsParts.Add(customData);
                    break;

                case Enums.MissionType.KillOnZone:
                    KillPartSaveData killOnZoneData = new KillPartSaveData(missionPart.Id);
                    KillPartSaveDatas.Add(killOnZoneData);
                    AllMissionsParts.Add(killOnZoneData);
                    break;
            }
        }
    }

    public int Version;
    public int MissionId;
    public bool AllMissionComplete;

    public List<MissionPartSaveDataBase> AllMissionsParts = new List<MissionPartSaveDataBase>();

    public List<KillPartSaveData> KillPartSaveDatas = new List<KillPartSaveData>();
    public List<RidePartSaveData> RidePartSaveDatas = new List<RidePartSaveData>();
    public List<CollectPartSaveData> CollectPartSaveDatas = new List<CollectPartSaveData>();
    public List<CustomPartSaveData> CustomPartSaveDatas = new List<CustomPartSaveData>();

    public MissionPartSaveDataBase GetMissionPartById(int id) =>
        AllMissionsParts.FirstOrDefault(x => x.PartId == id);

    public int GetKillPartProgress(int id) =>
        KillPartSaveDatas.FirstOrDefault(x => x.PartId == id).Progress;

    public void PrepareAfterLoad()
    {
        AllMissionsParts.Clear();

        foreach (var data in KillPartSaveDatas)
            AllMissionsParts.Add(data);

        foreach (var data in RidePartSaveDatas)
            AllMissionsParts.Add(data);

        foreach (var data in CollectPartSaveDatas)
            AllMissionsParts.Add(data);

        foreach (var data in CustomPartSaveDatas)
            AllMissionsParts.Add(data);
    }

    public void LogSaveDatas()
    {
        Debug.Log("MissionId = " + MissionId);
        Debug.Log("KillPartSaveDatas.Count = " + KillPartSaveDatas.Count);
        Debug.Log("RidePartSaveDatas.Count = " + RidePartSaveDatas.Count);
        Debug.Log("CollectPartSaveDatas.Count = " + CollectPartSaveDatas.Count);
        Debug.Log("CustomPartSaveDatas.Count = " + CustomPartSaveDatas.Count);
    }
}

[Serializable]
public class KillPartSaveData : MissionPartSaveDataBase
{
    public int Progress;

    public KillPartSaveData(int id) : base(id)
    {
    }
}

[Serializable]
public class RidePartSaveData : MissionPartSaveDataBase
{
    public int CheckpointsCounter;

    public RidePartSaveData(int id) : base(id)
    {
    }
}

[Serializable]
public class CollectPartSaveData : MissionPartSaveDataBase
{
    public int Progress;

    public CollectPartSaveData(int id) : base(id)
    {
    }
}

[Serializable]
public class CustomPartSaveData : MissionPartSaveDataBase
{
    public CustomPartSaveData(int id) : base(id)
    {
    }
}

[Serializable]
public class MissionPartSaveDataBase
{
    public MissionPartSaveDataBase(int id)
    {
        PartId = id;
    }

    public int PartId;
    public bool Complete;
    public bool Failed;
    public float Timer;
}