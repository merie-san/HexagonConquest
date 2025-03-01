using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : BaseUnit
{
    public Wizard()
    {
        Movement = 3;
        AttackRange = 2;
        _maxHP = 8;
        _health = 8;
        _magicAttack = 5;
        _defense = 0;
        _agility = 10;
        _precision = 150;
    }
}
