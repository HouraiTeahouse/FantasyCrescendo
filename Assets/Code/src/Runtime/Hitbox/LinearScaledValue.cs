using System;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public struct LinearScaledValue {

    public float BaseValue;
    public float ScalingFactor;

    public float GetScaledValue(float scaling) =>
        BaseValue + ScalingFactor * scaling;

    public static implicit operator LinearScaledValue(float value) => 
        new LinearScaledValue { BaseValue = value, ScalingFactor = 0f };
 
}

}
