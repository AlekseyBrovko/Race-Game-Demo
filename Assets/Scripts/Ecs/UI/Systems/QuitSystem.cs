using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public sealed class QuitSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<QuitEvent>> _filter = default;
        private EcsPoolInject<QuitEvent> _quitPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                _quitPool.Value.Del(entity);

                _state.Value.SaveCars();
                _state.Value.SaveMoneyScore();

                Debug.Log("QuitSystem Application.Quit()");
                
                Application.Quit();
            }
        }
    }
}