using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCavalry : BaseUnit
{
    public SwordCavalry()
    {
        Movement = 5;
        AttackRange = 1;
        _maxHP = 12;
        _health = 12;
        _attack = 6;
        _defense = 2;
        _agility = 15;
        _precision = 110;
    }
}
