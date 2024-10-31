using System.Linq;
using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using Client;

namespace Client
{
    sealed class TestersInit : IEcsInitSystem
    {
        private EcsCustomInject<GameState> _state;

        public void Init(EcsSystems systems)
        {
            IEnumerable<IInitable> testers = Object.FindObjectsOfType<MonoBehaviour>().OfType<IInitable>();
            foreach (var tester in testers)
                tester.Init(_state.Value);
        }
    }
}