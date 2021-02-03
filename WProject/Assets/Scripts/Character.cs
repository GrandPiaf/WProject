using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [Header("Character Stats")]
    public Stats stats;

    public float mana;
    public float manaRegen = 0.5f;
    public int manaMax = 100;

    public float attackDelay = 5f;
    public float timer = 0f;

    public bool isDead = false;

    [HideInInspector]
    public ESpellForce shield;
    [HideInInspector]
    public bool reflectingShield = false;

    private void FixedUpdate() {

        mana += manaRegen;

        if (mana > manaMax)
            mana = manaMax;
    }

    virtual public void Update() {
        timer += Time.deltaTime;
        if(stats.currentHealth <= 0) {
            isDead = true;
        }
    }

     virtual public void ResetCharacter() {
        this.stats.Heal(stats.maxHealth.baseValue);
        this.mana = manaMax;
        this.isDead = false;
    }

    public bool PlayCard(Spell spell, Character target) {

        if (timer >= attackDelay) {
            timer = 0;

            if (mana < (int)spell.manaCost) return false;

            foreach (SpellComponent component in spell.components) {
                HandleSpellComponnent(component, this, target);
            }
            mana -= (int)spell.manaCost;

            Debug.Log("Spell played : " + spell.type.ToString() + " launched by : " + gameObject.name);

            return true;
        }
        return false;

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
