using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider playerHealth;
    [SerializeField] private Slider playerShield;
    [SerializeField] private Slider playerKick;
    [SerializeField] private Slider aiHealth;
    [SerializeField] private Slider aiShield;
    [SerializeField] private Slider aiKick;
    [SerializeField] private Button useBtn;

    private void Start()
    {
        GameManager.GM.OnChangeTurn += ChangeTurn;
        playerHealth.maxValue = GameManager.GM.player.maxHealth;
        playerShield.maxValue = GameManager.GM.player.maxShieldRecharge;
        playerKick.maxValue = GameManager.GM.player.maxKickRecharge;
        aiShield.maxValue = GameManager.GM.ai.maxShieldRecharge;
        aiHealth.maxValue = GameManager.GM.ai.maxHealth;
        aiKick.maxValue = GameManager.GM.ai.maxKickRecharge;
    }

    private void Update()
    {
        playerHealth.value = GameManager.GM.player.MHealth;
        playerShield.value = GameManager.GM.player.MShieldDurability;
        playerKick.value = GameManager.GM.player.MKickRecharge;
        aiHealth.value = GameManager.GM.ai.MHealth;
        aiShield.value = GameManager.GM.ai.MShieldDurability;
        aiKick.value = GameManager.GM.ai.MKickRecharge;
    }

    private void ChangeTurn(GameManager.WhoTurn turn)
    {
        switch (turn)
        {
            case GameManager.WhoTurn.Player:
                useBtn.interactable = true;
                break;
            case GameManager.WhoTurn.AI:
                useBtn.interactable = false;
                break;
            default:
                break;
        }
    }
}
