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

    public bool PlayCard(Spell spell, Character target) {

        if (mana < (int)spell.manaCost) return false;

        foreach (SpellComponent component in spell.components) {
            HandleSpellComponnent(component, this, target);
        }
        mana -= (int)spell.manaCost;
        return true;
    }

    public void HandleSpellComponnent(SpellComponent component, Character caster, Character target) {

        switch (component.effect) {
            case ESpellEffect.Heal:
                caster.stats.Heal((int)component.force);
                break;
            case ESpellEffect.Armor:
                caster.stats.armor.AddModifier((int)component.force);
                break;
            case ESpellEffect.Attack:

                if (target.shield != ESpellForce.None && target.reflectingShield) {
                    caster.stats.TakeDamage((int)target.shield);
                }

                if ((int)target.shield < (int)component.force) {
                    target.stats.TakeDamage((int)component.force);
                }
                target.shield = ESpellForce.None;

                break;
            case ESpellEffect.Shield:
                caster.shield = component.force;
                break;
            case ESpellEffect.SelfDamages:
                caster.stats.TakeDamage((int)component.force);
                break;
            default:
                break;
        }
    }
}
