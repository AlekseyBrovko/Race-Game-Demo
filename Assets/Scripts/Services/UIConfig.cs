using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "Configs/UIConfig", order = 2)]
public class UIConfig : ScriptableObject
{
    [Header("Prefab")]
    public CanvasBehaviour CanvasBehaviourPrefab;
    public LoadingCanvas LoadingCanvas;
    public FadeCanvas FadeCanvasPrefab;
    public GameObject FpsCanvas;
}
