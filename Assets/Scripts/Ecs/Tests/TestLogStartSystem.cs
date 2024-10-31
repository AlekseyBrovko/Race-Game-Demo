using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Client
{
    sealed class TestLogStartSystem : IEcsRunSystem
    {
        //private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        //private EcsPoolInject<TestLogEvent> _logPool = default;
        //private EcsPoolInject<InputComp> _inputPool = default;

        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<CarFuelComp> _fuelPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<CarStatisticComp> _carStatisticPool = default;

        //private bool _prevPressed = false;

        public void Run(EcsSystems systems)
        {
            //ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);
            //if (inputComp.IsSpacePressed && !_prevPressed)
            //    Test();

            //_prevPressed = inputComp.IsSpacePressed;


            //if (inputComp.AxisKeyBoardValue != Vector2.zero)
            //{
            //    _logPool.Value.Add(_world.Value.NewEntity());
            //}
            //Debug.Log($"IsPause = {_state.Value.PauseSystems}; IsPlay = {_state.Value.PlaySystems}; " +
            //    $"IsWin = {_state.Value.WinSystems}; IsLose = {_state.Value.LoseSystems};");
        }

        private void Test()
        {
            int entity = _state.Value.PlayerCarEntity;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            ref var viewComp = ref _viewPool.Value.Get(entity);
            sw.Stop();
            ref var carComp = ref _carPool.Value.Get(entity);
            ref var fuelComp = ref _fuelPool.Value.Get(entity);
            ref var statComp = ref _carStatisticPool.Value.Get(entity);
            Debug.Log("sw.Elapsed = " + sw.Elapsed);
        }
    }
}