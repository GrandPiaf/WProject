using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    internal Player player;
    internal EnemyBot[] enemies;


    private void Awake() {

        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(Instance);
            Instance = gameObject.GetComponent<GameManager>();
            return;
        }

        player = FindObjectOfType<Player>();
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.E)) {
            player.stats.TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            player.stats.armor.AddModifier(10);
        }
    }

    public int turnTime;
    public float timer;

    public void PlayCard(Spell spell, Character caster, Character target) {

        if (caster.mana < (int)spell.manaCost) return;

        foreach (SpellComponent component in spell.components) {
            HandleSpellComponnent(component, caster, target);
        }
        caster.mana -= (int)spell.manaCost;
    }

    public void HandleSpellComponnent(SpellComponent spellCompo, Character caster, Character target) {

        switch (spellCompo.effect) {
            case ESpellEffect.Heal:
                caster.stats.Heal((int)spellCompo.force);
                break;
            case ESpellEffect.Armor:
                caster.stats.armor.AddModifier((int)spellCompo.force);
                break;
            case ESpellEffect.Attack:
                target.stats.TakeDamage((int)spellCompo.force);
                break;
            case ESpellEffect.Shield:
                caster.stats.TakeDamage((int)spellCompo.force);
                break;
            case ESpellEffect.SelfDamages:
                caster.stats.TakeDamage((int)spellCompo.force);
                break;
            default:
                break;
        }
    }
}



