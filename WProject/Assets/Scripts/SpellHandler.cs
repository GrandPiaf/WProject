using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellHandler
{
    public static void CastSpellHandler(EShapeCombination shapeCombination)
    {
        ESpell spell;
        switch (shapeCombination)
        {
            case EShapeCombination.SIMPLE_RECTANGLE:
                spell = ESpell.Shield;
                break;
            case EShapeCombination.SIMPLE_TRIANGLE:
                spell = ESpell.Attack;
                break;
            case EShapeCombination.SIMPLE_CIRCLE:
                spell = ESpell.Heal;
                break;
            case EShapeCombination.DOUBLE_RECTANGLE:
                spell = ESpell.StrongShield;
                break;
            case EShapeCombination.DOUBLE_TRIANGLE:
                spell = ESpell.StrongAttack;
                break;
            case EShapeCombination.DOUBLE_CIRCLE:
                spell = ESpell.StrongHeal;
                break;
            case EShapeCombination.RECTANGLE_TRIANGLE:
                spell = ESpell.ReflectShield;
                break;
            case EShapeCombination.RECTANGLE_CIRCLE:
                spell = ESpell.HealShield;
                break;
            case EShapeCombination.TRIANGLE_RECTANGLE:
                spell = ESpell.ShieldAttack;
                break;
            case EShapeCombination.TRIANGLE_CIRCLE:
                spell = ESpell.LifeSteal;
                break;
            case EShapeCombination.CIRCLE_RECTANGLE:
                spell = ESpell.Armor;
                break;
            case EShapeCombination.CIRCLE_TRIANGLE:
                spell = ESpell.Sacrifice;
                break;
            default:
                break;
        }
    }

    public static Spell GetSpellForce(ESpell spellName)
    {
        List<SpellData> spellDatas = new List<SpellData>();

        switch (spellName)
        {
            case ESpell.Shield:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Shield));
                break;
            case ESpell.Attack:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Attack));
                break;
            case ESpell.Heal:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Heal));
                break;
            case ESpell.StrongShield:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Strong, ESpellType.Shield));
                break;
            case ESpell.StrongAttack:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Strong, ESpellType.Attack));
                break;
            case ESpell.StrongHeal:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Strong, ESpellType.Heal));
                break;
            case ESpell.ReflectShield:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Shield));
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Attack));
                break;
            case ESpell.HealShield:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Shield));
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Heal));
                break;
            case ESpell.ShieldAttack:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Attack));
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Shield));
                break;
            case ESpell.LifeSteal:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Heal));
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Shield));
                break;
            case ESpell.Armor:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Armor));
                break;
            case ESpell.Sacrifice:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.SelfDamages));
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Attack));
                break;
            default:
                break;
        }

        Spell spell = new Spell();
        spell.data = spellDatas;
        return spell;
    }
}
