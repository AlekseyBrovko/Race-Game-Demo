using UnityEngine;
using UnityEngine.UI;

public class MissionPartIcon : MonoBehaviour
{
    public int MissionPartId { get; private set; }
    [SerializeField] private Text _iconText;
    [SerializeField] private Image _completeImage;
    [SerializeField] private Image _failImage;

    public void Init(int missionPartId) =>
        MissionPartId = missionPartId;

    public void ShowText(string text) =>
        _iconText.text = text;

    public void ShowPartCompleteState() =>
        _completeImage.gameObject.SetActive(true);

    public void ShowPartFailState() =>
        _failImage.gameObject.SetActive(true);
}