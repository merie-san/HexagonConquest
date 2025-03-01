using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballista : BaseUnit
{
    public Ballista()
    {
        Movement = 2;
        AttackRange = 4;
        _maxHP = 15;
        _health = 15;
        _attack = 9;
        _defense = 3;
        _agility = -100;
        _precision = 60;
    }
}
