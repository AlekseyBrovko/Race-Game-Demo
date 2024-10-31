using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using Client;

public class TutorialPanelBehaviourMB : MonoBehaviour
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
    }
    #endregion

    private CanvasBehaviour _canvasMb;
}
