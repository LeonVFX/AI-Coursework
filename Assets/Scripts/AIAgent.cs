using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.AnimationUtility;

public class AIAgent : Player
{
    private enum ActionState
    {
        None,
        Offense,
        VeryOffensive,
        Defense,
        VeryDefensive,
        Attack,
        Shield,
        Heal,
        Kick
    }

    private AIAgent ai;
    private Card selectedCard;
    private ActionState currentAction;

    [Header("Player Hand")]
    [SerializeField] private Player player;

    [Header("Behaviour State Curves")]
    [SerializeField] private AnimationCurve veryOffensiveCurve;
    [SerializeField] private AnimationCurve offenseCurve;
    [SerializeField] private AnimationCurve defenseCurve;
    [SerializeField] private AnimationCurve veryDefensiveCurve;

    [Header("Action Curves")]
    [SerializeField] private AnimationCurve attackCurve;
    [SerializeField] private AnimationCurve shieldCurve;
    [SerializeField] private AnimationCurve healCurve;
    [SerializeField] private AnimationCurve kickCurve;

    // Behaviour variables
    private float[] veryOffensiveValues;
    private float[] offenseValues;
    private float[] defenseValues;
    private float[] veryDefensiveValues;

    private float veryOffensiveMaxValue;
    private float offenseMaxValue;
    private float defenseMaxValue;
    private float veryDefensiveMaxValue;

    // Action variables
    private float[] attackValues;
    private float[] shieldValues;
    private float[] healValues;
    private float[] kickValues;

    private float attackMaxValue;
    private float shieldMaxValue;
    private float healMaxValue;
    private float kickMaxValue;

    private new void Start()
    {
        base.Start();

        ai = GetComponent<AIAgent>();
        selectedCard = GetComponent<Card>();

        // Arrays - 101 slots to accomodate 2 decimal point values for a broader range of values for Deffuzification.
        veryOffensiveValues = new float[101];
        offenseValues = new float[101];
        defenseValues = new float[101];
        veryDefensiveValues = new float[101];

        attackValues = new float[101];
        shieldValues = new float[101];
        healValues = new float[101];
        kickValues = new float[101];
    }

    private void Update()
    {
        if (GameManager.GM.turn == GameManager.WhoTurn.AI)
        {
            player.EvaluateValues();
            ai.EvaluateValues();

            Play();
            ResetValues();
            GameManager.GM.ChangeTurn();
        }
    }

    private void Play()
    {
        if (currentAction == ActionState.None)
            EvaluateDefault();

        if (currentAction == ActionState.Offense)
            EvaluateOffense();
        else if (currentAction == ActionState.VeryOffensive)
            EvaluateVeryOffensive();
        else if (currentAction == ActionState.Defense)
            EvaluateDefense();
        else if (currentAction == ActionState.VeryDefensive)
            EvaluateVeryDefensive();

        selectedCard.UseCard();
    }

    private void ResetValues()
    {
        Debug.Log($"VO: {veryOffensiveMaxValue}, O: {offenseMaxValue}, D: {defenseMaxValue}, VD: {veryDefensiveMaxValue}");

        // Behaviour Default Values
        veryOffensiveMaxValue = 0.0f;
        offenseMaxValue = 0.0f;
        defenseMaxValue = 0.0f;
        veryDefensiveMaxValue = 0.0f;

        // Action Default Values
        attackMaxValue = 0.0f;
        shieldMaxValue = 0.0f;
        healMaxValue = 0.0f;
        kickMaxValue = 0.0f;

        // Card
        selectedCard = null;

        // State
        currentAction = ActionState.None;
    }

