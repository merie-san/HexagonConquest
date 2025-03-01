using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordman : BaseUnit
{
    public Swordman()
    {
        Movement = 3;
        AttackRange = 1;
        _maxHP = 10;
        _health = 10;
        _attack = 4;
        _defense = 1;
        _agility = 30;
        _precision = 130;
    }
}
