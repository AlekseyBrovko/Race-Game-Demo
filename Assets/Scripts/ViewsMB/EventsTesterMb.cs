using Client;
using Leopotam.EcsLite;
using UnityEngine;

public class EventsTesterMb : MonoBehaviour, IInitable
{
    private EcsWorld _world;
    private EcsPool<TestLogEvent> _testPool;

    public void Init(GameState state)
    {
        _world = state.EcsWorld;
        _testPool = state.EcsWorld.GetPool<TestLogEvent>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("пробел");
            _testPool.Add(_world.NewEntity());
        }
    }
}