    private void EvaluateDefault()
    {
        // IF THEN Values
        if (ai.CRITICAL && player.CRITICAL)
            defenseMaxValue = (defenseMaxValue > Min(ai.criticalValue, player.criticalValue)) ? defenseMaxValue : Min(ai.criticalValue, player.criticalValue);
        if (ai.HURT && player.CRITICAL)
            offenseMaxValue = (offenseMaxValue > Min(ai.hurtValue, player.criticalValue)) ? offenseMaxValue : Min(ai.hurtValue, player.criticalValue);
        if (ai.HEALTHY && player.CRITICAL)
            veryOffensiveMaxValue = (veryOffensiveMaxValue > Min(ai.healthyValue, player.criticalValue)) ? veryOffensiveMaxValue : Min(ai.healthyValue, player.criticalValue);
        if (ai.CRITICAL && player.HURT)
            defenseMaxValue = (defenseMaxValue > Min(ai.criticalValue, player.hurtValue)) ? defenseMaxValue : Min(ai.criticalValue, player.hurtValue);
        if (ai.HURT && player.HURT)
            offenseMaxValue = (offenseMaxValue > Min(ai.hurtValue, player.hurtValue)) ? offenseMaxValue : Min(ai.hurtValue, player.hurtValue);
        if (ai.HEALTHY && player.HURT)
            offenseMaxValue = (offenseMaxValue > Min(ai.healthyValue, player.hurtValue)) ? offenseMaxValue : Min(ai.healthyValue, player.hurtValue);
        if (ai.CRITICAL && player.HEALTHY)
            veryDefensiveMaxValue = (veryDefensiveMaxValue > Min(ai.criticalValue, player.healthyValue)) ? veryDefensiveMaxValue : Min(ai.criticalValue, player.healthyValue);
        if (ai.HURT && player.HEALTHY)
            defenseMaxValue = (defenseMaxValue > Min(ai.hurtValue, player.healthyValue)) ? defenseMaxValue : Min(ai.hurtValue, player.healthyValue);
        if (ai.HEALTHY && player.HEALTHY)
            offenseMaxValue = (offenseMaxValue > Min(ai.healthyValue, player.healthyValue)) ? offenseMaxValue : Min(ai.healthyValue, player.healthyValue);

        // Create Flat Top Charts
        for (int i = 0; i < veryOffensiveValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = veryOffensiveCurve.Evaluate(j);
            veryOffensiveValues[i] = (evalatedValue > veryOffensiveMaxValue) ? veryOffensiveMaxValue : evalatedValue;
        }
        for (int i = 0; i < offenseValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = offenseCurve.Evaluate(j);
            offenseValues[i] = (evalatedValue > offenseMaxValue) ? offenseMaxValue : evalatedValue;
        }
        for (int i = 0; i < defenseValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = defenseCurve.Evaluate(j);
            defenseValues[i] = (evalatedValue > defenseMaxValue) ? defenseMaxValue : evalatedValue;
        }
        for (int i = 0; i < veryDefensiveValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = veryDefensiveCurve.Evaluate(j);
            veryDefensiveValues[i] = (evalatedValue > veryDefensiveMaxValue) ? veryDefensiveMaxValue : evalatedValue;
        }

        // Deffuzify
        float veryOffensiveCrisp = Deffuzify(veryOffensiveValues);
        float offensiveCrisp = Deffuzify(offenseValues);
        float defensiveCrisp = Deffuzify(defenseValues);
        float veryDefensiveCrisp = Deffuzify(veryDefensiveValues);

        float finalCrispValue = (veryOffensiveCrisp + offensiveCrisp + defensiveCrisp + veryDefensiveCrisp) / (veryOffensiveMaxValue + offenseMaxValue + defenseMaxValue + veryDefensiveMaxValue);
        // Debug.Log($"Final Crisp Value: {finalCrispValue}");

        // Change State
        if (finalCrispValue >= 75f)
            currentAction = ActionState.VeryDefensive;
        else if (finalCrispValue >= 35f)
            currentAction = ActionState.Defense;
        else if (finalCrispValue >= 20f)
            currentAction = ActionState.Offense;
        else
            currentAction = ActionState.VeryOffensive;
    }

