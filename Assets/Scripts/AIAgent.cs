using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
    private Player ai;

    #region Fuzzy Values
    [Header("Health Attributes")]
    [SerializeField] private AnimationCurve critical;
    [SerializeField] private AnimationCurve hurt;
    [SerializeField] private AnimationCurve healthy;

    private float criticalValue = 0.0f;
    private float hurtValue = 0.0f;
    private float healthyValue = 0.0f;

    [Header("Shield Conditions")]
    [SerializeField] private AnimationCurve broken;
    [SerializeField] private AnimationCurve damaged;
    [SerializeField] private AnimationCurve asNew;

    private float brokenValue = 0.0f;
    private float damagedValue = 0.0f;
    private float asNewValue = 0.0f;

    [Header("Kick Energy")]
    [SerializeField] private AnimationCurve exhausted;
    [SerializeField] private AnimationCurve tired;
    [SerializeField] private AnimationCurve energetic;

    private float exhaustedValue = 0.0f;
    private float tiredValue = 0.0f;
    private float energeticValue = 0.0f;

    [Header("Cards")]
    [SerializeField] private GameObject attackCard;
    [SerializeField] private GameObject blockCard;
    [SerializeField] private GameObject kickCard;
    [SerializeField] private GameObject healCard;
    #endregion

    private void Start()
    {
        ai = GetComponent<Player>();
    }

    public void EvaluateValues()
    {
        float aiTime = (float)ai.MHealth / (float)ai.maxHealth;

        criticalValue = critical.Evaluate(aiTime);
        hurtValue = hurt.Evaluate(aiTime);
        healthyValue = healthy.Evaluate(aiTime);
        
        Debug.Log($"Critical: {criticalValue}");
        Debug.Log($"Hurt: {hurtValue}");
        Debug.Log($"Healthy: {healthyValue}");

        aiTime = (float)ai.MShieldDurability / (float)ai.maxShieldRecharge;

        brokenValue = broken.Evaluate(aiTime);
        damagedValue = damaged.Evaluate(aiTime);
        asNewValue = asNew.Evaluate(aiTime);

        Debug.Log($"Broken: {brokenValue}");
        Debug.Log($"Damaged: {damagedValue}");
        Debug.Log($"As New: {asNewValue}");

        aiTime = (float)ai.MKickRecharge / (float)ai.maxKickRecharge;

        exhaustedValue = exhausted.Evaluate(aiTime);
        tiredValue = tired.Evaluate(aiTime);
        energeticValue = energetic.Evaluate(aiTime);

        Debug.Log($"Exhausted: {exhaustedValue}");
        Debug.Log($"Tired: {tiredValue}");
        Debug.Log($"Energetic: {energeticValue}");
    }

    #region Hedges
    private float Fairly(float value)
    {
        return Mathf.Sqrt(value);
    }

    private float Very(float value)
    {
        return value * value;
    }
    #endregion
}

// https://hub.packtpub.com/fuzzy-logic-ai-characters-unity-3d-games/