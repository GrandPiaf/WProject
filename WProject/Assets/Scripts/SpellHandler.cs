using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellHandler
{
    public static Spell ConvertShapeToSpell(EShapeCombination shapeCombination)
    {
        ESpellName spell;
        switch (shapeCombination)
        {
            case EShapeCombination.SIMPLE_RECTANGLE:
                spell = ESpellName.Shield;
                break;
            case EShapeCombination.SIMPLE_TRIANGLE:
                spell = ESpellName.Attack;
                break;
            case EShapeCombination.SIMPLE_CIRCLE:
                spell = ESpellName.Heal;
                break;
            case EShapeCombination.DOUBLE_RECTANGLE:
                spell = ESpellName.StrongShield;
                break;
            case EShapeCombination.DOUBLE_TRIANGLE:
                spell = ESpellName.StrongAttack;
                break;
            case EShapeCombination.DOUBLE_CIRCLE:
                spell = ESpellName.StrongHeal;
                break;
            case EShapeCombination.RECTANGLE_TRIANGLE:
                spell = ESpellName.ReflectShield;
                break;
            case EShapeCombination.RECTANGLE_CIRCLE:
                spell = ESpellName.HealShield;
                break;
            case EShapeCombination.TRIANGLE_RECTANGLE:
                spell = ESpellName.ShieldAttack;
                break;
            case EShapeCombination.TRIANGLE_CIRCLE:
                spell = ESpellName.LifeSteal;
                break;
            case EShapeCombination.CIRCLE_RECTANGLE:
                spell = ESpellName.Armor;
                break;
            case EShapeCombination.CIRCLE_TRIANGLE:
                spell = ESpellName.Sacrifice;
                break;
            default:
                spell = ESpellName.None;
                break;
        }

        return CreateSpell(spell);
    }

    public static Spell CreateSpell(ESpellName spellName)
    {
        List<SpellData> spellDatas = new List<SpellData>();
        EManaCost manaCost = 0;

        switch (spellName)
        {
            case ESpellName.Shield:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Shield));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.Attack:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Attack));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.Heal:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Heal));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.StrongShield:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Strong, ESpellType.Shield));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.StrongAttack:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Strong, ESpellType.Attack));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.StrongHeal:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Strong, ESpellType.Heal));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.ReflectShield:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Shield));
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Attack));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.HealShield:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Shield));
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Heal));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.ShieldAttack:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Attack));
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Shield));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.LifeSteal:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Heal));
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Shield));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.Armor:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Armor));
                manaCost = EManaCost.Small;
                break;
            case ESpellName.Sacrifice:
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.SelfDamages));
                spellDatas.Add(new SpellData(spellName, ESpellForce.Simple, ESpellType.Attack));
                manaCost = EManaCost.Small;
                break;
            default:
                break;
        }

        Spell spell = new Spell();
        spell.data = spellDatas;
        spell.manaCost = manaCost;
        return spell;
    }
}
