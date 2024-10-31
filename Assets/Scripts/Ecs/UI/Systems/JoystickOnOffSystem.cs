using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class JoystickOnOffSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<JoystickUIEvent>> _filter = default;

        private EcsPoolInject<JoystickUIEvent> _eventPool = default;
        private EcsPoolInject<JoystickPanelComp> _joystickPanelPool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var eventComp = ref _eventPool.Value.Get(entity);
                ref var joystickPanelComp = ref _joystickPanelPool.Value.Get(_state.Value.InterfaceEntity);

                switch(eventComp.Value)
                {
                    case Enums.OnOffEnum.On:
                        joystickPanelComp.JoystickPanelMb.TurnOnJoystick();
                        joystickPanelComp.JoystickPanelMb.SetJoystickPosition(eventComp.TouchPosition);
                        break;

                    case Enums.OnOffEnum.Off:
                        joystickPanelComp.JoystickPanelMb.TurnOffJoystick();
                        break;
                }
                _eventPool.Value.Del(entity);
            }
        }
    }
}