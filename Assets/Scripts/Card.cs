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
    [SerializeField] int effectValue = 1;

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
                targetPlayer.MHealth -= (effectValue - targetPlayer.MShieldDurability);
                selfPlayer.MKickRecharge = (selfPlayer.MKickRecharge >= selfPlayer.maxKickRecharge) ? selfPlayer.maxKickRecharge : selfPlayer.MKickRecharge + 1;
                break;
            case CardType.Shield:
                selfPlayer.MShieldDurability = (selfPlayer.MShieldDurability >= selfPlayer.maxShieldDurability) ? selfPlayer.maxShieldDurability : selfPlayer.MShieldDurability + effectValue;
                break;
            case CardType.Kick:
                int result = targetPlayer.MShieldDurability - (selfPlayer.MKickRecharge * effectValue);
                if (result <= 0)
                    targetPlayer.MShieldDurability = 0;
                else
                    targetPlayer.MShieldDurability = result;
                selfPlayer.MKickRecharge = 0;
                break;
            case CardType.Heal:
                selfPlayer.MHealth = (selfPlayer.MHealth >= selfPlayer.maxHealth) ? selfPlayer.maxHealth : selfPlayer.MHealth + effectValue;
                break;
            default:
                break;
        }
    }
}
