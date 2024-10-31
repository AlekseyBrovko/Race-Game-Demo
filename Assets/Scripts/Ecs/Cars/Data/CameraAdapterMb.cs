using UnityEngine;

public class CameraAdapterMb : MonoBehaviour, ICameraOffset
{
    public float CameraZOffset => _cameraZOffset;
    public float CameraYOffset => _cameraYOffset;

    [SerializeField] private float _cameraZOffset;
    [SerializeField] private float _cameraYOffset;
    [field: SerializeField] public float CameraLookAtYOffset { get; private set; }
}

public interface ICameraOffset
{
    public float CameraZOffset { get; }
    public float CameraYOffset { get; }
    public float CameraLookAtYOffset { get; }
}