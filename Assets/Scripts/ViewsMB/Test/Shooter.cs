using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _target;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _angleInDegrees = 30f;

    private float g = Physics.gravity.y;

    [ContextMenu("Shoot")]
    private void Shoot()
    {
        Vector3 dir = _target.transform.position - _spawnPoint.transform.position;
        GameObject bullet = Instantiate(_bulletPrefab, _spawnPoint.transform.position, Quaternion.LookRotation(dir));
        Vector3 rot = bullet.transform.rotation.eulerAngles;
        rot = new Vector3(-_angleInDegrees, rot.y, rot.z);
        bullet.transform.rotation = Quaternion.Euler(rot);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        Vector3 fromTo = _target.transform.position - _spawnPoint.transform.position;
        Vector3 fromToXZ = new Vector3(fromTo.x, 0, fromTo.z);

        float x = fromToXZ.magnitude;
        float y = fromTo.y;

        float angleInRad = _angleInDegrees * Mathf.Deg2Rad;
        float v2 = (g * x * x) / (2f * (y - Mathf.Tan(angleInRad) * x) * Mathf.Pow(Mathf.Cos(angleInRad), 2));
        float v = Mathf.Sqrt(Mathf.Abs(v2));

        rb.velocity = bullet.transform.forward * v;
    }

    public float GetVelocity(float angleInDegrees, Vector3 startPos, Vector3 targetPos)
    {
        Vector3 fromTo = targetPos - startPos;
        Vector3 fromToXZ = new Vector3(fromTo.x, 0, fromTo.z);

        float x = fromToXZ.magnitude;
        float y = fromTo.y;

        float angleInRad = angleInDegrees * Mathf.Deg2Rad;
        float v2 = (Physics.gravity.y * x * x) / (2f * (y - Mathf.Tan(angleInRad) * x) * Mathf.Pow(Mathf.Cos(angleInRad), 2));
        float result = Mathf.Sqrt(Mathf.Abs(v2));
        return result;
    }
}