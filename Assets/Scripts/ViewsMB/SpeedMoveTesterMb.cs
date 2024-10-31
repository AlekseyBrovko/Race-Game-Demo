using Client;
using Leopotam.EcsLite;
using UnityEngine;

public class SpeedMoveTesterMb : MonoBehaviour, IInitable
{
    [SerializeField] private Transform _movingObjectTransform;
    [SerializeField] private Transform _startMoveTransform;
    [SerializeField] private Transform _endMoveTransform;

    [SerializeField] private Enums.MovingType _movingType;
    [SerializeField] private float _durationOfAnimation = 5f;

    private GameState _state;
    private EcsWorld _world;
    private EcsPool<TransformMoveComp> _linearMovePool;

    public void Init(GameState state)
    {
        _state = state;
        _world = state.EcsWorld;

        _linearMovePool = _world.GetPool<TransformMoveComp>();
    }

    [ContextMenu("Execute TestMove")]
    public void TestMove()
    {
        _movingObjectTransform.transform.position = _startMoveTransform.position;
        ref var moveComp = ref _linearMovePool.Add(_world.NewEntity());
        moveComp.MovingType = _movingType;
        moveComp.IsLocal = false;
        moveComp.Transform = _movingObjectTransform;
        moveComp.StartPosition = _startMoveTransform.position;
        moveComp.FinishPosition = _endMoveTransform.position;
        moveComp.Timer = _durationOfAnimation;
        moveComp.Duration = _durationOfAnimation;
        moveComp.Callback = TestMoveBack;
    }

    public void TestMoveBack()
    {
        ref var moveComp = ref _linearMovePool.Add(_world.NewEntity());
        moveComp.MovingType = _movingType;
        moveComp.IsLocal = false;
        moveComp.Transform = _movingObjectTransform;
        moveComp.StartPosition = _endMoveTransform.position;
        moveComp.FinishPosition = _startMoveTransform.position;
        moveComp.Timer = _durationOfAnimation;
        moveComp.Duration = _durationOfAnimation;
    }   
}