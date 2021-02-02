using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [Header("Character Stats")]
    public Stats stats;

    public float mana;
    public float manaRegen = 0.5f;
    public int manaMax = 100;

    [HideInInspector]
    public ESpellForce shield;
    [HideInInspector]
    public bool reflectingShield = false;

    private void FixedUpdate() {

        mana += manaRegen;

        if (mana > manaMax)
            mana = manaMax;
    }
}
