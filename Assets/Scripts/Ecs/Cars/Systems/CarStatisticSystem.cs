using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using NWH.WheelController3D;
using UnityEngine;
using WheelHit = NWH.WheelController3D.WheelHit;

namespace Client
{
    sealed class CarStatisticSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarStatisticComp>> _filter = default;
        private EcsFilterInject<Inc<CarWcForwardSlipStatisticComp>> _forwardSlipFilter = default;
        private EcsFilterInject<Inc<CarWcSideSlipStaticticComp>> _sideSlipFilter = default;
        private EcsFilterInject<Inc<CarMotorAndBrakeTorqueStatisticComp>> _torqueFilter = default;
        private EcsFilterInject<Inc<CarWcRpmStaticticComp>> _rpmFilter = default;
        private EcsFilterInject<Inc<IndexOfSpeedStatisticComp>> _speedIndexFilter = default;

        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<CarWheelsControllComp> _wheelsPool = default;

        private EcsPoolInject<CarWcForwardSlipStatisticComp> _forwardSlipPool = default;
        private EcsPoolInject<CarWcSideSlipStaticticComp> _sideSlipPool = default;
        private EcsPoolInject<CarMotorAndBrakeTorqueStatisticComp> _torquePool = default;
        private EcsPoolInject<CarWcRpmStaticticComp> _rpmPool = default;
        private EcsPoolInject<IndexOfSpeedStatisticComp> _speedIndexPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);
                ref var viewComp = ref _viewPool.Value.Get(entity);

                statisticComp.SpeedMpS = carComp.CarMb.Rigidbody.velocity.magnitude;
                statisticComp.PreviousFrameSpeedKmpH = statisticComp.SpeedKmpH;
                statisticComp.SpeedKmpH = statisticComp.SpeedMpS * 3.6f;
            }

            foreach (var entity in _forwardSlipFilter.Value)
            {
                ref var slipComp = ref _forwardSlipPool.Value.Get(entity);
                ref var wheelComp = ref _wheelsPool.Value.Get(entity);

                slipComp.FR_ForwardSlip = GetForwardSlip(wheelComp.FrontRightWheel);
                slipComp.FL_ForwardSlip = GetForwardSlip(wheelComp.FrontLeftWheel);
                slipComp.RR_ForwardSlip = GetForwardSlip(wheelComp.RearRightWheel);
                slipComp.RL_ForwardSlip = GetForwardSlip(wheelComp.RearLeftWheel);
            }

            foreach (var entity in _sideSlipFilter.Value)
            {
                ref var slipComp = ref _sideSlipPool.Value.Get(entity);
                ref var wheelComp = ref _wheelsPool.Value.Get(entity);

                slipComp.FR_SideSlip = GetSideSlip(wheelComp.FrontRightWheel);
                slipComp.FL_SideSlip = GetSideSlip(wheelComp.FrontLeftWheel);
                slipComp.RR_SideSlip = GetSideSlip(wheelComp.RearRightWheel);
                slipComp.RL_SideSlip = GetSideSlip(wheelComp.RearLeftWheel);
            }

            foreach (var entity in _torqueFilter.Value)
            {
                ref var torqueComp = ref _torquePool.Value.Get(entity);
                ref var wheelComp = ref _wheelsPool.Value.Get(entity);

                torqueComp.FR_MotorTorque = wheelComp.FrontRightWheel.WheelController.MotorTorque;
                torqueComp.FL_MotorTorque = wheelComp.FrontLeftWheel.WheelController.MotorTorque;
                torqueComp.RR_MotorTorque = wheelComp.RearRightWheel.WheelController.MotorTorque;
                torqueComp.RL_MotorTorque = wheelComp.RearLeftWheel.WheelController.MotorTorque;

                torqueComp.FR_BrakeTorque = wheelComp.FrontRightWheel.WheelController.BrakeTorque;
                torqueComp.FL_BrakeTorque = wheelComp.FrontLeftWheel.WheelController.BrakeTorque;
                torqueComp.RR_BrakeTorque = wheelComp.RearRightWheel.WheelController.BrakeTorque;
                torqueComp.RL_BrakeTorque = wheelComp.RearLeftWheel.WheelController.BrakeTorque;
            }

            foreach (var entity in _rpmFilter.Value)
            {
                ref var rpmComp = ref _rpmPool.Value.Get(entity);
                ref var wheelComp = ref _wheelsPool.Value.Get(entity);
                rpmComp.FR_Rpm = wheelComp.FrontRightWheel.WheelController.RPM;
                rpmComp.FL_Rpm = wheelComp.FrontLeftWheel.WheelController.RPM;
                rpmComp.RR_Rpm = wheelComp.RearRightWheel.WheelController.RPM;
                rpmComp.RL_Rpm = wheelComp.RearLeftWheel.WheelController.RPM;
            }

            foreach (var entity in _speedIndexFilter.Value)
            {
                ref var indexComp = ref _speedIndexPool.Value.Get(entity);
                ref var wheelComp = ref _wheelsPool.Value.Get(entity);

                var FR_MsIndex = (Mathf.PI * 2 * wheelComp.FrontRightWheel.WheelController.Radius *
                    wheelComp.FrontRightWheel.WheelController.RPM) / 60f;

                var FL_MsIndex = (Mathf.PI * 2 * wheelComp.FrontLeftWheel.WheelController.Radius *
                    wheelComp.FrontLeftWheel.WheelController.RPM) / 60f;

                var RR_MsIndex = (Mathf.PI * 2 * wheelComp.RearRightWheel.WheelController.Radius *
                    wheelComp.RearRightWheel.WheelController.RPM) / 60f;

                var RL_MsIndex = (Mathf.PI * 2 * wheelComp.RearRightWheel.WheelController.Radius *
                    wheelComp.RearRightWheel.WheelController.RPM) / 60f;

                indexComp.FR_KmhIndex = FR_MsIndex * 3.6f;
                indexComp.FL_KmhIndex = FL_MsIndex * 3.6f;
                indexComp.RR_KmhIndex = RR_MsIndex * 3.6f;
                indexComp.RL_KmhIndex = RL_MsIndex * 3.6f;
            }
        }

        private float GetForwardSlip(Wheel wheel)
        {
            WheelController wc = wheel.WheelController;
            float result = 0;
            if (wc.IsGrounded)
                result = wc.LongitudinalSlip;
            return result;
        }

        private float GetSideSlip(Wheel wheel)
        {
            WheelController wc = wheel.WheelController;
            float result = 0;
            if (wc.IsGrounded)
                result = wc.LateralSlip;
            return result;
        }
    }
}