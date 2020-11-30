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
        Defense,
        Attack,
        Shield,
        Heal,
        Kick
    }

    private Player ai;
    private CardSelection selectedCard;
    private ActionState currentAction;

    [Header("Player Hand")]
    [SerializeField] private Player player;

    [Header("Action State Curve Defaults")]
    [SerializeField] private AnimationCurve veryOffensiveCurve;
    [SerializeField] private AnimationCurve offenseCurve;
    [SerializeField] private AnimationCurve defenseCurve;
    [SerializeField] private AnimationCurve veryDefensiveCurve;

    private float[] veryOffensiveValues;
    private float[] offenseValues;
    private float[] defenseValues;
    private float[] veryDefensiveValues;

    private float veryOffensiveMaxValue;
    private float offenseMaxValue;
    private float defenseMaxValue;
    private float veryDefensiveMaxValue;

    private void Start()
    {
        base.Start();

        ai = GetComponent<Player>();
        selectedCard = GetComponent<CardSelection>();

        // Arrays - 101 slots to accomodate 2 decimal point values for a broader range of values for Deffuzification.
        veryOffensiveValues = new float[101];
        offenseValues = new float[101];
        defenseValues = new float[101];
        veryDefensiveValues = new float[101];
    }

    private void Update()
    {
        if (GameManager.GM.turn == GameManager.WhoTurn.AI)
        {
            Play();
            //ResetValues();
            GameManager.GM.ChangeTurn();
        }
    }

    private void Play()
    {
        if (currentAction == ActionState.None)
            EvaluateDefault();
        if (currentAction == ActionState.Offense)
            EvaluateOffense();
        else if (currentAction == ActionState.Defense)
            EvaluateDefense();

        selectedCard.UseCard();
    }

    private void ResetValues()
    {
        // Action Default Values
        veryOffensiveMaxValue = 0.0f;
        offenseMaxValue = 0.0f;
        defenseMaxValue = 0.0f;
        veryDefensiveMaxValue = 0.0f;

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
        for (float i = 0.0f; i < veryOffensiveValues.Length * 0.01f; i += 0.01f)
        {
            float evalatedValue = veryOffensiveCurve.Evaluate(i);
            veryOffensiveValues[(int)(i * 100)] = (evalatedValue > veryOffensiveMaxValue) ? veryOffensiveMaxValue : evalatedValue;
        }
        for (float i = 0.0f; i < offenseValues.Length * 0.01f; i += 0.01f)
        {
            float evalatedValue = offenseCurve.Evaluate(i);
            offenseValues[(int)(i * 100)] = (evalatedValue > offenseMaxValue) ? offenseMaxValue : evalatedValue;
        }
        for (float i = 0.0f; i < defenseValues.Length * 0.01f; i += 0.01f)
        {
            float evalatedValue = defenseCurve.Evaluate(i);
            defenseValues[(int)(i * 100)] = (evalatedValue > defenseMaxValue) ? defenseMaxValue : evalatedValue;
        }
        for (float i = 0.0f; i < veryDefensiveValues.Length * 0.01f; i += 0.01f)
        {
            float evalatedValue = veryDefensiveCurve.Evaluate(i);
            veryDefensiveValues[(int)(i * 100)] = (evalatedValue > veryDefensiveMaxValue) ? veryDefensiveMaxValue : evalatedValue;
        }

        // Deffuzify
        float veryOffensiveCrisp = Deffuzify(veryOffensiveValues);
        float offensiveCrisp = Deffuzify(offenseValues);
        float defensiveCrisp = Deffuzify(defenseValues);
        float veryDefensiveCrisp = Deffuzify(veryDefensiveValues);

        float finalCrispValue = (veryOffensiveCrisp + offensiveCrisp + defensiveCrisp + veryDefensiveCrisp) / (veryOffensiveMaxValue + offenseMaxValue + defenseMaxValue + veryDefensiveMaxValue);
        Debug.Log($"Final Crisp Value: {finalCrispValue}");
    }

    private void EvaluateOffense()
    {

    }

    private void EvaluateDefense()
    {

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