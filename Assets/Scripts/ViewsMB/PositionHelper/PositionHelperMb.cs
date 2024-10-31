using UnityEngine;

public class PositionHelperMb : MonoBehaviour
{
    [SerializeField] private Color _gizmoColor;
    [SerializeField] private float _gizmoWidth = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawSphere(transform.position, _gizmoWidth);
    }
}