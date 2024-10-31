using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig", order = 2)]
public class PlayerConfig : ScriptableObject
{
    [Header("Camera Settings")]
    public Vector3 VirtualCameraBodyFollowOffset = new Vector3(0, 2.12f, -5.33f);
    public float xDamping = 1f;
    public float yDamping = 0.2f;
    public float zDamping = 0.3f;
    public float YawDamping = 1f;

    public Vector3 VirtualCameraAimObjectOffset = new Vector3(0, 0.4f, 0);
    public float LookaheadTime = 0.13f;
    public float LookaheadSmoothing = 5.8f;
    public float SoftZoneHeight = 0.4f;
    public float SoftZoneWidth = 0.5f;


    [Header("Cars Settings")]
    public AnimationCurve SteeringAnimationCurve;

    [Header("Cars Throttles")]
    public AnimationCurve SimpleThrottleCurve;
    public AnimationCurve RaceThrottleCurve;
    public AnimationCurve DriftThrottleCurve;
    public AnimationCurve OffroadThrottleCurve;

    public float TransmissionIndex = 15f;
    public float BackMaxSpeedKmh = 20f;
    public float IdleThresholdKmh = 3f;
}