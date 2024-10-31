using System;
using UnityEngine.Animations.Rigging;

public class ValueWrapper<T> : IValue<T>
{
    private T _value;

    public T Value { get => _value; set => _value = value; }
}

public interface IValue<T>
{
    public T Value { get; set; }
}

public class ValueVariable : IValue<float>
{
    public float Value { get => _rig.weight; set => _rig.weight = value; }

    private Rig _rig;

    public ValueVariable(Rig rig)
    {
        _rig = rig;
    }
}

public class FloatValueForChange
{
    public IValue<float> ValueForChange;
    public float StartValue;
    public float EndValue;
    public float Timer;
    public float Duration;
    public Action Callback;

    public FloatValueForChange(IValue<float> valueForChange, float startValue,
        float endValue, float duration, Action callback = null)
    {
        ValueForChange = valueForChange;
        StartValue = startValue;
        EndValue = endValue;
        Timer = duration;
        Duration = duration;
        Callback = callback;
    }
}