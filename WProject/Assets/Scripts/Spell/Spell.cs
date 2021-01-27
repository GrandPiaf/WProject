using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell
{
    public ESpell type;
    public List<SpellComponent> components;
    public EManaCost manaCost;
}

public struct SpellComponent
{
    public ESpellForce force;
    public ESpellEffect effect;

    public SpellComponent( ESpellForce _force, ESpellEffect _effect)
    {
        force = _force;
        effect = _effect;
    }
}
