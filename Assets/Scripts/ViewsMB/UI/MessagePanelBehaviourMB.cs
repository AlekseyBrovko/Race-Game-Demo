using Client;
using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagePanelBehaviourMB : MonoBehaviour
{
    #region ECS
    private EcsWorld _world;
    private GameState _state;

    public void Init(GameState state, CanvasBehaviour canvasMB)
    {
        var world = state.EcsWorld;
        _world = world;
        _state = state;
        _canvasMb = canvasMB;

        ClearPanel();
    }
    #endregion

    private CanvasBehaviour _canvasMb;
    [SerializeField] private GameObject _simpleMessageIcon;
    [SerializeField] private TMP_Text _simpleMessageText;

    public void ShowSimpleMessage(string message)
    {
        _simpleMessageText.text = message;
    }

    public void ClearPanel()
    {
        _simpleMessageIcon.SetActive(false);
    }
}
