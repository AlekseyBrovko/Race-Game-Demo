using UnityEngine;

public class PickupPointMb : MonoBehaviour
{
    [field: SerializeField] public Enums.PickupType PickupType { get; private set; }

    [SerializeField] private bool _drawGizmo;
    [SerializeField] private Color _color;

    private void OnDrawGizmos()
    {
        if (_drawGizmo)
        {
            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, 1);
        }
    }
}