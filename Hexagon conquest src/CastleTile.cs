using UnityEngine;
public class CastleTile : Tile
{
    private Wall wall;
    public Wall Wall { get { return wall; } set { wall = value; } }
    public CastleTile()
    {
        temporaryDefense = 2;
        temporaryAgility = 10;
        hpRegenerationTurn = 3;
    }

    override public bool IsAccessible(int factionID)
    {
        return wall.Faction.ID == factionID || wall.HP() <= 0;
    }
}
