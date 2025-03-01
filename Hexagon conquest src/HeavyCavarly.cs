using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyCavarly : BaseUnit
{
    public HeavyCavarly()
    {
        Movement = 4;
        AttackRange = 1;
        _maxHP = 14;
        _health = 14;
        _attack = 7;
        _defense = 3;
        _agility = -5;
        _precision = 105;
    }
}
