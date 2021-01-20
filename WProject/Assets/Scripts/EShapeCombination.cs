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


public enum ESpell
{
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
    Light = 40,
    Simple = 50,
    Strong = 100,
}

public enum ESpellType
{ 
    Heal,
    Armor,
    Attack,
    Shield,
    SelfDamages
}