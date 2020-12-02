using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager gm;
    public static GameManager GM
    {
        get { return gm; }
    }

    public event System.Action<WhoTurn> OnChangeTurn;

    public enum WhoTurn
    {
        Player,
        AI
    }

    public WhoTurn turn = WhoTurn.Player;
    public Player player = null;
    public AIAgent ai = null;

    private void Awake()
    {
        if (gm != null && gm != this)
            Destroy(this.gameObject);
        else
            gm = this;
    }

    private void Update()
    {
        if (player.MHealth <= 0)
        {
            Debug.Log("AI Wins");
        }
        if (ai.MHealth <= 0)
        {
            Debug.Log("Player Wins");
        }
    }

    public void ChangeTurn()
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
                break;
            case WhoTurn.AI:
                ai.GetComponent<CardSelection>().UseCard();
                break;
            default:
                break;
        }
        ChangeTurn();
    }
}