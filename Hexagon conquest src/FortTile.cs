using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortTile : Tile
{
    public FortTile()
    {
        temporaryDefense = 2;
        temporaryAgility = 10;
        hpRegenerationTurn = 2;
    }

}
