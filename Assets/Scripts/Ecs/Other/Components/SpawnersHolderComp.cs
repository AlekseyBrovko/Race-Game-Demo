using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    struct SpawnersHolderComp
    {
        public List<SpawnerMb> ActiveSpawners;
        public Dictionary<Transform, SpawnerMb> Spawners;
        public List<SpawnerPointCalculation> SpawnersCalculations;
    }

    public class SpawnerPointCalculation
    {
        public Transform Spawner { get; private set; }
        public float SqrDistance { get; set; }

        public SpawnerPointCalculation(Transform spawner)
        {
            Spawner = spawner;
        }
    }
}