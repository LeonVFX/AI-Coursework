using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event System.Action<WhoTurn> OnChangeTurn;

    public enum WhoTurn
    {
        Player,
        AI
    }

    public WhoTurn turn = WhoTurn.Player;
    public Player player = null;
    public Player ai = null;

    private void Update()
    {
        // For Debugging Purposes
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ai.GetComponent<AIAgent>().EvaluateValues();
        }
    }

    private void ChangeTurn()
    {
        switch (turn)
        {
            case WhoTurn.Player:
                turn = WhoTurn.AI;
                break;
            case WhoTurn.AI:
                turn = WhoTurn.Player;
                break;
            default:
                break;
        }

        OnChangeTurn?.Invoke(turn);
    }

    public void UseCard()
    {
        switch (turn)
        {
            case WhoTurn.Player:
                player.GetComponent<CardSelection>().UseCard();
                player.MKickRecharge++;
                break;
            case WhoTurn.AI:
                ai.GetComponent<CardSelection>().UseCard();
                ai.MKickRecharge++;
                break;
            default:
                break;
        }
        ChangeTurn();
    }
}