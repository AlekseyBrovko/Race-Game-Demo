using NWH.WheelController3D;
using System;
using UnityEngine;
using static Enums;

[Serializable]
public class Wheel
{
    public Transform WheelMesh;
    public WheelController WheelController;
    public OrientationEnum Orientation;
    public SideEnum Side;
    public bool IsCustomDrivingWheel;
    public int AxisNumber;
    public WheelEffectsMb EffectsMb;

    //friction settings
    private float _firwardStiffness;
    private float _forwardGrip;
    private float _sideStiffness;
    private float _sideGrip;

    public void SaveStartFrictionSettings()
    {
        _firwardStiffness = WheelController.LongitudinalFrictionStiffness;
        _forwardGrip =  WheelController.LongitudinalFrictionGrip;
        _sideStiffness = WheelController.LateralFrictionStiffness;
        _sideGrip = WheelController.LateralFrictionGrip;
    }

    public void SetHandBrakeFrictionSettings(float index)
    {
        //Debug.Log("SetHandBrakeFrictionSettings");
        WheelController.LateralFrictionStiffness *= index;
        WheelController.LateralFrictionGrip *= index;
    }

    public void SetStartFrictionSettings()
    {
        WheelController.LongitudinalFrictionStiffness = _firwardStiffness;
        WheelController.LongitudinalFrictionGrip = _forwardGrip;
        WheelController.LateralFrictionStiffness = _sideStiffness;
        WheelController.LateralFrictionGrip = _sideGrip;
    }

    public void SetTorque(float value)
    {
        if (float.IsNaN(value) || float.IsPositiveInfinity(value) || float.IsNegativeInfinity(value))
        {
            Debug.LogWarning("опнакелю мюдн тхйяхрэ SetTorque value == " + value);
            WheelController.MotorTorque = 0;
            return;
        }

        if (WheelController.IsGrounded)
            WheelController.MotorTorque = value;
        else
            WheelController.MotorTorque = 0;
    }
        

    public void SetBrake(float value)
    {
        if (WheelController.IsGrounded)
            WheelController.BrakeTorque = value;
        else
            WheelController.BrakeTorque = 0;
    }
}