    private void EvaluateOffense()
    {
        // IF THEN Values
        if (ai.EXHAUSTED && player.BROKEN)
            attackMaxValue = (attackMaxValue > Min(ai.exhaustedValue, player.brokenValue)) ? attackMaxValue : Min(ai.exhaustedValue, player.brokenValue);
        if (ai.TIRED && player.BROKEN)
            attackMaxValue = (attackMaxValue > Min(ai.tiredValue, player.brokenValue)) ? attackMaxValue : Min(ai.tiredValue, player.brokenValue);
        if (ai.ENERGETIC && player.BROKEN)
            attackMaxValue = (attackMaxValue > Min(ai.energeticValue, player.brokenValue)) ? attackMaxValue : Min(ai.energeticValue, player.brokenValue);
        if (ai.EXHAUSTED && player.DAMAGED)
            attackMaxValue = (attackMaxValue > Min(ai.exhaustedValue, player.damagedValue)) ? attackMaxValue : Min(ai.exhaustedValue, player.damagedValue);
        if (ai.TIRED && player.DAMAGED)
            attackMaxValue = (attackMaxValue > Min(ai.tiredValue, player.damagedValue)) ? attackMaxValue : Min(ai.tiredValue, player.damagedValue);
        if (ai.ENERGETIC && player.DAMAGED)
            kickMaxValue = (kickMaxValue > Min(ai.energeticValue, player.damagedValue)) ? kickMaxValue : Min(ai.energeticValue, player.damagedValue);
        if (ai.EXHAUSTED && player.ASNEW)
            attackMaxValue = (attackMaxValue > Min(ai.exhaustedValue, player.asNewValue)) ? attackMaxValue : Min(ai.exhaustedValue, player.asNewValue);
        if (ai.TIRED && player.ASNEW)
            kickMaxValue = (kickMaxValue > Min(ai.tiredValue, player.asNewValue)) ? kickMaxValue : Min(ai.tiredValue, player.asNewValue);
        if (ai.ENERGETIC && player.ASNEW)
            kickMaxValue = (kickMaxValue > Min(ai.energeticValue, player.asNewValue)) ? kickMaxValue : Min(ai.energeticValue, player.asNewValue);

        // Create Flat Top Charts
        for (int i = 0; i < attackValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = attackCurve.Evaluate(j);
            attackValues[i] = (evalatedValue > attackMaxValue) ? attackMaxValue : evalatedValue;
        }
        for (int i = 0; i < kickValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = kickCurve.Evaluate(j);
            kickValues[i] = (evalatedValue > kickMaxValue) ? kickMaxValue : evalatedValue;
        }

        // Deffuzify
        float attackCrisp = Deffuzify(attackValues);
        float kickCrisp = Deffuzify(kickValues);

        float finalCrispValue = (attackCrisp + kickCrisp) / (attackMaxValue + kickMaxValue);

        Debug.Log($"Final Crisp Value: {finalCrispValue}");

        // Change State
        if (finalCrispValue >= 50f)
            selectedCard = ai.kickCard.GetComponent<Card>();
        else
            selectedCard = ai.attackCard.GetComponent<Card>();
    }
    
    private void EvaluateVeryOffensive()
    {
        // IF THEN Values
        if (ai.EXHAUSTED && player.BROKEN)
            attackMaxValue = (attackMaxValue > Min(ai.exhaustedValue, player.brokenValue)) ? attackMaxValue : Min(ai.exhaustedValue, player.brokenValue);
        if (ai.TIRED && player.BROKEN)
            attackMaxValue = (attackMaxValue > Min(ai.tiredValue, player.brokenValue)) ? attackMaxValue : Min(ai.tiredValue, player.brokenValue);
        if (ai.ENERGETIC && player.BROKEN)
            attackMaxValue = (attackMaxValue > Min(ai.energeticValue, player.brokenValue)) ? attackMaxValue : Min(ai.energeticValue, player.brokenValue);
        if (ai.EXHAUSTED && player.DAMAGED)
            attackMaxValue = (attackMaxValue > Min(ai.exhaustedValue, player.damagedValue)) ? attackMaxValue : Min(ai.exhaustedValue, player.damagedValue);
        if (ai.TIRED && player.DAMAGED)
            attackMaxValue = (attackMaxValue > Min(ai.tiredValue, player.damagedValue)) ? attackMaxValue : Min(ai.tiredValue, player.damagedValue);
        if (ai.ENERGETIC && player.DAMAGED)
            attackMaxValue = (attackMaxValue > Min(ai.energeticValue, player.damagedValue)) ? attackMaxValue : Min(ai.energeticValue, player.damagedValue);
        if (ai.EXHAUSTED && player.ASNEW)
            attackMaxValue = (attackMaxValue > Min(ai.exhaustedValue, player.asNewValue)) ? attackMaxValue : Min(ai.exhaustedValue, player.asNewValue);
        if (ai.TIRED && player.ASNEW)
            attackMaxValue = (attackMaxValue > Min(ai.tiredValue, player.asNewValue)) ? attackMaxValue : Min(ai.tiredValue, player.asNewValue);
        if (ai.ENERGETIC && player.ASNEW)
            kickMaxValue = (kickMaxValue > Min(ai.energeticValue, player.asNewValue)) ? kickMaxValue : Min(ai.energeticValue, player.asNewValue);

        // Create Flat Top Charts
        for (int i = 0; i < attackValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = attackCurve.Evaluate(j);
            attackValues[i] = (evalatedValue > attackMaxValue) ? attackMaxValue : evalatedValue;
        }
        for (int i = 0; i < kickValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = kickCurve.Evaluate(j);
            kickValues[i] = (evalatedValue > kickMaxValue) ? kickMaxValue : evalatedValue;
        }

        // Deffuzify
        float attackCrisp = Deffuzify(attackValues);
        float kickCrisp = Deffuzify(kickValues);

        float finalCrispValue = (attackCrisp + kickCrisp) / (attackMaxValue + kickMaxValue);

        Debug.Log($"Final Crisp Value: {finalCrispValue}");

        // Change State
        if (finalCrispValue >= 50f)
            selectedCard = ai.kickCard.GetComponent<Card>();
        else
            selectedCard = ai.attackCard.GetComponent<Card>();
    }

