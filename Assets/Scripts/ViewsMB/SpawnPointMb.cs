using UnityEngine;

public class SpawnPointMb : MonoBehaviour
{
    [SerializeField] private bool _drawGizmo;
    [SerializeField] private Color _gizmoColor;

    private void OnDrawGizmos()
    {
        if (_drawGizmo)
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(transform.position, 1);
        }
    }
}