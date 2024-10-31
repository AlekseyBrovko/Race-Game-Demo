using Cinemachine;
using UnityEngine;

public class SceneIniterMb : MonoBehaviour
{
    [SerializeField] public GameObject MainCamera;
    [SerializeField] public CinemachineVirtualCamera VirtualCamera;
    [SerializeField] public CinemachineFreeLook FreeLookCamera;
    [SerializeField] public PatrollPointsHolder PatrollPointsHolder;
    [SerializeField] public SpawnersHolder SpawnersHolder;
    [SerializeField] public PlayerSpawnPointMb PlayerSpawnPointMb;
    [SerializeField] public Transform PickupsPointsHolder;
    [SerializeField] public PointsOfInterestHolder PointsOfInterestHolder;
    [SerializeField] public Transform RespawnPoints;
    [SerializeField] public Transform ZoneSeparators;
    [SerializeField] public Transform MissionSpawnPoints;
}