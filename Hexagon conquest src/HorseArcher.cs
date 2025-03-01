using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseArcher : BaseUnit
{
    public HorseArcher()
    {
        Movement = 5;
        AttackRange = 3;
        _maxHP = 10;
        _health = 10;
        _attack = 5;
        _defense = 2;
        _agility = 15;
        _precision = 130;
    }
}
