using Client;
using Leopotam.EcsLite;
using UnityEngine;

public class QuestStartPointMb : MonoBehaviour
{
    private GameState _state;
    private Quest _quest;
    private EcsPool<QuestStartEvent> _startPool;
    private bool _isInteractable;

    public void Init(GameState state, Quest quest)
    {
        _state = state;
        _quest = quest;
        _startPool = _state.EcsWorld.GetPool<QuestStartEvent>();
        _isInteractable = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerCar carMb))
        {
            if (_isInteractable)
            {
                ref var startComp = ref _startPool.Add(_state.EcsWorld.NewEntity());
                startComp.QuestToStart = _quest;
                _isInteractable = false;
            }
        }
    }
}