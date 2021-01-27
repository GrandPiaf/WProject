using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellBuilder {
    public static Spell ConvertShapeToSpell(EShapeCombination shapeCombination) {
        ESpell spell;
        switch (shapeCombination) {
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
                spell = ESpell.None;
                break;
        }

        return CreateSpell(spell);
    }

    public static Spell CreateSpell(ESpell spellType) {

        List<SpellComponent> spellCompos = new List<SpellComponent>();
        EManaCost manaCost = 0;

        switch (spellType) {
            case ESpell.Shield:
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Shield));
                manaCost = EManaCost.Small;
                break;
            case ESpell.Attack:
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Attack));
                manaCost = EManaCost.Small;
                break;
            case ESpell.Heal:
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Heal));
                manaCost = EManaCost.Small;
                break;
            case ESpell.StrongShield:
                spellCompos.Add(new SpellComponent(ESpellForce.Strong, ESpellEffect.Shield));
                manaCost = EManaCost.Small;
                break;
            case ESpell.StrongAttack:
                spellCompos.Add(new SpellComponent(ESpellForce.Strong, ESpellEffect.Attack));
                manaCost = EManaCost.Small;
                break;
            case ESpell.StrongHeal:
                spellCompos.Add(new SpellComponent(ESpellForce.Strong, ESpellEffect.Heal));
                manaCost = EManaCost.Small;
                break;
            case ESpell.ReflectShield:
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Shield));
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Attack));
                manaCost = EManaCost.Small;
                break;
            case ESpell.HealShield:
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Shield));
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Heal));
                manaCost = EManaCost.Small;
                break;
            case ESpell.ShieldAttack:
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Attack));
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Shield));
                manaCost = EManaCost.Small;
                break;
            case ESpell.LifeSteal:
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Heal));
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Shield));
                manaCost = EManaCost.Small;
                break;
            case ESpell.Armor:
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Armor));
                manaCost = EManaCost.Small;
                break;
            case ESpell.Sacrifice:
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.SelfDamages));
                spellCompos.Add(new SpellComponent(ESpellForce.Simple, ESpellEffect.Attack));
                manaCost = EManaCost.Small;
                break;
            default:
                break;
        }

        return new Spell() { type = spellType, components = spellCompos, manaCost = manaCost };
    }
}
