using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    internal Player player;
    internal EnemyBot[] enemies;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = gameObject.GetComponent<GameManager>();
            return;
        }

        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.stats.TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.stats.armor.AddModifier(10);
        }
    }

    public int turnTime;
    public float timer;

    public void PlayCard(Spell spell, Character caster, Character target)
    {
        if (caster.stats.mana.GetValue() < (int)spell.manaCost) return;

        foreach (SpellData spellData in spell.data)
        {
            HandleSpellData(spellData, caster, target);
        }
    }

    public void HandleSpellData(SpellData spellData, Character caster, Character target)
    {
        switch (spellData.type)
        {
            case ESpellType.Heal:
                caster.stats.Heal((int)spellData.force);
                break;
            case ESpellType.Armor:
                caster.stats.armor.AddModifier((int)spellData.force);
                break;
            case ESpellType.Attack:
                target.stats.TakeDamage((int)spellData.force);
                break;
            case ESpellType.Shield:
                caster.stats.TakeDamage((int)spellData.force);
                break;
            case ESpellType.SelfDamages:
                caster.stats.TakeDamage((int)spellData.force);
                break;
            default:
                break;
        }
    }
}



