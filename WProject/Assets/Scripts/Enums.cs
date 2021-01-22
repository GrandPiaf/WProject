using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EShapeCombination
{
    SIMPLE_RECTANGLE,
    SIMPLE_TRIANGLE,
    SIMPLE_CIRCLE,
    DOUBLE_RECTANGLE,
    DOUBLE_TRIANGLE,
    DOUBLE_CIRCLE,
    RECTANGLE_TRIANGLE,
    RECTANGLE_CIRCLE,
    TRIANGLE_RECTANGLE,
    TRIANGLE_CIRCLE,
    CIRCLE_RECTANGLE,
    CIRCLE_TRIANGLE
}


public enum ESpellName
{
    None,
    Shield,
    Attack,
    Heal,
    StrongShield,
    StrongAttack,
    StrongHeal,
    ReflectShield,
    HealShield,
    ShieldAttack,
    LifeSteal,
    Armor,
    Sacrifice
}

public enum ESpellForce
{
    None = 0,
    Light = 10,
    Simple = 30,
    Strong = 50,
}

public enum ESpellType
{ 
    Heal,
    Armor,
    Attack,
    Shield,
    SelfDamages
}

public enum EManaCost
{
    Small,
    Medium,
    High
}