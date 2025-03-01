using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spearman : BaseUnit
{
    public Spearman()
    {
        Movement = 3;
        AttackRange = 1;
        _maxHP = 10;
        _health = 10;
        _attack = 5;
        _defense = 1;
        _agility = 20;
        _precision = 120;
    }
}
