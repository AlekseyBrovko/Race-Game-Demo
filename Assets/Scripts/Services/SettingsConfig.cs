using UnityEngine;

[CreateAssetMenu(fileName = "SettingsConfig", menuName = "Configs/SettingsConfig", order = 2)]
public class SettingsConfig : ScriptableObject
{
    [Header("Animation Curves For Move or Rotate Animations")]
    public AnimationCurve LinearCurve;

    public AnimationCurve FastOnStartCurve;
    public AnimationCurve VeryFastOnStartCurve;

    public AnimationCurve FastOnEndCurve;
    public AnimationCurve VeryFastOnEndCurve;

    [Header("Enemies Settings")]
    [Header("Vision")]
    public float AngleOfMeleeLook = 40f;
    public float AngleOfRangeLook = 40f;

    public float RadiusOfMeleeLook = 20f;
    public float SqrRadiusOfMeleeFeel = 100f;

    public float RadiusOfRangeLook = 50f;
    public float SqrRadiusOfRangeFeel = 500f;

    [Header("Attack")]
    public float MeleeDamage = 10f;
    public float RangeDamage = 10f;
    public float RangeExplosionDamage = 10f;
    public float ExplosionRadiusOfDamage = 5f;
    public float MeleeAttackCoolDown = 1f;
    public float RangeAttackCoolDown = 2f;

    [Header("Money")]
    public float RewardForMelee = 10f;
    public float RewardForRange = 10f;



    public AnimationCurve GetCurveByAnimationType(Enums.MovingType movingType)
    {
        AnimationCurve result = LinearCurve;
        switch (movingType)
        {
            default: result = LinearCurve; break;
            case Enums.MovingType.FastOnEnd: result = FastOnEndCurve; break;
            case Enums.MovingType.FastOnStart: result = FastOnStartCurve; break;
            case Enums.MovingType.VeryFastOnEnd: result = VeryFastOnEndCurve; break;
            case Enums.MovingType.VeryFastOnStart: result = VeryFastOnStartCurve; break;
        }
        return result;
    }
}