using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigBigObjectInHandsHandler : MonoBehaviour
{
    [SerializeField] private Rig _twoHandsRig;
    [SerializeField] private Transform _leftHandTargetTransform;
    [SerializeField] private Transform _rightHandTargetTransform;

    private int _entity;
    private GameState _state;
    private RangeThrowingNpcMb _owner;

    private Vector3 _leftHandTargetStartPos;
    private Vector3 _rightHandTargetStartPos;

    public void Init(GameState state, int entity)
    {
        _state = state;
        _entity = entity;

        _leftHandTargetStartPos = _leftHandTargetTransform.position;
        _rightHandTargetStartPos = _rightHandTargetTransform.position;
    }

    public void OnTakeObjectInHands(IThrowObject throwObj)
    {
        TurnOnRig();
    }

    public void TurnOnRig()
    {
        _twoHandsRig.weight = 1.0f;
    }

    public void TurnOffRig()
    {
        _twoHandsRig.weight = 0f;
    }

    public void ResetPositionOfTargetObjects()
    {
        _leftHandTargetTransform.position = _leftHandTargetStartPos;
        _rightHandTargetTransform.position = _rightHandTargetStartPos;
    }
    
    public void HandleRig(IThrowObject throwObj)
    {

    }

    public void HandleRig(ITwoHandsThrowingObject throwObj)
    {
        _leftHandTargetTransform.position = throwObj.LeftHandPlace.position;
        _rightHandTargetTransform.position = throwObj.RightHandPlace.position;
    }
}