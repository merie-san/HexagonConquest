using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : BaseUnit
{
    public Catapult()
    {
        Movement = 2;
        AttackRange = 5;
        _maxHP = 15;
        _health = 15;
        _attack = 10;
        _defense = 3;
        _agility = -100;
        _precision = 40;
    }
}
