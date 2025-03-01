using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearCavalry : BaseUnit
{
    public SpearCavalry()
    {
        Movement = 5;
        AttackRange = 1;
        _maxHP = 10;
        _health = 10;
        _attack = 7;
        _defense = 2;
        _agility = 15;
        _precision = 115;
    }
}
