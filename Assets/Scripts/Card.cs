using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardType
    {
        Attack,
        Shield,
        Kick,
        Heal
    }

    [SerializeField] private CardType cardType;
    private Player selfPlayer = null;
    private Player targetPlayer = null;
    [SerializeField] int healthValue = 1;

    private void Start()
    {
        selfPlayer = GameManager.GM.player;
        targetPlayer = GameManager.GM.ai;
        GameManager.GM.OnChangeTurn += ChangeTarget;
    }

    private void ChangeTarget(GameManager.WhoTurn turn)
    {
        switch (turn)
        {
            case GameManager.WhoTurn.Player:
                selfPlayer = GameManager.GM.player;
                targetPlayer = GameManager.GM.ai;
                break;
            case GameManager.WhoTurn.AI:
                selfPlayer = GameManager.GM.ai;
                targetPlayer = GameManager.GM.player;
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
                selfPlayer.MKickRecharge++;
                break;
            case CardType.Shield:

                break;
            case CardType.Kick:
                selfPlayer.MKickRecharge = 0;
                targetPlayer.MShieldDurability -= 2;
                break;
            case CardType.Heal:
                selfPlayer.MHealth += healthValue;
                break;
            default:
                break;
        }
    }
}
