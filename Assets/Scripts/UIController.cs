using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [SerializeField] private Slider playerHealth;
    [SerializeField] private Slider playerShield;
    [SerializeField] private Slider playerKick;
    [SerializeField] private Slider aiHealth;
    [SerializeField] private Slider aiShield;
    [SerializeField] private Slider aiKick;
    [SerializeField] private Button useBtn;

    private void Start()
    {
        gm.OnChangeTurn += ChangeTurn;
        playerHealth.maxValue = gm.player.maxHealth;
        playerShield.maxValue = gm.player.maxShieldRecharge;
        playerKick.maxValue = gm.player.maxKickRecharge;
        aiShield.maxValue = gm.ai.maxShieldRecharge;
        aiHealth.maxValue = gm.ai.maxHealth;
        aiKick.maxValue = gm.ai.maxKickRecharge;
    }

    private void Update()
    {
        playerHealth.value = gm.player.MHealth;
        playerShield.value = gm.player.MShieldDruability;
        playerKick.value = gm.player.MKickRecharge;
        aiHealth.value = gm.ai.MHealth;
        aiShield.value = gm.ai.MShieldDruability;
        aiKick.value = gm.ai.MKickRecharge;
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
