using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjectsShooter : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _target;
    [SerializeField] private Transform _spawnPoint;
    private SimpleTestMover _targetMover;
    private float _maxThrowForce = 200f;

    private float _predictedSpeedIndex = 1.5f;
    private float _predictedDistanceIndex = 1f;

    private void Start()
    {
        _targetMover = _target.GetComponent<SimpleTestMover>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //GameObject bullet = Instantiate(_bulletPrefab, _spawnPoint.position, _spawnPoint.rotation);
            //Rigidbody rb = bullet.GetComponent<Rigidbody>();
            //var data = CalculateThrowData(_target.transform.position, bullet.transform.position);
            //rb.velocity = data.ThrowVelocity;

            GameObject bullet = Instantiate(_bulletPrefab, _spawnPoint.position, _spawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            ThrowData data = CalculateThrowData(_target.transform.position, bullet.transform.position);
            data = CalculatePredictedThrowData(data, Vector3.zero);
            rb.velocity = data.ThrowVelocity;
        }
    }

    private ThrowData CalculatePredictedThrowData(ThrowData directionData, Vector3 targetVelocity)
    {
        Vector3 throwVelocity = directionData.ThrowVelocity;
        throwVelocity.y = 0;

        float time = directionData.DeltaXZ * _predictedDistanceIndex / throwVelocity.magnitude;
        //float time = directionData.DeltaXZ / throwVelocity.magnitude;

        Vector3 playerMovement = _target.transform.forward * _targetMover.Speed * _predictedSpeedIndex;

        Vector3 newTargetPosition = new Vector3(
            _target.transform.position.x + playerMovement.x,
            _target.transform.position.y,
            _target.transform.position.z + playerMovement.z
            ) ;

        Vector3 randomAround = GetRandomVectorInRadius(newTargetPosition, 5f);

        var predictedData = CalculateThrowData(newTargetPosition, directionData.StartPos);
        //var predictedData = CalculateThrowData(randomAround, directionData.StartPos);
        predictedData.ThrowVelocity =
            Vector3.ClampMagnitude(predictedData.ThrowVelocity, _maxThrowForce);
        return predictedData;
    }

    private ThrowData CalculateThrowData(Vector3 targetPos, Vector3 startPos)
    {
        Vector3 displacement = new Vector3(
            targetPos.x,
            startPos.y,
            targetPos.z
            ) - startPos;

        float deltaY = targetPos.y - startPos.y;
        float deltaZX = displacement.magnitude;
        float gravity = Mathf.Abs(Physics.gravity.y);
        float throwStrength = Mathf.Clamp(
            Mathf.Sqrt(gravity * (deltaY + Mathf.Sqrt(Mathf.Pow(deltaY, 2) + Mathf.Pow(deltaZX, 2)))),
            0.01f, _maxThrowForce
            );

        float angle = Mathf.PI / 2f - (0.5f * (Mathf.PI / 2f - (deltaY / deltaZX)));

        Vector3 initialVelocity = Mathf.Cos(angle) * throwStrength * displacement.normalized +
            Mathf.Sin(angle) * throwStrength * Vector3.up;

        return new ThrowData
        {
            ThrowVelocity = initialVelocity,
            Angle = angle,
            DeltaXZ = deltaZX,
            DeltaY = deltaY,
            StartPos = startPos
        };
    }

    private Vector3 GetRandomVectorInRadius(Vector3 pos, float radius)
    {
        return new Vector3(
            Random.Range(pos.x - radius, pos.x + radius),
            pos.y,
            Random.Range(pos.z - radius, pos.z + radius)
            );
    }
}

public class ThrowData
{
    public Vector3 ThrowVelocity { get; set; }
    public float Angle { get; set; }
    public float DeltaXZ { get; set; }
    public float DeltaY { get; set; }
    public Vector3 StartPos { get; set; }
}