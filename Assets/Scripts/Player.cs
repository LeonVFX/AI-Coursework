using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int mHealth;

    public int MHealth
    {
        get { return mHealth; }
        set { mHealth = value; }
    }

    private int mShieldDurability;

    public int MShieldDurability
    {
        get { return mShieldDurability; }
        set { mShieldDurability = value; }
    }

    private int mKickRecharge;

    public int MKickRecharge
    {
        get { return mKickRecharge; }
        set { mKickRecharge = value; }
    }

    [Header("Max Amount of Stats")]
    public int maxHealth = 100;
    public int maxShieldDurability = 10;
    public int maxKickRecharge = 3;

    #region Fuzzy Values
    [Header("Health Attributes")]
    [SerializeField] protected AnimationCurve critical;
    [SerializeField] protected AnimationCurve hurt;
    [SerializeField] protected AnimationCurve healthy;

    public float criticalValue = 0.0f;
    public float hurtValue = 0.0f;
    public float healthyValue = 0.0f;

    public bool CRITICAL { get { return criticalValue > 0; } }
    public bool HURT { get { return hurtValue > 0; } }
    public bool HEALTHY { get { return healthyValue > 0; } }

    [Header("Shield Conditions")]
    [SerializeField] protected AnimationCurve broken;
    [SerializeField] protected AnimationCurve damaged;
    [SerializeField] protected AnimationCurve asNew;

    public float brokenValue = 0.0f;
    public float damagedValue = 0.0f;
    public float asNewValue = 0.0f;

    public bool BROKEN { get { return brokenValue > 0; } }
    public bool DAMAGED { get { return damagedValue > 0; } }
    public bool ASNEW { get { return asNewValue > 0; } }

    [Header("Kick Energy")]
    [SerializeField] protected AnimationCurve exhausted;
    [SerializeField] protected AnimationCurve tired;
    [SerializeField] protected AnimationCurve energetic;

    public float exhaustedValue = 0.0f;
    public float tiredValue = 0.0f;
    public float energeticValue = 0.0f;

    public bool EXHAUSTED { get { return exhaustedValue > 0; } }
    public bool TIRED { get { return tiredValue > 0; } }
    public bool ENERGETIC { get { return energeticValue > 0; } }

    [Header("Cards")]
    [SerializeField] protected GameObject attackCard;
    [SerializeField] protected GameObject shieldCard;
    [SerializeField] protected GameObject kickCard;
    [SerializeField] protected GameObject healCard;

    // https://hub.packtpub.com/fuzzy-logic-ai-characters-unity-3d-games/
    #endregion

    protected void Start()
    {
        mHealth = maxHealth;
        mShieldDurability = 0;
        mKickRecharge = 0;
    }

    public void EvaluateValues()
    {
        float aiTime = (float)MHealth / (float)maxHealth;

        criticalValue = critical.Evaluate(aiTime);
        hurtValue = hurt.Evaluate(aiTime);
        healthyValue = healthy.Evaluate(aiTime);

        //Debug.Log($"Critical: {criticalValue}");
        //Debug.Log($"Hurt: {hurtValue}");
        //Debug.Log($"Healthy: {healthyValue}");

        aiTime = (float)MShieldDurability / (float)maxShieldDurability;

        brokenValue = broken.Evaluate(aiTime);
        damagedValue = damaged.Evaluate(aiTime);
        asNewValue = asNew.Evaluate(aiTime);

        //Debug.Log($"Broken: {brokenValue}");
        //Debug.Log($"Damaged: {damagedValue}");
        //Debug.Log($"As New: {asNewValue}");

        aiTime = (float)MKickRecharge / (float)maxKickRecharge;

        exhaustedValue = exhausted.Evaluate(aiTime);
        tiredValue = tired.Evaluate(aiTime);
        energeticValue = energetic.Evaluate(aiTime);

        //Debug.Log($"Exhausted: {exhaustedValue}");
        //Debug.Log($"Tired: {tiredValue}");
        //Debug.Log($"Energetic: {energeticValue}");
    }
}
