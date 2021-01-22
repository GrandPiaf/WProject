using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell
{
    public List<SpellData> data;
    public EManaCost manaCost;
}

public struct SpellData
{
    public ESpellName spell;
    public ESpellForce force;
    public ESpellType type;

    public SpellData(ESpellName _spell, ESpellForce _force, ESpellType _type)
    {
        spell = _spell;
        force = _force;
        type = _type;
    }
}
