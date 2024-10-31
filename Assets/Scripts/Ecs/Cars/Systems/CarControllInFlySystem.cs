using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarControllInFlySystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarIsFlyComp>> _filter = default;
        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<CarComp> _carPool = default;

        private float _torqueForce = 1000f;
        //private float _angularVelocityThreshold = 0.2f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var carComp = ref _carPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);

                if (controllComp.VerticalInput != 0)
                {
                    carComp.Rigidbody.AddRelativeTorque(-Vector3.right * _torqueForce * controllComp.VerticalInput,
                    ForceMode.Force);
                }
                else
                {
                    //if (Mathf.Abs(carComp.Rigidbody.angularVelocity.z) > _angularVelocityThreshold)
                    //{
                    //    var sign = Mathf.Sign(carComp.Rigidbody.angularVelocity.z);
                    //    carComp.Rigidbody.AddRelativeTorque(sign * Vector3.right * _torqueForce, ForceMode.Force);
                    //}
                }

                if (controllComp.HorizontalInput != 0)
                {
                    carComp.Rigidbody.AddRelativeTorque(Vector3.forward * _torqueForce * controllComp.HorizontalInput,
                    ForceMode.Force);
                }
                else
                {
                    //if (Mathf.Abs(carComp.Rigidbody.angularVelocity.x) > _angularVelocityThreshold)
                    //{
                    //    var sign = Mathf.Sign(carComp.Rigidbody.angularVelocity.x);
                    //    carComp.Rigidbody.AddRelativeTorque(-sign * Vector3.forward * _torqueForce * 0.5f, ForceMode.Force);
                    //}
                }
            }
        }
    }
}