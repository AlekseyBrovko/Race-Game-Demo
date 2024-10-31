using Client;
using UnityEngine;
using static Enums;

public interface ICar
{
    public Transform Transform { get; }
    public int Entity { get; }
    public WheelDriveType WheelDriveType { get; }
    public CarType CarType { get; }
    public CarMotorType CarMotorType { get; }
    public BoxCollider MainTriggerCollider { get; }
    public Transform CenterOfMass { get; }
    public Rigidbody Rigidbody { get; }
    public Wheel[] Wheels { get; }
    public ParticleSystem[] StopFlareParticles { get; }
    public ParticleSystem[] BackDriveFlareParticles { get; }
    public ParticleSystem[] DeathSmokeParticles { get; }
    public GameObject ArmorGameObject { get; }
    public int TransmissionGearsAmount { get; }
    public float DriftHelpIndex { get; }
    public float MotorPower { get; }
    public float MaxSpeed { get; }
    public float TurningRadius { get; }
    public float SteeringSpeed { get; }
    public float BreakPower { get; }
    public float HandBreakPower { get; }

    public void Init(GameState state, int entity);
    public void SetShopState();
    public void ResetCollisionsTemp();
}