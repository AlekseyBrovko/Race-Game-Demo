using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PlatformInit : IEcsInitSystem
    {
        private EcsCustomInject<GameState> _state = default;

        public void Init(EcsSystems systems)
        {
            //bool isWebgl
            _state.Value.IsMobilePlatform = Application.isMobilePlatform;

            if (Application.platform == RuntimePlatform.Android)
                Application.targetFrameRate = 60;

            if (Application.platform == RuntimePlatform.WebGLPlayer)
                _state.Value.IsWebGl = true;

            //Debug.Log("Application.targetFrameRate = " + Application.targetFrameRate); 
            Debug.Log($"Application.platform = {Application.platform.ToString()}; IsMobilePlatform = {Application.isMobilePlatform};");
        }
    }
}