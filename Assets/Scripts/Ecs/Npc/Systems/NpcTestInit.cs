using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class NpcTestInit : IEcsInitSystem
    {
        private EcsWorldInject _world = default;
        private EcsPoolInject<NpcInitEvent> _initPool = default;

        public void Init(EcsSystems systems)
        {
            GameObject[] npcGos = GameObject.FindGameObjectsWithTag("Npc");
            foreach (var npc in npcGos)
            {
                ref var npcInitComp = ref _initPool.Value.Add(_world.Value.NewEntity());
                npcInitComp.NpcGo = npc;
                npcInitComp.NpcMb = npc.GetComponent<INpcMb>();
            }
        }
    }
}