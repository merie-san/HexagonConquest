using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Support : BaseUnit
{
    public Support()
    {
        Movement = 3;
        AttackRange = 2;
        _maxHP = 8;
        _health = 8;
        _healingPower = 3;
        _attack = 0;
        _defense = 0;
        _agility = 10;
        _precision = 255;
    }
}
