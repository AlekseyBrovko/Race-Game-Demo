using UnityEngine;

public class PatrollPointMb : MonoBehaviour
{
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