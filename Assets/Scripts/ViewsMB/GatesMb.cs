using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GatesMb : MonoBehaviour
{
    [SerializeField] private Transform _leftGate;
    [SerializeField] private Transform _rightGate;

    private GatesState _state = GatesState.Closed;

    private Coroutine _openGatesCor;
    private Coroutine _closeGatesCor;

    private float _gatesOpeningDeltaPos = 3.45f;

    private Vector3 _leftGateClosePos;
    private Vector3 _leftGateOpenPos;
    private Vector3 _rightGateClosePos;
    private Vector3 _rightGateOpenPos;

    private float _defaultDuration = 3f;
    
    private void Awake()
    {
        _leftGateClosePos = _leftGate.transform.localPosition;
        _rightGateClosePos = _rightGate.transform.localPosition;

        _leftGateOpenPos =
            new Vector3(_rightGateOpenPos.x, _rightGateOpenPos.y, _rightGateOpenPos.z + _gatesOpeningDeltaPos);
        _rightGateOpenPos =
            new Vector3(_rightGateClosePos.x, _rightGateClosePos.y, _rightGateClosePos.z - _gatesOpeningDeltaPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CarMb carMb))
            OpenGates();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CarMb carMb))
            CloseGates();
    }

    private enum GatesState
    {
        Closed, Opened, Opening, Closing
    }

    [ContextMenu("OpenGates()")]
    private void OpenGates()
    {
        switch (_state)
        {
            case GatesState.Opened:
                break;

            case GatesState.Closed:
                _openGatesCor = StartCoroutine(OpenGatesCor());
                break;

            case GatesState.Opening:
                break;

            case GatesState.Closing:
                StopCoroutine(_closeGatesCor);
                _openGatesCor = StartCoroutine(OpenGatesCor());
                break;
        }
    }

    [ContextMenu("CloseGates()")]
    private void CloseGates()
    {
        switch (_state)
        {
            case GatesState.Opened:
                _closeGatesCor = StartCoroutine(CloseGatesCor());
                break;

            case GatesState.Closed:
                break;

            case GatesState.Opening:
                StopCoroutine(_openGatesCor);
                _closeGatesCor = StartCoroutine(CloseGatesCor());
                break;

            case GatesState.Closing:
                break;
        }
    }

    private IEnumerator OpenGatesCor()
    {
        _state = GatesState.Opening;
        float distance = Vector3.Distance(_leftGate.localPosition, _leftGateOpenPos);
        float duration = _defaultDuration * distance / _gatesOpeningDeltaPos;
        bool isOpen = false;
        float timer = 0f;

        Vector3 leftGateStartPos = _leftGate.localPosition;
        Vector3 rightGateStartPos = _rightGate.localPosition;

        while (!isOpen)
        {   
            timer += Time.deltaTime;
            float index = timer / duration;

            Vector3 leftTempPos = Vector3.Lerp(leftGateStartPos, _leftGateOpenPos, index);
            _leftGate.localPosition = leftTempPos;

            Vector3 rightTempPos = Vector3.Lerp(rightGateStartPos, _rightGateOpenPos, index);
            _rightGate.localPosition = rightTempPos;

            if (Tools.ComparisonWithThreshold(_leftGate.position.z, _leftGateOpenPos.z, 0.01f))
                isOpen = true;
            yield return null;
        }

        _leftGate.localPosition = _leftGateOpenPos;
        _rightGate.localPosition = _rightGateOpenPos;

        _state = GatesState.Opened;
    }

    private IEnumerator CloseGatesCor()
    {
        _state = GatesState.Closing;
        float distance = Vector3.Distance(_leftGate.localPosition, _leftGateClosePos);
        float duration = _defaultDuration * distance / _gatesOpeningDeltaPos;
        bool isClose = false;
        float timer = 0f;

        Vector3 leftGateStartPos = _leftGate.localPosition;
        Vector3 rightGateStartPos = _rightGate.localPosition;

        while (!isClose)
        {
            timer += Time.deltaTime;
            float index = timer / duration;

            Vector3 leftTempPos = Vector3.Lerp(leftGateStartPos, _leftGateClosePos, index);
            _leftGate.localPosition = leftTempPos;

            Vector3 rightTempPos = Vector3.Lerp(rightGateStartPos, _rightGateClosePos, index);
            _rightGate.localPosition = rightTempPos;

            if (Tools.ComparisonWithThreshold(_leftGate.position.z, _leftGateOpenPos.z, 0.01f))
                isClose = true;

            yield return null;
        }

        _leftGate.localPosition = _leftGateClosePos;
        _rightGate.localPosition = _rightGateClosePos;

        _state = GatesState.Closed;
    }
}