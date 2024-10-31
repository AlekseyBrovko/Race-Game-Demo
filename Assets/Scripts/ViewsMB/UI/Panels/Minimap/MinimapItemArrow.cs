using UnityEngine;
using UnityEngine.UI;

public class MinimapItemArrow : MonoBehaviour
{
    public int Id { get; private set; }
    [field: SerializeField] public RectTransform RectTransform { get; private set; }

    [SerializeField] private Image _iconImage;

    public void Init(int id, Color color)
    {
        Id = id;
        _iconImage.color = color;
    }
}