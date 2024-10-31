using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    struct NpcSpawnerComp
    {
        //old
        public SpawnerMb SpawnPointMb;
        public Transform Spawner;

        //new
        public string ZoneName;
        public Transform[] SpawnPoints;
        public GameObjectsPool[] GameObjectsPools;
        public List<ISpawned> SpawnedMbs;   //npc
    }
}