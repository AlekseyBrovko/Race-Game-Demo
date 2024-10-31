using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.EventSystems;

public class StickMB : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region ECS
    private EcsWorld _world;
    private GameState _state;
    private EcsPool<JoystickUIEvent> _joystickPool;

    public void Init(GameState state)
    {
        _state = state;
        _world = state.EcsWorld;
        _joystickPool = _world.GetPool<JoystickUIEvent>();
    }
    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        ref var joystickEventComp = ref _joystickPool.Add(_world.NewEntity());
        joystickEventComp.Value = Enums.OnOffEnum.On;
        joystickEventComp.TouchPosition = eventData.pointerPressRaycast.screenPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ref var joystickEventComp = ref _joystickPool.Add(_world.NewEntity());
        joystickEventComp.Value = Enums.OnOffEnum.Off;
    }
}
