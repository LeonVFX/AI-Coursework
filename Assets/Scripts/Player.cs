using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int mHealth;

    public int MHealth
    {
        get { return mHealth; }
        set { mHealth = value; }
    }

    private int mShieldDurability;

    public int MShieldDruability
    {
        get { return mShieldDurability; }
        set { mShieldDurability = value; }
    }

    private int mKickRecharge;

    public int MKickRecharge
    {
        get { return mKickRecharge; }
        set { mKickRecharge = value; }
    }

    public int maxHealth = 100;
    public int maxShieldRecharge = 100;
    public int maxKickRecharge = 100;

    private void Start()
    {
        mHealth = maxHealth;
        mShieldDurability = maxShieldRecharge;
        mKickRecharge = 0;
    }

}