    private void EvaluateDefense()
    {
        // IF THEN Values
        if (ai.BROKEN && player.EXHAUSTED)
            shieldMaxValue = (shieldMaxValue > Min(ai.brokenValue, player.exhaustedValue)) ? shieldMaxValue : Min(ai.brokenValue, player.exhaustedValue);
        if (ai.DAMAGED && player.EXHAUSTED)
            shieldMaxValue = (shieldMaxValue > Min(ai.damagedValue, player.exhaustedValue)) ? shieldMaxValue : Min(ai.damagedValue, player.exhaustedValue);
        if (ai.ASNEW && player.EXHAUSTED)
            healMaxValue = (healMaxValue > Min(ai.asNewValue, player.exhaustedValue)) ? healMaxValue : Min(ai.asNewValue, player.exhaustedValue);
        if (ai.BROKEN && player.TIRED)
            shieldMaxValue = (shieldMaxValue > Min(ai.brokenValue, player.tiredValue)) ? shieldMaxValue : Min(ai.brokenValue, player.tiredValue);
        if (ai.DAMAGED && player.TIRED)
            healMaxValue = (healMaxValue > Min(ai.damagedValue, player.tiredValue)) ? healMaxValue : Min(ai.damagedValue, player.tiredValue);
        if (ai.ASNEW && player.TIRED)
            healMaxValue = (healMaxValue > Min(ai.asNewValue, player.tiredValue)) ? healMaxValue : Min(ai.asNewValue, player.tiredValue);
        if (ai.BROKEN && player.ENERGETIC)
            healMaxValue = (healMaxValue > Min(ai.brokenValue, player.energeticValue)) ? healMaxValue : Min(ai.brokenValue, player.energeticValue);
        if (ai.DAMAGED && player.ENERGETIC)
            healMaxValue = (healMaxValue > Min(ai.damagedValue, player.energeticValue)) ? healMaxValue : Min(ai.damagedValue, player.energeticValue);
        if (ai.ASNEW && player.ENERGETIC)
            healMaxValue = (healMaxValue > Min(ai.asNewValue, player.energeticValue)) ? healMaxValue : Min(ai.asNewValue, player.energeticValue);

        // Create Flat Top Charts
        for (int i = 0; i < shieldValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = shieldCurve.Evaluate(j);
            shieldValues[i] = (evalatedValue > shieldMaxValue) ? shieldMaxValue : evalatedValue;
        }
        for (int i = 0; i < healValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = healCurve.Evaluate(j);
            healValues[i] = (evalatedValue > healMaxValue) ? healMaxValue : evalatedValue;
        }

        // Deffuzify
        float shieldCrisp = Deffuzify(shieldValues);
        float healCrisp = Deffuzify(healValues);

        float finalCrispValue = (shieldCrisp + healCrisp) / (shieldMaxValue + healMaxValue);

        Debug.Log($"Final Crisp Value: {finalCrispValue}");

        // Change State
        if (finalCrispValue >= 50f)
            selectedCard = ai.healCard.GetComponent<Card>();
        else
            selectedCard = ai.shieldCard.GetComponent<Card>();
    }

