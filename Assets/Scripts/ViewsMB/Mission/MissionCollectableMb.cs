using Client;
using Leopotam.EcsLite;
using UnityEngine;

public class MissionCollectableMb : MonoBehaviour
{
    public int Entity { get; private set; }
    public int MissionPartId { get; private set; }
    public int PointId { get; private set; }

    private EcsWorld _world;
    private EcsPool<MissionCollectEvent> _collectEventPool;
    private bool _hasTrigger;
    
    public void Init(GameState state, int missionPartId, int itemId, int entity)
    {
        _world = state.EcsWorld;
        MissionPartId = missionPartId;
        PointId = itemId;
        Entity = entity;

        _collectEventPool = _world.GetPool<MissionCollectEvent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTrigger)
            return;

        HandleCheckpoint();
    }

    private void HandleCheckpoint()
    {
        _hasTrigger = true;
        ref var checkpointComp = ref _collectEventPool.Add(_world.NewEntity());
        checkpointComp.MissionPartId = MissionPartId;
        checkpointComp.CollectableEntity = Entity;
        checkpointComp.Position = transform.position;
    }
}