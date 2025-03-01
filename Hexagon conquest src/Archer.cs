using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : BaseUnit
{
    public Archer()
    {
        Movement = 3;
        AttackRange = 2;
        _maxHP = 8;
        _health = 8;
        _attack = 4;
        _defense = 1;
        _agility = 20;
        _precision = 135;
    }
}
