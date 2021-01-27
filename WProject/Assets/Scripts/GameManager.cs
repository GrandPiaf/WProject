using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    internal Player player;

    public EnemyBot[] enemies;

    public int turnTime;
    public float timer;

    //TEST
    public Dropdown testDropDown;

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

    private void Start() {

        #region TEST
        List<string> list = new List<string>();
        for (int i = 0; i <= (int)ESpell.Sacrifice; i++) {
            list.Add(((ESpell)i).ToString());
        }
        testDropDown.AddOptions(list);
        #endregion

    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.E)) {
            player.stats.TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            player.stats.armor.AddModifier(10);
        }
    }

    public void PlayCard(Spell spell, Character caster, Character target) {

        if (caster.mana < (int)spell.manaCost) return;

        foreach (SpellComponent component in spell.components) {
            HandleSpellComponnent(component, caster, target);
        }
        caster.mana -= (int)spell.manaCost;
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

    //TEST
    public void PlayTestCard() {

        string s = testDropDown.options[testDropDown.value].text;
        for (int i = 0; i <= (int)ESpell.Sacrifice; i++) {
            ESpell eSpell = (ESpell)i;

            if (s == eSpell.ToString()) {
                Spell spell = SpellBuilder.CreateSpell(eSpell);
                PlayCard(spell, player, enemies[0]);
                return;
            }
        }
    }
}



