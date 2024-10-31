using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AttachedObstacleMb : PhysicalObject, IPhysicalObject, ITriggerInteractableWithThreshold
{
    [SerializeField] private UnityEvent _onCrash;
    [SerializeField] private bool _hasDelayFunc;
    [SerializeField] private float _delayTimer = 5f;
    [SerializeField] private UnityEvent _afterDelay;
    [SerializeField] private Enums.PhysicalInteractableType _physicalType;

    public override Enums.PhysicalInteractableType PhysicalInteractableType => _physicalType;
    public bool HasFirstTrigger { get; private set; }
    public int? SpeedThreshold => null;

    public void OnFirstTrigger()
    {
        HasFirstTrigger = true;
        _onCrash.Invoke();
        if (_hasDelayFunc)
            StartCoroutine(DelayCor(_delayTimer));
    }

    private IEnumerator DelayCor(float time)
    {
        yield return new WaitForSeconds(time);
        _afterDelay.Invoke();
    }
}