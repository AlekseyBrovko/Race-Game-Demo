using UnityEngine;

public class RespawnPointMb : MonoBehaviour
{
    [field: SerializeField] public Enums.IslandName IslandName { get; private set; }
    [SerializeField] private Color _gizmoColor;

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawSphere(this.transform.position, 1f);
    }
}