    private void EvaluateVeryDefensive()
    {
        // IF THEN Values
        if (ai.BROKEN && player.EXHAUSTED)
            shieldMaxValue = (shieldMaxValue > Min(ai.brokenValue, player.exhaustedValue)) ? shieldMaxValue : Min(ai.brokenValue, player.exhaustedValue);
        if (ai.DAMAGED && player.EXHAUSTED)
            healMaxValue = (healMaxValue > Min(ai.damagedValue, player.exhaustedValue)) ? healMaxValue : Min(ai.damagedValue, player.exhaustedValue);
        if (ai.ASNEW && player.EXHAUSTED)
            healMaxValue = (healMaxValue > Min(ai.asNewValue, player.exhaustedValue)) ? healMaxValue : Min(ai.asNewValue, player.exhaustedValue);
        if (ai.BROKEN && player.TIRED)
            healMaxValue = (healMaxValue > Min(ai.brokenValue, player.tiredValue)) ? healMaxValue : Min(ai.brokenValue, player.tiredValue);
        if (ai.DAMAGED && player.TIRED)
            healMaxValue = (healMaxValue > Min(ai.damagedValue, player.tiredValue)) ? healMaxValue : Min(ai.damagedValue, player.tiredValue);
        if (ai.ASNEW && player.TIRED)
            healMaxValue = (healMaxValue > Min(ai.asNewValue, player.tiredValue)) ? healMaxValue : Min(ai.asNewValue, player.tiredValue);
        if (ai.BROKEN && player.ENERGETIC)
            healMaxValue = (healMaxValue > Min(ai.brokenValue, player.energeticValue)) ? healMaxValue : Min(ai.brokenValue, player.energeticValue);
        if (ai.DAMAGED && player.ENERGETIC)
            healMaxValue = (healMaxValue > Min(ai.damagedValue, player.energeticValue)) ? healMaxValue : Min(ai.damagedValue, player.energeticValue);
        if (ai.ASNEW && player.ENERGETIC)
            healMaxValue = (healMaxValue > Min(ai.asNewValue, player.energeticValue)) ? healMaxValue : Min(ai.asNewValue, player.energeticValue);

        // Create Flat Top Charts
        for (int i = 0; i < shieldValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = shieldCurve.Evaluate(j);
            shieldValues[i] = (evalatedValue > shieldMaxValue) ? shieldMaxValue : evalatedValue;
        }
        for (int i = 0; i < healValues.Length; ++i)
        {
            float j = i * 0.01f;
            float evalatedValue = healCurve.Evaluate(j);
            healValues[i] = (evalatedValue > healMaxValue) ? healMaxValue : evalatedValue;
        }

        // Deffuzify
        float shieldCrisp = Deffuzify(shieldValues);
        float healCrisp = Deffuzify(healValues);

        float finalCrispValue = (shieldCrisp + healCrisp) / (shieldMaxValue + healMaxValue);

        Debug.Log($"Final Crisp Value: {finalCrispValue}");

        // Change State
        if (finalCrispValue >= 50f)
            selectedCard = ai.healCard.GetComponent<Card>();
        else
            selectedCard = ai.shieldCard.GetComponent<Card>();
    }

    /// <summary>
    /// Deffuzify using Average of Maxima
    /// </summary>
    private float Deffuzify(float[] flatTopArray)
    {
        float maxValue = 0.0f;
        int maxStart = -1;
        int maxEnd = -1;
        float midPoint = 0.0f;

        // Get Max Value
        for (int i = 0; i < flatTopArray.Length; ++i)
        {
            if (flatTopArray[i] > maxValue)
                maxValue = flatTopArray[i];
        }

        // Get Reference Number
        for (int i = 0; i < flatTopArray.Length; ++i)
        {
            if (maxValue == 0.0f)
                break;
            if (flatTopArray[i] == maxValue)
            {
                if (maxStart == -1)
                {
                    maxStart = i;
                    maxEnd = i;
                }
                else if (flatTopArray[i] != maxValue)
                    break;
                else
                    maxEnd = i;
            }
        }
        midPoint = (maxStart + maxEnd) * 0.5f;

        // Return Deffuzified Number
        return (midPoint * maxValue);
    }

    /// <summary>
    /// Hedges to be used in case of needing Fairly or Very variants.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Updates an AI state value from the min value.
    /// </summary>
    /// <returns></returns>
    private float Min(float val1, float val2)
    {
        return (val1 < val2) ? val1 : val2;
    }
}