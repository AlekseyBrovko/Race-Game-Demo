using UnityEngine;

public class NpcMeleeAttackBodyPoints : MonoBehaviour
{
    public Transform HeadTransform => _headTransform;
    public Transform LeftHandTransform => _leftHandTransform;
    public Transform RightHandTransform => _rightHandTransform;
    public Transform LeftLegTransform => _leftLegTransform;
    public Transform RightLegTransform => _rightLegTransform;

    [SerializeField] private Transform _headTransform;
    [SerializeField] private Transform _leftHandTransform;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _leftLegTransform;
    [SerializeField] private Transform _rightLegTransform;
}
