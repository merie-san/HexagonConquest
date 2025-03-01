using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public const string TURN_ENDED = "TURN_ENDED";
    public const string MONEY_SPENT = "MONEY_SPENT";
    public const string CREATING_UNIT = "CREATING_UNIT";
    public const string HP_CHANGED = "HP_CHANGED";
    public const string WHP_CHANGED = "WHP_CHANGED";
    public const string UNIT_INTERACTION = "UNIT_INTERACTION";
    public const string WALL_INTERACTION = "WALL_INTERACTION";
    public const string REPAIR_WALL = "REPAIR_WALL";
    public const string UPGRADE_WALL = "UPGRADE_WALL";
    public const string GAME_STATUS_CHANGED = "GAME_STATUS_CHANGED";
}
