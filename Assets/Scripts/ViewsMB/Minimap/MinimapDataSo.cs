using UnityEngine;

public class MinimapDataSo : ScriptableObject
{
    [field: SerializeField] public float SquareSide { get; set; }
    [field: SerializeField] public int HorizontalSquareCount { get; set; }
    [field: SerializeField] public int VerticalSquareCount { get; set; }
    [field: SerializeField] public float HorizontalOffset { get; set; }
    [field: SerializeField] public float VerticalOffset { get; set; }
    [field: SerializeField] public Texture2D[] PartsOfMinimap { get; set; }
}