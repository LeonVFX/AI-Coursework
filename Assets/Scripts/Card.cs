using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardType
    {
        Attack,
        Block,
        Kick,
        Heal
    }

    [SerializeField] private CardType cardType;
    private GameManager gm = null;
    private Player selfPlayer = null;
    private Player targetPlayer = null;
    [SerializeField] int healthValue = 1;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        selfPlayer = gm.player;
        targetPlayer = gm.ai;
        gm.OnChangeTurn += ChangeTarget;
    }

    private void ChangeTarget(GameManager.WhoTurn turn)
    {
        switch (turn)
        {
            case GameManager.WhoTurn.Player:
                selfPlayer = gm.player;
                targetPlayer = gm.ai;
                break;
            case GameManager.WhoTurn.AI:
                selfPlayer = gm.ai;
                targetPlayer = gm.player;
                break;
            default:
                break;
        }
    }

    public void UseCard()
    {
        switch (cardType)
        {
            case CardType.Attack:
                targetPlayer.MHealth -= healthValue;
                break;
            case CardType.Block:

                break;
            case CardType.Kick:

                break;
            case CardType.Heal:
                selfPlayer.MHealth += healthValue;
                break;
            default:
                break;
        }
    }
}
