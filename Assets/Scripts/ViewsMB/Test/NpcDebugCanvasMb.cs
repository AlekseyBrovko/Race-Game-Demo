using UnityEngine;
using UnityEngine.UI;

public class NpcDebugCanvasMb : MonoBehaviour
{
    [SerializeField] private Text _debugText;
    
    public void ShowState(string state)
    {
        _debugText.text = state;
    }
}