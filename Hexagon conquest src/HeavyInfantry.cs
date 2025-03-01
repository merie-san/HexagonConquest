using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyInfantry : BaseUnit
{
    public HeavyInfantry()
    {
        Movement = 2;
        AttackRange = 1;
        _maxHP = 12;
        _health = 12;
        _attack = 5;
        _defense = 3;
        _agility = 0;
        _precision = 110;
    }
}
