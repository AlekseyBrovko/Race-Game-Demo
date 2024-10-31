using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class JoystickPanelBehaviour : PanelBase, ISecondPanelWithMainPanel
{
    public string MainPanelId { get; set; }

    [SerializeField] private Image _joystickImage;
    [SerializeField] private Image _stickImage;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private OnScreenStick _onScreenStick;
    [SerializeField] private StickMB _stickMb;

    private EcsWorld _world;
    private EcsPool<JoystickPanelComp> _joystickPanelPool;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _world = _state.EcsWorld;
        _joystickPanelPool = _world.GetPool<JoystickPanelComp>();
        ref var joystickPanelComp = ref _joystickPanelPool.Add(_state.InterfaceEntity);
        joystickPanelComp.JoystickPanelMb = this;

        InitJoystick();
    }

    public override void CleanupPanel() =>
        _joystickPanelPool.Del(_state.InterfaceEntity);

    public void SetJoystickPosition(Vector2 position) =>
        _joystickImage.rectTransform.position = position;

    public void TurnOnJoystick() =>
        _canvasGroup.alpha = 1;

    public void TurnOffJoystick() =>
        _canvasGroup.alpha = 0;

    private void InitJoystick()
    {
        _stickMb.Init(_state);

        float movementRange = _onScreenStick.movementRange;
        float radius = _stickImage.rectTransform.sizeDelta.x + movementRange * 2f;
        _joystickImage.rectTransform.sizeDelta = new Vector2(radius, radius);

        TurnOffJoystick();
    }
}
